using Microsoft.AspNetCore.Mvc;
using WordGame.Models;
using WordGame.Data;
namespace WordGame.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    
    using global::WordGame.Models.Entities;
    using global::WordGame.Models.ViewModels;

    public class WordController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public WordController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        //Returns Add Screen
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //Saving word
        [HttpPost]
        public async Task<IActionResult> Add(AddWordViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            string? imagePath = null;

            if (model.PictureFile != null && model.PictureFile.Length > 0)
            {
                var username = HttpContext.Session.GetString("UserName") ?? "anonim";
                var folderPath = Path.Combine("images", "wordImages", username);
                var absoluteFolderPath = Path.Combine(_env.WebRootPath, folderPath);

                if (!Directory.Exists(absoluteFolderPath))
                {
                    Directory.CreateDirectory(absoluteFolderPath);
                }

                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(model.PictureFile.FileName);
                var filePath = Path.Combine(absoluteFolderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.PictureFile.CopyToAsync(stream);
                }

                imagePath = Path.Combine("/", folderPath, fileName).Replace("\\", "/");
            }

            var userId = HttpContext.Session.GetInt32("UserId");

            var word = new Word
            {
                EngWordName = model.EngWordName,
                TurWordName = model.TurWordName,
                Picture = imagePath,
                UserId=userId.Value
            };

            _context.Words.Add(word);
            await _context.SaveChangesAsync();

            foreach (var sample in model.SampleSentences)
            {
                if (!string.IsNullOrWhiteSpace(sample))
                {
                    var wordSample = new WordSample
                    {
                        WordId = word.WordId,
                        SampleSentence = sample
                    };
                    _context.WordSamples.Add(wordSample);
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("List");
        }

        //Returns word list
        public IActionResult List()
        {
            var words = _context.Words.ToList();
            return View(words);
        }

        //Deleting word
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var userId = HttpContext.Session.GetInt32("UserId");
            if (userId == null)
                return RedirectToAction("Login", "Auth");

            var word = _context.Words.FirstOrDefault(w => w.WordId == id && w.UserId == userId);
            if (word == null) return NotFound();

            var samples = _context.WordSamples.Where(s => s.WordId == id).ToList();
            _context.WordSamples.RemoveRange(samples);
            _context.Words.Remove(word);

            _context.SaveChanges();
            return RedirectToAction("List");
        }


    }


}
