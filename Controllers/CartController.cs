using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Project.Repository;
using Project.Models;
using System.Text;
using Project.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Project.Controllers
{
    [Route("[Controller]/[action]")]
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        public readonly ICartRepository _cartRepo;
        public readonly IProductRepository _ProductRepo;
        //public readonly DataContext _context;
        private UserManager<ApplicationUser> _userManager;

        private readonly Random _random = new Random();
        public CartController(ICartRepository cartRepo, IProductRepository ProductRepo, UserManager<ApplicationUser> userManager)
        {
            _cartRepo = cartRepo;
            _ProductRepo = ProductRepo;
            _userManager = userManager;
        }
        [NonAction]
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
        [HttpGet]
        public async Task<IActionResult> ShowCart()
        {
            string email = HttpContext.User.Identity.Name;
            return View(await _cartRepo.ShowCartAsync(email));
        }
        [HttpGet]
        public async Task<IActionResult> AddItem(string IDProduct, int number)
        {
            try
            {
                string email = HttpContext.User.Identity.Name;
                var Product = await _ProductRepo.GetProductByIdAsync(IDProduct);
                CartDetail item = new CartDetail
                {
                    Id = RandomId(),
                    IdProduct = IDProduct,
                    Number = number,
                    Price = Product.Price,
                };
                await _cartRepo.AddItemAsync(item, email);

                string referer = Request.Headers["Referer"].ToString();
                if (!string.IsNullOrEmpty(referer))
                {
                    return Redirect(referer);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpGet]
        public async Task<IActionResult> InfoOrder(string idItem, string IdProduct, int number)
        {
            try
            {
                List<string> idItemlist = new List<string>();
                List<string> idItemProductlist = new List<string>();
                string email = HttpContext.User.Identity.Name;
                //int number = 0;
                var product = await _ProductRepo.GetProductByIdAsync(IdProduct);

				var ListSelct = new List<SelctItemModel>();
                if (idItem == null)
                {
                    string random = RandomId();
                    idItemlist.Add(random);
                    CartDetail item = new CartDetail
                    {
                        Id = RandomId(),
                        IdProduct = IdProduct,
                        Number = number,
                        Price = product.Price,
                    };
                    await _cartRepo.AddItemAsync(item, email);
                }
                else
                {
                    idItemlist = new List<string>(idItem.Split(','));
                    idItemProductlist = new List<string>(IdProduct.Split(','));
                }
                foreach (var item in idItemlist)
                {
                    if (idItem == null)
                    {
                        var itemCart = await _cartRepo.ShowItemSelectAsync(email, IdProduct);
                        SelctItemModel model = new SelctItemModel
                        {
                            Id = IdProduct,
                            IdItem = itemCart.Id,
                            Number = number,
                            NameProduct = itemCart.Name,
                            Picture = itemCart.Picture,
                            Price = itemCart.Price,
                        };
                        ListSelct.Add(model);
                    }
                    else
                    {
                        var itemCart = await _cartRepo.ShowItemSelecByIDAsync(email, item);
                        SelctItemModel model = new SelctItemModel
                        {
                            Id = itemCart.IdProduct,
                            IdItem = itemCart.Id,
                            Number = itemCart.Number,
                            NameProduct = itemCart.Name,
                            Picture = itemCart.Picture,
                            Price = itemCart.Price,
                        };
                        ListSelct.Add(model);
                    }
                }
                InfoOrderModel infoOrder = new InfoOrderModel()
                {
                    selctItemModels = ListSelct,
                };
                return View(infoOrder);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCart(Dictionary<string, int> quantities)
        {
            string email = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(email);
            await _cartRepo.UpdateCartAsync(user.Id, quantities);

            return RedirectToAction(nameof(ShowCart));
        }

        [HttpPost]
        public async Task<IActionResult> Buy(string idItem, string IdProduct)
        {
            try
            {
                List<string> idItemlist = new List<string>();

                string email = HttpContext.User.Identity.Name;
                int number = 0;
                var ListSelct = new List<SelctItemModel>();
                if (idItem == null)
                {
                    string random = RandomId();
                    idItemlist.Add(random);
                    CartDetail itemCart = new CartDetail
                    {
                        Id = random,
                        IdProduct = IdProduct,
                        Number = 1,
                    };
                    await _cartRepo.AddItemAsync(itemCart, email);
                    number = 1;
                }
                else
                {
                    idItemlist = new List<string>(idItem.Split(','));
                }
                foreach (var item in idItemlist)
                {
                    if (number != 0)
                    {
                        var itemCart = await _cartRepo.ShowItemSelectAsync(email, IdProduct);
                        SelctItemModel model = new SelctItemModel
                        {
                            //Id = RandomId(),
                            IdItem = itemCart.Id,
                            Number = number,
                            NameProduct = itemCart.Name,
                            Picture = itemCart.Picture,
                            Price = itemCart.Price,
                        };
                        ListSelct.Add(model);
                    }
                    else
                    {
                        var itemCart = await _cartRepo.ShowItemSelecByIDAsync(email, item);
                        SelctItemModel model = new SelctItemModel
                        {
                            //Id = RandomId(),
                            IdItem = itemCart.Id,
                            Number = itemCart.Number,
                            NameProduct = itemCart.Name,
                            Picture = itemCart.Picture,
                            Price = itemCart.Price,
                        };
                        ListSelct.Add(model);
                    }
                }
                InfoOrderModel infoOrder = new InfoOrderModel()
                {
                    selctItemModels = ListSelct,
                };
                return View(infoOrder);
            }
            catch
            {
                return NotFound();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteItemCart([FromBody] string id)
        {
            await _cartRepo.RemoveItenAsync(id);
            return RedirectToAction(nameof(ShowCart));
        }


        public async Task<IActionResult> ChangeNumber(string idAndNumber)
        {
            string email = HttpContext.User.Identity.Name;
            string[] pairs = idAndNumber.Split(',');

            List<ItemModel> itemList = new List<ItemModel>();

            foreach (string pair in pairs)
            {
                string[] parts = pair.Split('.');

                if (parts.Length == 2)
                {
                    itemList.Add(new ItemModel { Id = parts[0], Number = parts[1] });
                }
            }


            foreach (ItemModel item in itemList)
            {
                await _cartRepo.ChangeNumberAsync(email, item.Id, int.Parse(item.Number));
            }
            return RedirectToAction(nameof(ShowCart));
        }
    }
    public class ItemModel
    {
        public string Id { get; set; }
        public string Number { get; set; }
    }
}
