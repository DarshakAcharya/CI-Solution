using System.ComponentModel.DataAnnotations;

namespace CI_Platform.Models.ViewModels
{
    public class ForgotPasswordVM
    {
        [Required(ErrorMessage ="please enter email address")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; } = null!;
    }
}
