    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.ComponentModel.DataAnnotations;

    namespace Connect2Deal.ViewModels
    {
        public class Listing
        {


            [Required]
            [StringLength(30, MinimumLength = 2)]
            public string Title { get; set; } = string.Empty;


            [Required]
            [StringLength(30, MinimumLength = 2)]
            public string Description { get; set; } = string.Empty;

            [Required]
            [Range(0.01, 9999999.99, ErrorMessage = "Enter a valid price.")]
            public decimal Price { get; set; }


            [Required]
            [Display(Name = "Category")]
            [Range(1, int.MaxValue, ErrorMessage = "Please choose a category.")]
            public int ParentCategory { get; set; }

            [Required]
            [Range(1, int.MaxValue, ErrorMessage = "Please choose a sub-category.")]
            [StringLength(30, MinimumLength = 2)]
            public int ChildCategory { get; set; }


            public List<SelectListItem> ParentCategories { get; set; } = new List<SelectListItem>();
            public List<SelectListItem> ChildCategories { get; set; } = new List<SelectListItem>();
        }
    }
