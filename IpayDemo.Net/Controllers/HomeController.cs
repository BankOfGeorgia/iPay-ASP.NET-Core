using IpayDemo.Net.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace IpayDemo.Net.Controllers
{
    public class HomeController : Controller
    {
        private readonly IIpayService ipayService;

        public HomeController(IIpayService ipayService)
        {
            this.ipayService = ipayService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<IActionResult> InitiatePayment()
        {
            var order = await ipayService.MakeOrder(Intent.Capture, new List<Item>
            {
                new Item
                {
                    Amount = 100,
                    Description = "Item 1",
                    ProductID = 11111,
                    Quantity = 1
                },
                new Item
                {
                    Amount = 50,
                    Description = "Item 2",
                    ProductID = 11112,
                    Quantity = 1
                }
            }, "https://demo.ipay.ge", "ka", "123465");

            var redirect = order.Links.Single(link => link.Method == Method.Redirect);

            return Redirect(redirect.Href);
        }

        [HttpPost]
        public async Task<IActionResult> InitiateInstallment()
        {
            var order = await ipayService.MakeOrder(Intent.Loan, new List<Item>
            {
                new Item
                {
                    Amount = 100,
                    Description = "Item 1",
                    ProductID = 11111,
                    Quantity = 1
                },
                new Item
                {
                    Amount = 50,
                    Description = "Item 2",
                    ProductID = 11112,
                    Quantity = 1
                }
            }, "https://demo.ipay.ge", "ka", "123465");

            var redirect = order.Links.Single(link => link.Method == Method.Redirect);

            return Redirect(redirect.Href);
        }
    }
}
