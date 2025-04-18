namespace WordGame.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Word
    {
        [Key]
        public int WordId { get; set; }

        [Required]
        public string? EngWordName { get; set; }

        [Required]
        public string? TurWordName { get; set; }

        public string? Picture { get; set; }

        public ICollection<WordSample>? WordSamples { get; set; }
    }

}
