using AutoMapper;
using Project.Areas.Identity.Data;
using Project.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PayPal.Api;

namespace Project.Repository
{
    public class BillRepository : IBillRepository
    {
        public readonly DataContext _context;
        public readonly IMapper _mapper;
        public BillRepository(IMapper mapper, DataContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<Bill> CreateBillAsync(Bill model, List<BillItem> billItem)
        {
            await _context.Bills!.AddAsync(model);
            foreach(var item in billItem)
            {
                await _context.BillItems!.AddAsync(item);
            }
            await _context.SaveChangesAsync();
            return model;
        }

        public async Task<List<Bill>> GetAllBillByUserIdAsync(string Id)
        {
            var bill = await _context.Bills!.Where(x=> x.UserId == Id).Include(db => db.BillItems).OrderByDescending(x => x.BuyingDate).ToListAsync();
            return bill.ToList();
        }

        public async Task<List<Bill>> GetAllBill()
        {
            var ListItem = await _context.Bills!.Include(db => db.BillItems).Include(db => db.User).OrderByDescending(x => x.BuyingDate).ToListAsync();
            return ListItem;
        }
        public Bill GetBillById(string idBill)
        {
            var Item = _context.Bills!.Include(x => x.BillItems).FirstOrDefault(x => x.Id == idBill);
            return Item;
        }
        
        public async Task UpdateBillAsync(string id, int status)
        {
            var a = await _context.Bills!.FirstOrDefaultAsync(x => x.Id == id);
            a.Status = status;
            await _context.SaveChangesAsync();
        }
        
        public async Task UpdateStatusPaymentAsync(string id, int status)
        {
            var a = await _context.Bills!.FirstOrDefaultAsync(x => x.Id == id);
            a.StatusPayment = status;
            await _context.SaveChangesAsync();
        }
    }
}
