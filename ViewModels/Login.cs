using System.ComponentModel.DataAnnotations;

namespace Connect2Deal.ViewModels
{


    public class Login
    {

        [Required]
        public string Username { get; set; } = string.Empty;



        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;




    }
}
