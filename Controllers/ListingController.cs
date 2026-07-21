using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;

namespace Connect2Deal.Controllers
{

    [Authorize]
    public class ListingController : Controller
    {

        private readonly ListingService _listingService;
        private readonly IWebHostEnvironment _environment;

        public ListingController(
            ListingService listingService,
            IWebHostEnvironment environment)
        {
            _listingService = listingService;
            _environment = environment;
        }


        [HttpGet]
        public async Task<IActionResult> CreateListing()
        {
            var model = new Listing
            {
                ParentCategories = await BuildParentCategoryList(),
                Countries = await BuildParentLocationList()
            };

            return View(model);
        }




            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<IActionResult> CreateListing(Listing model, List<IFormFile> images)
            {
                if (!await _listingService.IsCategoryValid(model.ParentCategory, model.ChildCategory))
                    ModelState.AddModelError(nameof(model.ChildCategory), "Invalid sub-category.");

                if (!await _listingService.IsLocationValid(model.Country, model.City))
                    ModelState.AddModelError(nameof(model.City), "Invalid city.");

                if (!ModelState.IsValid)
                {
                    model.ParentCategories = await BuildParentCategoryList();
                    model.ChildCategories = await BuildChildCategoryList(model.ParentCategory);
                    model.Countries = await BuildParentLocationList();
                    model.Cities = await BuildChildLocationList(model.Country);
                    return View(model);
                }

                int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

                await _listingService.CreateListing(
                    userId,
                    model.ChildCategory,      
                    model.City,      
                    model.Title,
                    model.Description,
                    model.Price,
                    images, _environment.WebRootPath);
                

                return RedirectToAction("Index", "Home");
            }


        private async Task<List<SelectListItem>> BuildParentLocationList()
        {
            var countries = await _listingService.CountryFetch();
            return countries.Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name}).ToList();
        }

        private async Task<List<SelectListItem>> BuildChildLocationList(int parentId)
        {
            var cities = await _listingService.CityFetch(parentId);
            return cities
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }




        private async Task<List<SelectListItem>> BuildParentCategoryList()
        {
            var categories = await _listingService.ParentCategoryFetch();
            return categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }

        private async Task<List<SelectListItem>> BuildChildCategoryList(int parentId)
        {
            var categories = await _listingService.ChildCategoryFetch(parentId);
            return categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList();
        }


        [HttpGet]
        public async Task<IActionResult> SubCategories(int parentId)
        {
            var subs = await _listingService.ChildCategoryFetch(parentId);
            return Json(subs.Select(c => new { id = c.Id, name = c.Name }));
        }

        [HttpGet]
        public async Task<IActionResult> Cities(int parentId)
        {
            var cities = await _listingService.CityFetch(parentId);
            return Json(cities.Select(c => new { id = c.Id, name = c.Name }));
        }





        #region Getting listing by Id

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ListingDetails (int id)
        {
            var model =await _listingService.GetListingById(id);

            if (model == null)
            {
                return NotFound();
            }

            return PartialView("_PartialListingDetails", model);
        }



            #endregion



        }
}
