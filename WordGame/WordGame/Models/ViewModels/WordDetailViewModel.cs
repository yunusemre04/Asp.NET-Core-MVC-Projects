namespace WordGame.Models.ViewModels
{
    public class WordDetailViewModel
    {
        public string? EngWordName { get; set; }
        public string? TurWordName { get; set; }
        public string? Picture { get; set; }

        public string? MnemonicNote { get; set; }
        public string? MnemonicImagePath { get; set; }

        public List<string> SampleSentences { get; set; } = new();
    }

}
