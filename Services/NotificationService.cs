using Connect2Deal.Constants;
using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Connect2Deal.Services



{
    public class NotificationService
    {

        private readonly AppDbContext mycontext;

        public NotificationService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }



        public async Task<List<Notification>> GetAllNotifications(int userId)
        {

            var notifications = await mycontext.Notifications.Where(x=>x.UserId==userId).
                OrderByDescending(x => x.CreatedAt).ToListAsync();

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
        }



        #endregion



    }
}
