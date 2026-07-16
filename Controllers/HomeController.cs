using Connect2Deal.Models;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Connect2Deal.Controllers
{
    public class HomeController : Controller
    {

        private readonly ListingService _listingService;

        public HomeController(ListingService listingService)
        {
            _listingService = listingService;
        }

        public async Task<IActionResult> Index()
        {
            var model = await _listingService.GetAllListings();
            return View(model);
        }

        [Authorize]
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
