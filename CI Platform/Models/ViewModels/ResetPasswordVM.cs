using System.ComponentModel.DataAnnotations;

namespace CI_Platform.Models.ViewModels
{
    public class ResetPasswordVM
    {
        [Required(ErrorMessage = "Please Enter Email")]
        public string? Email { get; set; }

       /* public string Token { get; set; } = null!;  */

        public DateTime CreatedAt { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = null!;

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = null!;

        [Required]
        public string Token { get; set; }
    }
}
