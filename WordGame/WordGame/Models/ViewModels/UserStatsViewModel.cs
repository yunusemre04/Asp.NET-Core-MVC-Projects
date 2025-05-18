namespace WordGame.Models.ViewModels
{
    public class UserStatsViewModel
    {
        public int TotalWorkedWords { get; set; }
        public int TotalCompletedWords { get; set; }
        public int TotalWrongAttempts { get; set; }
        public double SuccessRate { get; set; }

        public List<string> HardestWords { get; set; } = new();
        public List<WordProgressViewModel> WordProgresses { get; set; } = new();
    }

    public class WordProgressViewModel
    {
        public string? WordName { get; set; }
        public int CorrectCount { get; set; } // 0–6 arası
    }

}
