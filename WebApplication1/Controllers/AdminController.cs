using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Services;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        private readonly MongoDbService _mongo;
        private const string AdminSessionKey = "IsAdmin";
        private const string AdminPassword = "02022708";
        public AdminController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString(AdminSessionKey) == "true";
        }

        private async Task<string> GetYandexDirectLink(string publicUrl)
        {
            try
            {
                using var http = new HttpClient();
                var response = await http.GetStringAsync($"https://yandexdisk.direct/api/download?url={Uri.EscapeDataString(publicUrl)}");
                var href = JsonDocument.Parse(response).RootElement.GetProperty("href").GetString();
                return href ?? publicUrl;
            }
            catch { return publicUrl; }
        }

        private string GetDropboxDirectLink(string url)
        {
            if (url.Contains("dropbox.com"))
            {
                url = url.Replace("?dl=0", "?raw=1").Replace("?dl=1", "?raw=1");
                if (!url.Contains("?raw=1"))
                    url += "?raw=1";
            }
            return url;
        }

        private async Task<string> GetDirectImageLink(string url)
        {
            if (url.Contains("disk.yandex.ru"))
                return await GetYandexDirectLink(url);
            if (url.Contains("dropbox.com"))
                return GetDropboxDirectLink(url);
            return url;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string password)
        {
            if (password == AdminPassword)
            {
                HttpContext.Session.SetString(AdminSessionKey, "true");
                return RedirectToAction("Menu");
            }
            ViewBag.Error = "Неверный пароль";
            return View();
        }

        [HttpGet]
        public IActionResult Menu()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            return View();
        }

        [HttpGet]
        public IActionResult AddTestQuest()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddTestQuest(string name, string description, string type, string picture, int price, string password)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            if (password != AdminPassword)
            {
                ViewBag.Error = "Неверный пароль";
                return View();
            }
            var directLink = await GetDirectImageLink(picture);
            var quest = new Quest
            {
                Id = (int)DateTimeOffset.UtcNow.ToUnixTimeSeconds(),
                Name = name,
                Description = description,
                Type = type == "Type2" ? QuestType.Type2 : QuestType.Type1,
                picture = directLink,
                Price = price
            };
            await _mongo.AddQuestAsync(quest);
            ViewBag.Success = true;
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> AllBookings()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            var quests = await _mongo.GetQuestsAsync();
            var bookings = new List<AdminBookingViewModel>();
            foreach (var quest in quests)
            {
                if (quest.Bookings != null)
                {
                    foreach (var booking in quest.Bookings)
                    {
                        if (booking.Status == "Booked")
                        {
                            bookings.Add(new AdminBookingViewModel
                            {
                                QuestId = quest.Id,
                                QuestName = quest.Name,
                                Date = booking.Date,
                                Time = booking.Time,
                                Name = booking.Name,
                                Phone = booking.Phone,
                                Email = booking.Email,
                                PaymentStatus = booking.PaymentStatus,
                                UserId = booking.UserId
                            });
                        }
                    }
                }
            }
            return View(bookings);
        }

        [HttpGet]
        public async Task<IActionResult> EditQuest()
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            var quests = await _mongo.GetQuestsAsync();
            return View(quests);
        }

        [HttpGet]
        public async Task<IActionResult> EditQuestItem(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            var quest = await _mongo.GetQuestByIdAsync(id);
            return View(quest);
        }

        [HttpPost]
        public async Task<IActionResult> EditQuestItem(int id, string name, string description, string type, string picture, int price)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            var quest = await _mongo.GetQuestByIdAsync(id);
            if (quest == null) return RedirectToAction("EditQuest");
            quest.Name = name;
            quest.Description = description;
            quest.Type = type == "Type2" ? QuestType.Type2 : QuestType.Type1;
            quest.picture = picture;
            quest.Price = price;
            await _mongo.UpdateQuestAsync(quest);
            ViewBag.Success = true;
            return View(quest);
        }

        [HttpGet]
        public async Task<IActionResult> DeleteQuest(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login");
            await _mongo.DeleteQuestAsync(id);
            return RedirectToAction("EditQuest");
        }
    }
}
