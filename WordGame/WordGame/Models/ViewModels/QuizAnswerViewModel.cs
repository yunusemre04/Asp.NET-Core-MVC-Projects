namespace WordGame.Models.ViewModels
{
    public class QuizAnswerItem
    {
        public int WordId { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuizSubmitViewModel
    {
        public List<QuizAnswerItem> Answers { get; set; } = new();
    }

}
