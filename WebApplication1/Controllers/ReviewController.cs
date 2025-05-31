using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Threading.Tasks;
using System.Linq;

namespace WebApplication1.Controllers
{
    public class ReviewController : Controller
    {
        private readonly MongoDbService _mongo;
        public ReviewController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        [HttpGet]
        public async Task<IActionResult> Create(int questId, string userId = null, string date = null, string time = null)
        {
            var quest = await _mongo.GetQuestByIdAsync(questId);
            if (quest == null) return NotFound();
            string userName = "";
            if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(date) && !string.IsNullOrEmpty(time))
            {
                var booking = quest.Bookings?.Find(b => b.UserId == userId && b.Date == date && b.Time == time);
                if (booking != null)
                    userName = booking.Name;
            }
            ViewBag.QuestName = quest.Name;
            ViewBag.UserName = userName;
            return View(new Review { UserName = userName });
        }

        [HttpPost]
        public async Task<IActionResult> Create(int questId, string userName, string text, int rating)
        {
            var review = new Review
            {
                UserName = userName ?? "Гость",
                Text = text ?? string.Empty,
                Rating = rating,
                Date = System.DateTime.Now.ToString("yyyy-MM-dd")
            };
            await _mongo.AddReviewAsync(questId, review);
            return RedirectToAction("Thanks");
        }

        [HttpGet]
        public IActionResult Thanks()
        {
            return View();
        }
    }
}
