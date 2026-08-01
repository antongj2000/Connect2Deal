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

        


      
    }
}
