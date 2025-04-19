using Microsoft.AspNetCore.Mvc;
using WordGame.Data;
using WordGame.Models.ViewModels;

namespace WordGame.Controllers
{
    public class MnemonicController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public MnemonicController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet]
        public IActionResult Add(int wordId)
        {
            var model = new MnemonicViewModel { WordId = wordId };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(MnemonicViewModel model)
        {
            var word = _context.Words.FirstOrDefault(w => w.WordId == model.WordId);
            if (word == null) return NotFound();

            if (model.MnemonicImage != null && model.MnemonicImage.Length > 0)
            {
                var username = HttpContext.Session.GetString("UserName") ?? "anonim";
                var folder = Path.Combine("images", "mnemonics", username);
                var absPath = Path.Combine(_env.WebRootPath, folder);

                if (!Directory.Exists(absPath))
                    Directory.CreateDirectory(absPath);

                var filename = Guid.NewGuid().ToString() + Path.GetExtension(model.MnemonicImage.FileName);
                var filePath = Path.Combine(absPath, filename);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.MnemonicImage.CopyToAsync(stream);
                }

                word.MnemonicImagePath = Path.Combine("/", folder, filename).Replace("\\", "/");
            }

            word.MnemonicNote = model.MnemonicNote;
            _context.Words.Update(word);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", "Word", new { id = word.WordId });
        }
    }

}
