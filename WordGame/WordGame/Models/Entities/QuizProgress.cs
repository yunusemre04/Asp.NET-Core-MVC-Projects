using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WordGame.Models.Entities
{
    public class QuizProgress
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int WordId { get; set; }

        public int CorrectCount { get; set; } 

        public DateTime LastAnsweredDate { get; set; }
        public bool IsCompleted { get; set; }

        [ForeignKey("WordId")]
        public Word? Word { get; set; }
    }
}
