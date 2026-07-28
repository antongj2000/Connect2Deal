using Connect2Deal.Models;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Connect2Deal.Controllers
{
    public class HomeController : Controller
    {

        private readonly ListingService _listingService;

        public HomeController(ListingService listingService)
        {
            _listingService = listingService;
        }
        public async Task<IActionResult> Index(bool favorites = false)
        {
            List<Listing> model;

            if (favorites && User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                model = await _listingService.GetUserFavorites(userId);
                ViewData["ActivePage"] = "Saved";
            }
            else
            {
                model = await _listingService.GetAllListings();
                ViewData["ActivePage"] = "Marketplace";
            }

            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["SavedIds"] = await _listingService.GetUserFavoriteIds(userId);
            }

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
