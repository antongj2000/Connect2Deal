using Microsoft.AspNetCore.Mvc.Rendering;
using Connect2Deal.Models;

namespace Connect2Deal.ViewModels
{
    public class FilterListings
    {
        public List<Connect2Deal.Models.Listing> Listings { get; set; } = new();

        // izabrane vrijednosti (vraćaju se u polja poslije pretrage)
        public string? Search { get; set; }
        public int? CountryId { get; set; }
        public int? CityId { get; set; }
        public int? CategoryId { get; set; }
        public int? SubcategoryId { get; set; }

        // liste za dropdown-ove
        public List<SelectListItem> Countries { get; set; } = new();
        public List<SelectListItem> Cities { get; set; } = new();
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> Subcategories { get; set; } = new();
    }
}