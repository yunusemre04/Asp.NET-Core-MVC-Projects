namespace WordGame.Models.ViewModels
{
    using System.ComponentModel.DataAnnotations;

    public class AddWordViewModel
    {
        [Required]
        public string? EngWordName { get; set; }

        [Required]
        public string? TurWordName { get; set; }

        public IFormFile? PictureFile { get; set; }

        public List<string> SampleSentences { get; set; } = new();
    }

}
