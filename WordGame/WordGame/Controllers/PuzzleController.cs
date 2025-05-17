using Microsoft.AspNetCore.Mvc;
using WordGame.Data;
using WordGame.Extensations;
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

            if (HttpContext.Session.GetString("PuzzleWord") == null)
            {
                var word = _context.QuizProgresses
                    .Where(p => p.UserId == userId && p.IsCompleted)
                    .Join(_context.Words, p => p.WordId, w => w.WordId, (p, w) => w)
                    .Where(w => w.EngWordName.Length >= 3 && w.EngWordName.Length <= 8)
                    .OrderBy(x => Guid.NewGuid())
                    .Select(w => w.EngWordName.ToUpper())
                    .FirstOrDefault();

                if (word == null)
                {
                    TempData["Error"] = "Uygun uzunlukta öğrenilmiş bir kelime bulunamadı.";
                    return RedirectToAction("Index", "Home");
                }

                HttpContext.Session.SetString("PuzzleWord", word);
            }

            return View(new PuzzleGameViewModel());
        }

        [HttpPost]
        public IActionResult Submit(string guess, PuzzleGameViewModel model)
        {
            var answer = HttpContext.Session.GetString("PuzzleWord");
            if (answer == null)
                return RedirectToAction("Index");

            guess = guess?.Trim().ToUpper() ?? "";

            if (guess.Length != answer.Length)
            {
                model.Message = $"Lütfen {answer.Length} harfli bir kelime girin.";
                return View("Index", LoadModelFromSession(model)); // geçmişi geri yükle
            }

            var feedback = new List<PuzzleLetterFeedback>();
            for (int i = 0; i < answer.Length; i++)
            {
                var letter = guess[i];
                string status = "absent";

                if (letter == answer[i])
                    status = "correct";
                else if (answer.Contains(letter))
                    status = "present";

                feedback.Add(new PuzzleLetterFeedback { Letter = letter, Status = status });
            }

            var attempts = HttpContext.Session.GetObjectFromJson<List<PuzzleAttemptViewModel>>("PuzzleAttempts") ?? new List<PuzzleAttemptViewModel>();
            attempts.Add(new PuzzleAttemptViewModel { Feedback = feedback });

            HttpContext.Session.SetObjectAsJson("PuzzleAttempts", attempts);

            model = LoadModelFromSession(model);
            model.AttemptCount = attempts.Count;
            model.Attempts = attempts;

            if (guess == answer)
            {
                model.IsSolved = true;
                model.Message = "Tebrikler! 🎉 Doğru tahmin ettiniz.";
            }
            else if (model.AttemptCount >= 6)
            {
                model.Message = $"Hakkınız doldu. Doğru kelime: {answer}";
            }

            return View("Index", model);
        }



        public IActionResult Restart()
        {
            HttpContext.Session.Remove("PuzzleWord");
            HttpContext.Session.Remove("PuzzleAttempts");
            return RedirectToAction("Index");
        }


        private PuzzleGameViewModel LoadModelFromSession(PuzzleGameViewModel model)
        {
           
            model.Attempts = HttpContext.Session.GetObjectFromJson<List<PuzzleAttemptViewModel>>("PuzzleAttempts") ?? new List<PuzzleAttemptViewModel>();
            model.AttemptCount = model.Attempts.Count;
            return model;
        }
    }

    


}