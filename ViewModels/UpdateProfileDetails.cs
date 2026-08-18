using System.ComponentModel.DataAnnotations;

namespace Connect2Deal.ViewModels
{
    public class UpdateProfileDetails
    {

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50)]
        public string FirstName { get; set; } = "";

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50)]
        public string LastName { get; set; } = "";

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        [StringLength(20)]
        public string? PhoneNumber { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

    }
}
