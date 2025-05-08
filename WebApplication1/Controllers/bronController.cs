using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebApplication1.Models;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    public class BronController : Controller
    {
        private readonly MongoDbService _mongo;
        public BronController(MongoDbService mongo)
        {
            _mongo = mongo;
        }

        // GET: /Bron/Create?questId=1&date=2025-05-10
        [HttpGet]
        public async Task<IActionResult> Create(int questId, string date = null)
        {
            var quest = await _mongo.GetQuestByIdAsync(questId);
            if (quest == null)
                return NotFound();
            string selectedDate = date ?? DateTime.Today.ToString("yyyy-MM-dd");
            // Генерируем все возможные слоты времени
            var allSlots = new List<string>();
            for (int hour = 10; hour < 20; hour++)
            {
                allSlots.Add($"{hour:D2}:00");
                allSlots.Add($"{hour:D2}:30");
            }
            // Получаем забронированные слоты
            var bookedSlots = new HashSet<string>((await _mongo.GetBookingsForQuestAndDateAsync(questId, selectedDate)).ConvertAll(b => b.Time));
            var vm = new BookingViewModel
            {
                Quest = quest,
                SelectedDate = selectedDate,
                AllSlots = allSlots,
                BookedSlots = bookedSlots,
                Success = false
            };
            return View(vm);
        }

        // POST: /Bron/Create
        [HttpPost]
        public async Task<IActionResult> Create(int questId, string selectedDate, string selectedTime, string name, string phone, string email, string userId)
        {
            var quest = await _mongo.GetQuestByIdAsync(questId);
            if (quest == null)
                return NotFound();
            // Проверяем, не занят ли слот
            var bookings = await _mongo.GetBookingsForQuestAndDateAsync(questId, selectedDate);
            if (bookings.Exists(b => b.Time == selectedTime))
            {
                ModelState.AddModelError("", "Этот слот уже занят!");
                return await Create(questId, selectedDate);
            }
            var booking = new Booking
            {
                Date = selectedDate,
                Time = selectedTime,
                Name = name,
                Phone = phone,
                Email = email,
                Status = "Booked",
                PaymentStatus = "Pending",
                UserId = userId
            };
            await _mongo.AddBookingAsync(questId, booking);
            // После бронирования можно показать кнопку "Оплатить"
            var allSlots = new List<string>();
            for (int hour = 10; hour < 20; hour++)
            {
                allSlots.Add($"{hour:D2}:00");
                allSlots.Add($"{hour:D2}:30");
            }
            var bookedSlots = new HashSet<string>((await _mongo.GetBookingsForQuestAndDateAsync(questId, selectedDate)).ConvertAll(b => b.Time));
            var vm = new BookingViewModel
            {
                Quest = quest,
                SelectedDate = selectedDate,
                AllSlots = allSlots,
                BookedSlots = bookedSlots,
                Name = name,
                Phone = phone,
                Email = email,
                SelectedTime = selectedTime,
                Success = true
            };
            return View(vm);
        }

        // GET: /Bron/Index
        public async Task<IActionResult> Index()
        {
            var quests = await _mongo.GetQuestsAsync();
            return View(quests);
        }

        // POST: /Bron/Pay
        [HttpPost]
        public async Task<IActionResult> Pay(int questId, string selectedDate, string selectedTime, string userId)
        {
            await _mongo.UpdateBookingPaymentStatusAsync(questId, selectedDate, selectedTime, userId, "Paid");

            // Получаем бронирование для извлечения email и названия квеста
            var quest = await _mongo.GetQuestByIdAsync(questId);
            var booking = quest?.Bookings?.Find(b => b.Date == selectedDate && b.Time == selectedTime && b.UserId == userId);
            if (booking != null && !string.IsNullOrWhiteSpace(booking.Email))
            {
                var emailService = new WebApplication1.Services.EmailService();
                string reviewLink = Url.Action("Create", "Review", new { questId = questId }, Request.Scheme);
                emailService.SendReviewRequest(booking.Email, quest?.Name ?? "Квест", reviewLink);
            }
            return RedirectToAction("Create", new { questId = questId, date = selectedDate });
        }
    }
}
