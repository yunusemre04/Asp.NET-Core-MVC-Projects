namespace WordGame.Models.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required]
        public string? UserName { get; set; }

        [Required, EmailAddress]
        public string? Email { get; set; }

        [Required]
        public string? PasswordHash { get; set; }

        public int DailyWordLimit { get; set; } = 10; // varsayılan değer

        public ICollection<Word>? Words { get; set; }
    }


}
