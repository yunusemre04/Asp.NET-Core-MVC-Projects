namespace WordGame.Models.Entities
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class WordSample
    {
        [Key]
        public int WordSampleId { get; set; }

        [Required]
        public string? SampleSentence { get; set; }

        // Foreign Key
        [ForeignKey("Word")]
        public int WordId { get; set; }

        public Word? Word { get; set; }
    }

}
