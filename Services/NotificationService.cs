using Connect2Deal.Constants;
using Connect2Deal.Data;
using Connect2Deal.Hubs;
using Connect2Deal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Services



{
    public class NotificationService
    {

        private readonly AppDbContext mycontext;
        private readonly IHubContext<ChatHub> _hub;

        public NotificationService(AppDbContext _mycontext, IHubContext<ChatHub> hub)
        {
            mycontext = _mycontext;
            _hub = hub;
        }



        public async Task<List<Notification>> GetAllNotifications(int userId)
        {

            var notifications = await mycontext.Notifications.Where(x=>x.UserId==userId).
                OrderByDescending(x => x.CreatedAt).AsNoTracking().ToListAsync();

            return notifications;
        }


        public async Task MarkNotificationAsRead(int transactionId, int userId)
        {
            var notification = await mycontext.Notifications.FirstOrDefaultAsync(
                x => x.UserId == userId
                  && x.Type == NotificationTypes.RateSeller
                  && x.RelatedId == transactionId);

            if (notification != null)
            {
                notification.IsRead = true;
                await mycontext.SaveChangesAsync();
            }
        }



        #region Notification for seller

        public async Task CreateRatingReceivedNotification(int sellerId, int ratingId, string raterUsername, int score, string listingTitle)
        {
            var notification = new Notification
            {
                UserId = sellerId,
                Type = NotificationTypes.RatingReceived,
                Message = $"{raterUsername} rated you {score} stars for \"{listingTitle}\".",
                RelatedId = ratingId,
                CreatedAt = DateTime.UtcNow,
                IsRead = false
            };

            mycontext.Notifications.Add(notification);
            await mycontext.SaveChangesAsync();

            await _hub.Clients.Group($"user-{sellerId}").SendAsync("NotificationUpdate", new
            {
                message = notification.Message
            });
        }



        #endregion


        #region Count all unreaded notifications

        public async Task<int> CountAllUnreadNotifications(int userId)
        {
            int allNotifications = await mycontext.Notifications.CountAsync(x=>x.IsRead != true && x.UserId == userId);
            return allNotifications;
        }

        #endregion

        #region Mark notifications as read

        public async Task MarkAllAsRead(int userId)
        {
            var readNotifications = await mycontext.Notifications.Where(x=>x.UserId == userId && !x.IsRead && x.Type!=NotificationTypes.RateSeller).ToListAsync();

            foreach (var read in readNotifications)
            {
                read.IsRead = true;
            }

            await mycontext.SaveChangesAsync();
        }

        #endregion



        #region Admin warrning

        public async Task CreateAdminWarning(int userId, int listingId, string message)
        {
            var notification = new Notification
            {
                UserId = userId,
                Type = NotificationTypes.AdminWarning,
                Message = message,
                RelatedId = listingId
            };

            mycontext.Notifications.Add(notification);
            await mycontext.SaveChangesAsync();

            await _hub.Clients.Group($"user-{userId}").SendAsync("NotificationUpdate");
        }

        #endregion
    }
}
