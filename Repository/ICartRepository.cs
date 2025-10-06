using Project.Areas.Identity.Data;
using Project.Models;
using Microsoft.AspNetCore.Mvc;

namespace Project.Repository
{
    public interface ICartRepository
    {
        public Task<List<CartDetail>> ShowCartAsync(string email);
        public Task<CartItemModel> ShowItemSelectAsync(string email, string idItem);
        public Task UpdateCartItemAsync(string email, string Id);
        //public Task<List<SelctItemModel>> SelectItem(SelctItemModel model);
        Task UpdateCartAsync(string userId, Dictionary<string, int> quantities);
        public Task newCartAsync(string id, string email);
        public Task ChangeNumberAsync(string email, string Id, int number);
        public Task AddItemAsync(CartDetail model, string email);
        public Task RemoveItenAsync(string IdItem);
        public Task<CartItemModel> ShowItemSelecByIDAsync(string email, string idItem);
    }
}
