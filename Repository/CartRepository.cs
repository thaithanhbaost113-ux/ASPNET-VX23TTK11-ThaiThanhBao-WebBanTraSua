using AutoMapper;
using Project.Areas.Identity.Data;
using Project.Models;
using Microsoft.EntityFrameworkCore;
using System.Text;
using System;
using Microsoft.AspNetCore.Mvc;
using System.Drawing.Drawing2D;
using static System.Reflection.Metadata.BlobBuilder;
using PayPal.Api;
using AutoMapper.Execution;
using Microsoft.AspNetCore.Identity;

namespace Project.Repository
{

    public class CartRepository : ICartRepository
    {
        private readonly DataContext _context;
        private readonly IMapper _mapper;
        private Random _random = new Random();
        private UserManager<ApplicationUser> _userManager;
        public CartRepository(DataContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
        }
        public string RandomId()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            StringBuilder idBuilder = new StringBuilder();
            for (int i = 0; i < 10; i++)
            {
                idBuilder.Append(chars[_random.Next(chars.Length)]);
            }
            return idBuilder.ToString();
        }
        public async Task<List<CartDetail>> ShowCartAsync(string Email)
        {
            var ListItem = _context.CartDetails.Where(x=> x.IdcartNavigation.User.Email == Email).Include(db => db.IdProductNavigation).ThenInclude(db => db.Images).ToList();
            return ListItem;
        }
        public async Task<CartItemModel> ShowItemSelectAsync(string Email, string idProduct)
        {
            //ShowCartAsync(email);
            var ListItem = _context.CartDetails
            .Where(CartDetails => CartDetails.IdcartNavigation.User.Email == Email && CartDetails.IdProduct == idProduct && CartDetails.IdcartNavigation.Status == true)
            .Join(_context.Products,
                CartDetails => CartDetails.IdProduct,
                Products => Products.Id,
                (CartDetails, Products) => new CartItemModel
                {
                    Email = CartDetails.IdcartNavigation.User.Email,
                    Id = CartDetails.Id,
                    IdProduct = CartDetails.IdProduct,
                    Name = Products.NameProduct,
                    Picture = Products.Images.FirstOrDefault(x => x.IsMain == true).UrlImage,
                    Number = CartDetails.Number,
                    Status = CartDetails.IdcartNavigation.Status,
                    Price = Products.Price,

                });
            return ListItem.FirstOrDefault();
        }
        public async Task<CartItemModel> ShowItemSelecByIDAsync(string email, string idItem)
        {
            //ShowCartAsync(email);
            var ListItem = _context.CartDetails
            .Where(CartDetails => CartDetails.IdcartNavigation.User.Email == email && CartDetails.Id == idItem)
            .Join(_context.Products,
                CartDetails => CartDetails.IdProduct,
                Products => Products.Id,
                (CartDetails, Products) => new CartItemModel
                {
                    Email = CartDetails.IdcartNavigation.User.Email,
                    Id = CartDetails.Id,
                    IdProduct = CartDetails.IdProduct,
                    Name = Products.NameProduct,
                    Picture = Products.Images.Where(x => x.IsMain == true).Select(x => x.UrlImage).FirstOrDefault(),
                    Number = CartDetails.Number,
                    Status = CartDetails.IdcartNavigation.Status,
                    Price = Products.Price,

                });
            return ListItem.FirstOrDefault();
        }

        public async Task newCartAsync(string id, string Email)
        {
            var user = await _userManager.FindByNameAsync(Email);
            var NewItem = _context.Carts.Add(new Cart 
            { 
                Id = id,
                UserId = user.Id, 
                Status = true,
            });
            await _context.SaveChangesAsync();
        }
        public async Task AddItemAsync(CartDetail model, string email)
        {
            var cart = _context.Carts.FirstOrDefault(x => x.User.Email == email);
            string idcart;
            if (cart == null)
            {
                var idCart = RandomId();
                await newCartAsync(idCart, email);
                idcart = idCart;
            }
            else
            {
                idcart = cart.Id;
            }
            var item = _context.CartDetails!.SingleOrDefault(x => x.IdProduct == model.IdProduct && x.IdcartNavigation.Status == true && x.IdcartNavigation.User.Email == email);
            if (item == null)
            {
                model.Idcart = idcart;
                var NewItem = _mapper.Map<CartDetail>(model);
                await _context.CartDetails!.AddAsync(NewItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                item.Number = item.Number + model.Number;
                await _context.SaveChangesAsync();
            }
        }
        public async Task UpdateCartAsync(string userId, Dictionary<string, int> quantities)
        {
            var cartItems = await _context.CartDetails
                .Where(c => c.IdcartNavigation.UserId == userId)
                .ToListAsync();
            foreach (var cartItem in cartItems)
            {
                if (quantities.TryGetValue(cartItem.Id, out var newQuantity))
                {
                    cartItem.Number = newQuantity;
                }
            }
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCartItemAsync(string email, string Id)
        {
            var cartItem = _context.CartDetails!.Include(db => db.IdProductNavigation).FirstOrDefault(x => x.Id == Id);
            _context.CartDetails!.RemoveRange(cartItem);
            var Product = _context.Products!.SingleOrDefault(x => x.Id == cartItem.IdProduct);
            Product.Sales += cartItem.Number;
            _context.SaveChanges();
        }

        public async Task ChangeNumberAsync(string email, string Id, int number)
        {
            var item = _context.CartDetails!.SingleOrDefault(x => x.Id == Id && x.IdcartNavigation.User.Email == email);
            item.Number = number;
            await _context.SaveChangesAsync();
        }

        public async Task RemoveItenAsync(string IdItem)
        {
            var DeleteItem = _context.CartDetails!.SingleOrDefault(x => x.Id == IdItem);
            if (DeleteItem != null)
            {
                _context.CartDetails!.Remove(DeleteItem);
                _context.SaveChanges();
            }
        }
    }
}
