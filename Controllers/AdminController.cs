using Connect2Deal.Data;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ListingService _listingService;
        private readonly NotificationService _notificationService;

        public AdminController(AppDbContext context, ListingService listingService, NotificationService notificationService)
        {
            _context = context;
            _listingService = listingService;
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Reports()
        {
            ViewData["ActivePage"] = "AdminReports";

            var reports = await _context.Reports
                .Include(r => r.Listing)
                .Include(r => r.Reporter)
                .OrderByDescending(r => r.CreatedAt)
                .ToListAsync();

            return View(reports);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DismissReport(int reportId)
        {
            var report = await _context.Reports.FindAsync(reportId);
            if (report == null) return NotFound();

            _context.Reports.Remove(report);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            ViewData["ActivePage"] = "AdminUsers";

            var users = await _context.Users
                .OrderByDescending(u => u.CreatedAt)
                .ToListAsync();

            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleUserBlock(int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return NotFound();

            if (user.Role == "Admin")
            {
                return BadRequest(new { success = false, message = "Cannot block an admin account." });
            }

            user.IsBlocked = !user.IsBlocked;
            await _context.SaveChangesAsync();

            return Json(new { success = true, isBlocked = user.IsBlocked });
        }

        [HttpGet]
        public async Task<IActionResult> WarnForm(int listingId)
        {
            var listing = await _listingService.GetListingById(listingId);
            if (listing == null) return NotFound();

            return PartialView("_WarnUser", listing);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateWithWarning(int listingId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
            {
                return BadRequest();
            }

            var listing = await _context.Listings.FindAsync(listingId);
            if (listing == null) return NotFound();

            listing.Status = "Inactive";
            await _context.SaveChangesAsync();

            await _notificationService.CreateAdminWarning(listing.UserId, listingId, message.Trim());

            return Json(new { success = true });
        }
    }
}