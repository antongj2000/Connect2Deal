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
        private readonly ChatService _chatService;
        private readonly IWebHostEnvironment _environment;

        public ListingController(
            ListingService listingService,
            ChatService chatService,
            IWebHostEnvironment environment)
        {
            _listingService = listingService;
            _environment = environment;
            _chatService = chatService;
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



        #region Favorites
        [HttpPost]
        public async Task<IActionResult> Favorites(int listingId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            bool exists = await _listingService.FavoritesExists(userId, listingId);

            if (exists)
            {
                await _listingService.RemoveFavorite(userId, listingId);
            }
            else
            {

                await _listingService.CreateFavorite(userId, listingId);
            }

            return Json(new { isFavorited = !exists });
        }



        #endregion



        #region Close Transaction and sellect buyer

        [HttpPost]
        public async Task<IActionResult> MarkAsSold(int listingId, int buyerId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var listings = await _listingService.GetListingById(listingId);

            if (listings == null)
            {
                return NotFound();
            }

            if (listings.UserId != userId)
            {
                return Forbid();
            }

            await _listingService.CloseTransaction(listingId, buyerId, userId);

            return Json(new { success = true });
        }

        [HttpGet]   
        public async Task<IActionResult> SelectBuyer(int listingId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
            var conversations = await _chatService.GetConversations(userId);

            ViewBag.ListingId = listingId;
            ViewBag.SellerId = userId; 

            return PartialView("_SelectBuyer", conversations);
        }





        #endregion

    }
}
