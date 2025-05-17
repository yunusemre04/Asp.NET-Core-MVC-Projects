namespace WordGame.Models.ViewModels
{

    public class PuzzleGameViewModel
    {
        public int AttemptCount { get; set; } = 0;
        public List<PuzzleAttemptViewModel> Attempts { get; set; } = new();
        public bool IsSolved { get; set; }
        public bool IsGameOver => AttemptCount >= 6 || IsSolved;

        public string Message { get; set; } = "";
    }



}
