namespace WordGame.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class UserSettingsViewModel
    {
        [Required]
        [Range(1, 100, ErrorMessage = "1 ile 100 arasında bir sayı giriniz.")]
        public int DailyWordLimit { get; set; }
    }
}
