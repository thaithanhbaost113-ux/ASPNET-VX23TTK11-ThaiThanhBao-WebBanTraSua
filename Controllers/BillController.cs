using Project.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using Project.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using PayPal.Api;
using Project.Others;
using ServiceStack.Text;

namespace Project.Controllers
{
    public class BillController : Controller
    {
        public readonly IBillRepository _BillRepo;
        public readonly ICartRepository _CartRepo;
        public readonly IProductRepository _ProductRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;


        private readonly Random _random = new Random();
        private UserManager<ApplicationUser> _userManager;
        private Payment payment;
        
        public BillController(IHttpContextAccessor httpContextAccessor, IBillRepository _billRepo, ICartRepository _cartRepo, UserManager<ApplicationUser> userManager, IProductRepository ProductRepo)
        {
            _BillRepo = _billRepo;
            _CartRepo = _cartRepo;
            _userManager = userManager;
            _ProductRepo = ProductRepo;
            _httpContextAccessor = httpContextAccessor;
        }
        private Payment ExecutePayment(APIContext apiContext, string payerId, string paymentId)
        {
            var paymentExecution = new PaymentExecution()
            {
                payer_id = payerId
            };
            this.payment = new Payment()
            {
                id = paymentId
            };
            return this.payment.Execute(apiContext, paymentExecution);
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
        public async Task<IActionResult> ShowAllBill()
        {
            string email = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(email);

            return View(await _BillRepo.GetAllBillByUserIdAsync(user.Id));
        }
        
        [HttpGet]
        public async Task<IActionResult> ChangeStatusBill(string id, int status)
        {
            string email = HttpContext.User.Identity.Name;
            await _BillRepo.UpdateBillAsync(id, status);
            return RedirectToAction(nameof(ShowAllBill));
        }

        public ActionResult FailureView()
        {
            return View();
        }
        public ActionResult SuccessView()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBill(string name, int total, string address, string note, string phone, string paymentMethods, string IdCartItem)
        {

            string email = HttpContext.User.Identity.Name;
            var user = await _userManager.FindByNameAsync(email);

            string idBill = RandomId();
            List<string> idItemlist = new List<string>(IdCartItem.Split(','));
            Bill bill = new Bill
            {
                Id = idBill,
                UserId = user.Id,
                BuyingDate = DateTime.Now,
                Total = total,
                Status = 0,
                Phone = phone,
                Note = note,
                Name = name,
                Address = address,
                StatusPayment = 3,
            };
            List<BillItem> BillItems = new List<BillItem>();
            var cartItem = await _CartRepo.ShowCartAsync(email);
            foreach (var item in cartItem)
            {
                BillItem Items = new BillItem
                {
                    Id = RandomId(),
                    IdBill = idBill,
                    Name = item.IdProductNavigation.NameProduct,
                    Number = item.Number,
                    Price = item.Price,
                    UrlImage = item.IdProductNavigation.Images.FirstOrDefault(x => x.IsMain == true)?.UrlImage,
                };
                BillItems.Add(Items);
            }
            await _BillRepo.CreateBillAsync(bill, BillItems);
            foreach (var idItem in idItemlist)
            {
                var a = _CartRepo.UpdateCartItemAsync(email, idItem);
            }
            if (paymentMethods != "cash")
            {
                return RedirectToAction("PaymentWithPaypal", new { idBill = idBill });

            }
            await _BillRepo.UpdateStatusPaymentAsync(idBill, 0);
            return View(bill);
        }
        [HttpGet]
        public async Task<IActionResult> PaymentWithPaypal(string idBill,  string Cancel = null)
        {
            APIContext apiContext = PaypalConfiguration.GetAPIContext();

            try
            {
                string payerId = HttpContext.Request.Query["PayerID"];

                if (string.IsNullOrEmpty(payerId))
                {
                    var bill = _BillRepo.GetBillById(idBill);
                    string baseURI = $"{Request.Scheme}://{Request.Host}/Bill/PaymentWithPaypal?";
                    var guid = Guid.NewGuid().ToString();
                    var createdPayment = CreatePayment(apiContext, $"{baseURI}guid={guid}&idBill={idBill}", bill.Total);
                    var paypalRedirectUrl = createdPayment.links
                        .FirstOrDefault(link => string.Equals(link.rel, "approval_url", StringComparison.OrdinalIgnoreCase))?.href;
                    HttpContext.Session.SetString(guid, createdPayment.id);
                    return Redirect(paypalRedirectUrl);
                }
                else
                {
                    // Thực hiện thanh toán sau khi nhận `PayerID`
                    string guid = HttpContext.Request.Query["guid"];
                    string paymentId = HttpContext.Session.GetString(guid);
                    var executedPayment = ExecutePayment(apiContext, payerId, paymentId);
                    if (executedPayment.state.ToLower() != "approved")
                    {
                        await _BillRepo.UpdateStatusPaymentAsync(idBill, 2);
                        return View("FailureView");
                    }
                    await _BillRepo.UpdateStatusPaymentAsync(idBill, 1);
                    await _BillRepo.UpdateBillAsync(idBill, 1);
                }
            }
            catch (Exception ex)
            {
                await _BillRepo.UpdateStatusPaymentAsync(idBill, 2);
                return View("FailureView");
            }

            return View("SuccessView");
        }

        private Payment CreatePayment(APIContext apiContext, string redirectUrl, decimal amountVND)
        {
            decimal exchangeRate = 23000m;
            decimal amountUSD = Math.Round(amountVND / exchangeRate, 2); 
            var itemList = new ItemList()
            {
                items = new List<Item>()
            };

            itemList.items.Add(new Item()
            {
                name = "Invoice Payment",
                currency = "USD",
                price = amountUSD.ToString("F2"), 
                quantity = "1",
                sku = "sku"
            });

            var payer = new Payer()
            {
                payment_method = "paypal"
            };

            var redirUrls = new RedirectUrls()
            {
                cancel_url = redirectUrl + "&Cancel=true",
                return_url = redirectUrl
            };

            var details = new Details()
            {
                subtotal = amountUSD.ToString("F2")
            };

            var amount = new Amount()
            {
                currency = "USD",
                total = amountUSD.ToString("F2"), 
                details = details
            };

            var transactionList = new List<Transaction>();
            var paypalOrderId = DateTime.Now.Ticks;
            transactionList.Add(new Transaction()
            {
                description = $"Invoice #{paypalOrderId}",
                invoice_number = paypalOrderId.ToString(),
                amount = amount,
                item_list = itemList
            });

            this.payment = new Payment()
            {
                intent = "sale",
                payer = payer,
                transactions = transactionList,
                redirect_urls = redirUrls
            };

            return this.payment.Create(apiContext);
        }

    }
}
