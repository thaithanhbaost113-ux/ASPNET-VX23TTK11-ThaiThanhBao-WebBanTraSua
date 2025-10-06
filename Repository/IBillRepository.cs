using Project.Areas.Identity.Data;
using Project.Models;

namespace Project.Repository
{
    public interface IBillRepository
    {
        public Task<Bill> CreateBillAsync(Bill model, List<BillItem> billItem);
        public Task UpdateBillAsync(string id, int status);
        public Task UpdateStatusPaymentAsync(string id, int status);
        public Task<List<Bill>> GetAllBillByUserIdAsync(string email);
        public Task<List<Bill>> GetAllBill();
        public Bill GetBillById(string idBill);
    }
}
