namespace WordGame.Models.ViewModels
{
    public class UserStatsViewModel
    {
        public int TotalWorkedWords { get; set; }
        public int TotalCompletedWords { get; set; }
        public int TotalWrongAttempts { get; set; }
        public double SuccessRate { get; set; }

        public List<string> HardestWords { get; set; } = new();
    }

}
