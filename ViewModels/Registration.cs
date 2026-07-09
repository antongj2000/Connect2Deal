using System.ComponentModel.DataAnnotations;



namespace Connect2Deal.ViewModels
{
    public class Registration
    {

        [Required]
        [Display(Name = "First name")]
        [StringLength(30, MinimumLength = 2)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Last name")]
        [StringLength(30, MinimumLength = 2)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [StringLength(20, MinimumLength = 5)]
        public string Username { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [RegularExpression(@"^.{3,}@.+$", ErrorMessage = "Need at least 3 characters before the @.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;


    }
}


