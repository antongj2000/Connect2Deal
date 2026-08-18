using Connect2Deal.Data;
using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Connect2Deal.Controllers
{
    [Authorize]
    public class UserRatingController : Controller
    {

        private readonly UserRatingService _userRatingService;
        private readonly UserService _userService;
        private readonly NotificationService _notificationService;

        public UserRatingController(UserRatingService userRatingService, NotificationService notificationService, UserService userService)
        {
            _userRatingService = userRatingService;
            _notificationService = notificationService;
            _userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> RateSeller(int transactionId)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var check = await _userRatingService.HasAlreadyRated(transactionId, userId);
            if (check)
            {
                return RedirectToAction("Notification", "Notification");
            }

            var transaction = await _userRatingService.GetTransactionForRating(transactionId, userId);
            if (transaction == null)
            {
                return NotFound();
            }

            return PartialView("_RateSeller", transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitRating(int transactionId, int score, string? comment)
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            if (score < 1 || score > 5)
            {
                return BadRequest();
            }

            var transaction = await _userRatingService.GetTransactionForRating(transactionId, userId);

            if (transaction == null)
            {
                return NotFound();
            }

            var check = await _userRatingService.HasAlreadyRated(transactionId, userId);
            if (check)
            {
                return RedirectToAction("Notification", "Notification");
            }

            var rating = await _userRatingService.CreateRating(transactionId, userId, transaction.SellerId, score, comment);

            await _notificationService.MarkNotificationAsRead(transactionId, userId);

            await _notificationService.CreateRatingReceivedNotification(
                        transaction.SellerId,
                        rating.Id,  
                        transaction.Buyer.Username,
                        score,
                        transaction.Listing.Title
                );

            return RedirectToAction("Notification", "Notification");
        }



        #region Show user details and ratings


        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> SellerProfile(int sellerId)
        {
            var seller = await _userService.getUserById(sellerId);

            if (seller == null)
            {
                return NotFound();
            }

            var ratings = await _userService.GetRatingsForUser(sellerId);

            var model = new SellerProfileView
            {
                SellerId = seller.Id,
                Username = seller.Username,
                FirstName = seller.FirstName,
                LastName = seller.LastName,
                PhoneNumber = seller.PhoneNumber,
                Description = seller.Description,
                ProfileImage = seller.ProfileImage,
                MemberSince = seller.CreatedAt,
                Ratings = ratings,
                RatingCount = ratings.Count,
                AverageScore = ratings.Count > 0 ? ratings.Average(r => r.Score) : 0
            };

            return PartialView("_SellerProfile", model);
        }

        #endregion



    }
}
