namespace WordGame.Models.ViewModels
{
    public class PuzzleItemViewModel
    {
        public int WordId { get; set; }
        public string ScrambledWord { get; set; } // karıştırılmış hali
        public string CorrectAnswer { get; set; } // kontrol için
    }

}
