using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Threading.Tasks;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDbService _mongo;
        private const string AdminPassword = "02022708";
        public AdminController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        [HttpGet]
        public IActionResult AddTestQuest()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTestQuest(string name, string description, string type, string picture, int price, string password)
        {
            if (password != AdminPassword)
            {
                ViewBag.Error = "Неверный пароль";
                return View();
            }
            var quest = new Quest
            {
                Id = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // уникальный id
                Name = name,
                Description = description,
                Type = type == "Type2" ? QuestType.Type2 : QuestType.Type1,
                picture = picture,
                Price = price
            };
            await _mongo.AddQuestAsync(quest);
            ViewBag.Success = true;
            return View();
        }
    }
}
