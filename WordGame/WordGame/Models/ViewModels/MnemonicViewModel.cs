namespace WordGame.Models.ViewModels
{
    public class MnemonicViewModel
    {
        public int WordId { get; set; }
        public string? MnemonicNote { get; set; }
        public IFormFile? MnemonicImage { get; set; }
    }
}
