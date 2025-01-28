using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BronController(BronContext context) : Controller
    {
        // GET: bronController
        public IActionResult Index()
        {
            var quests = context.Quests.ToList();
            return View(quests);
        }

        // GET: bronController/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: bronController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(IFormCollection collection)
        {
            try
            {
                // Добавьте логику создания квеста здесь
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
