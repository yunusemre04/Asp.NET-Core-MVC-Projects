namespace WordGame.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Mevcut şifre zorunludur")]
        public required string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre zorunludur")]
        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır")]
        public required string NewPassword { get; set; }

        [Required(ErrorMessage = "Yeni şifre tekrarı zorunludur")]
        [Compare("NewPassword", ErrorMessage = "Şifreler uyuşmuyor")]
        public required string ConfirmPassword { get; set; }
    }

}
