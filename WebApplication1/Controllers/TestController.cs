using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class TestController : Controller
    {
        private readonly BronContext _context;

        public TestController(BronContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Выполнить простой запрос к базе данных
            var testQuery = _context.Quests.FirstOrDefault();
            return testQuery != null ? Content("Successfully connected to the database.") : Content("No data found.");
        }
    }
}