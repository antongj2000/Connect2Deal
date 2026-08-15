using Connect2Deal.Data;
using Connect2Deal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;


namespace Connect2Deal.Services
{
    public class UserRatingService
    {


        private readonly AppDbContext mycontext;

        public UserRatingService(AppDbContext _mycontext)
        {
            mycontext = _mycontext;
        }

        public async Task<bool> HasAlreadyRated(int transactionId, int rateId) 
        {
            return await mycontext.UserRatings.AnyAsync(x=>x.TransactionId==transactionId && x.RaterId == rateId);
        }


        public async Task<Transaction?> GetTransactionForRating(int transactionId, int buyerId)
        {
              return await mycontext.Transactions
            .Include(x => x.Seller)
            .Include(x => x.Listing)
            .Include(x => x.Buyer)
            .FirstOrDefaultAsync(x => x.Id == transactionId && x.BuyerId == buyerId);
        }


        public async Task<UserRating> CreateRating (int transactionId, int raterId, int ratedUserId, int score, string? comment)
        {
            var newRating = new UserRating
            {
                TransactionId = transactionId,
                RaterId = raterId,
                RatedUserId = ratedUserId,
                Score = score,
                Comment = comment,
                CreatedAt = DateTime.UtcNow
            };

            mycontext.UserRatings.Add(newRating);
            await mycontext.SaveChangesAsync();


            return newRating;
        }
        

        

    }
}
