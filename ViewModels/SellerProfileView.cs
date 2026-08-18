using Connect2Deal.Models;

namespace Connect2Deal.ViewModels
{
    public class SellerProfileView
    {
        public int SellerId { get; set; }
        public string Username { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? PhoneNumber { get; set; }
        public string? Description { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime MemberSince { get; set; }

        public List<UserRating> Ratings { get; set; } = new();
        public int RatingCount { get; set; }
        public double AverageScore { get; set; }
    }
}