using System.ComponentModel.DataAnnotations;

namespace WordGame.Models.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-posta adresi zorunludur")]
        [EmailAddress]
        public string? Email { get; set; }
    }

}
