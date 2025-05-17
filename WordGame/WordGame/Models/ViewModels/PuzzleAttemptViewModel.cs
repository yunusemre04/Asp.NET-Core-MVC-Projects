namespace WordGame.Models.ViewModels
{
    public class PuzzleLetterFeedback
    {
        public char Letter { get; set; }
        public string Status { get; set; } // "correct", "present", "absent"
    }

    public class PuzzleAttemptViewModel
    {
        public List<PuzzleLetterFeedback> Feedback { get; set; } = new();
    }


}
