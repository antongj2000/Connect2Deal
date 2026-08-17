using Connect2Deal.Services;
using Connect2Deal.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Connect2Deal.ViewComponents
{
    public class UnreadBadgesViewComponent : ViewComponent
    {
        private readonly ChatService _chatService;
        private readonly NotificationService _notificationService;

        public UnreadBadgesViewComponent(ChatService chatService, NotificationService notificationService)
        {
            _chatService = chatService;
            _notificationService = notificationService;
        }

        public async Task<IViewComponentResult> InvokeAsync(string kind)
        {
            if (User.Identity == null || !User.Identity.IsAuthenticated)
            {
                return View(0);
            }

            int userId = int.Parse(((ClaimsPrincipal)User).FindFirstValue(ClaimTypes.NameIdentifier));

            int count = kind == "messages"
                ? await _chatService.CountAllUnreadMessages(userId)
                : await _notificationService.CountAllUnreadNotifications(userId);
            ViewData["Kind"] = kind;
            return View(count);
        }

    }
}