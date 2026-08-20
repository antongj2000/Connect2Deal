using Connect2Deal.Models;
using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Security.Claims;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Connect2Deal.Controllers
{
    public class HomeController : Controller
    {

        private readonly ListingService _listingService;

        public HomeController(ListingService listingService)
        {
            _listingService = listingService;
        }


        public async Task<IActionResult> Index(bool favorites = false, bool mylistings = false,
    string? search = null, int? countryId = null, int? cityId = null,
    int? categoryId = null, int? subcategoryId = null)
        {
            var model = new FilterListings();

            if (favorites && User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.Listings = await _listingService.GetUserFavorites(userId);
                ViewData["ActivePage"] = "Saved";
            }
            else if (mylistings && User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                model.Listings = await _listingService.GetMyListings(userId);
                ViewData["ActivePage"] = "MyListings";
            }
            else
            {
                model.Listings = await _listingService.GetFilteredListings(
                    search, countryId, cityId, categoryId, subcategoryId);

                model.Search = search;
                model.CountryId = countryId;
                model.CityId = cityId;
                model.CategoryId = categoryId;
                model.SubcategoryId = subcategoryId;

                var countries = await _listingService.CountryFetch();
                model.Countries = countries
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

                var categories = await _listingService.ParentCategoryFetch();
                model.Categories = categories
                    .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                    .ToList();

                if (countryId != null)
                {
                    var cities = await _listingService.CityFetch(countryId.Value);
                    model.Cities = cities
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                }

                if (categoryId != null)
                {
                    var subs = await _listingService.ChildCategoryFetch(categoryId.Value);
                    model.Subcategories = subs
                        .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                        .ToList();
                }

                ViewData["ActivePage"] = "Marketplace";
            }

            if (User.Identity.IsAuthenticated)
            {
                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["SavedIds"] = await _listingService.GetUserFavoriteIds(userId);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetCities(int countryId)
        {
            var cities = await _listingService.CityFetch(countryId);
            return Json(cities.Select(c => new { id = c.Id, name = c.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> GetSubcategories(int categoryId)
        {
            var subs = await _listingService.ChildCategoryFetch(categoryId);
            return Json(subs.Select(c => new { id = c.Id, name = c.Name }));
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
