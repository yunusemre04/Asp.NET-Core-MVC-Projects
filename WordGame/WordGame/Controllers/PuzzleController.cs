using Microsoft.AspNetCore.Mvc;
using WordGame.Data;
using WordGame.Models.ViewModels;

namespace WordGame.Controllers
{
    public class PuzzleController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PuzzleController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var kelimeler = _context.Words
                .OrderBy(r => Guid.NewGuid()) // rastgele sırala
                .Take(5)
                .ToList();

            var puzzleList = kelimeler.Select(k => new PuzzleItemViewModel
            {
                WordId = k.WordId,
                CorrectAnswer = k.EngWordName,
                ScrambledWord = Shuffle(k.EngWordName)
            }).ToList();

            return View(puzzleList);
        }

        private string Shuffle(string input)
        {
            var rng = new Random();
            return new string(input.OrderBy(c => rng.Next()).ToArray());
        }

        [HttpPost]
        public IActionResult Submit(List<string> userAnswers, List<string> correctAnswers)
        {
            int correctCount = 0;

            for (int i = 0; i < correctAnswers.Count; i++)
            {
                if (userAnswers[i].Trim().ToLower() == correctAnswers[i].ToLower())
                    correctCount++;
            }

            ViewBag.Total = correctAnswers.Count;
            ViewBag.Correct = correctCount;

            return View("Result");
        }

    }

}
