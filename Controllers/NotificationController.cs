using Connect2Deal.Data;
using Connect2Deal.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Connect2Deal.Controllers
{
    public class NotificationController : Controller
    {



        private readonly NotificationService mynotification;

        public NotificationController(NotificationService _mynotification)
        {
            mynotification = _mynotification;
        }



        
        [Authorize]
        public async Task<IActionResult> Notification()
        {
            int userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));

            var notifications =  await mynotification.GetAllNotifications(userId);

            return View(notifications);
        }


        
        





    }
}
