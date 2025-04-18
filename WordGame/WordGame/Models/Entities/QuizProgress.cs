using System.ComponentModel.DataAnnotations;

namespace WordGame.Models.Entities
{
    public class QuizProgress
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int WordId { get; set; }

        public int CorrectCount { get; set; } // kaç adımı tamamladı

        public DateTime LastAnsweredDate { get; set; } // son bilindiği tarih
        public bool IsCompleted { get; set; } // 6 adımı tamamladı mı?
    }
}
