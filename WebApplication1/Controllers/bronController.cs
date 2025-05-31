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
            var bookings = await _mongo.GetBookingsForQuestAndDateAsync(questId, selectedDate);
            var bookedSlots = new HashSet<string>(bookings != null ? bookings.ConvertAll(b => b.Time) : new List<string>());
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
            }
            // Проверка: не более 2 бронирований на email и телефон (по всем датам и квестам)
            else
            {
                var allQuests = await _mongo.GetQuestsAsync();
                int totalEmailBookings = allQuests.SelectMany(q => q.Bookings ?? new List<Booking>()).Count(b => b.Email == email);
                int totalPhoneBookings = allQuests.SelectMany(q => q.Bookings ?? new List<Booking>()).Count(b => b.Phone == phone);
                if (totalEmailBookings >= 2)
                {
                    ModelState.AddModelError("email", "Этот e-mail уже использовался для 2 бронирований. Пожалуйста, используйте другой e-mail.");
                }
                else if (totalPhoneBookings >= 2)
                {
                    ModelState.AddModelError("phone", "Этот номер уже использовался для 2 бронирований. Пожалуйста, используйте другой номер.");
                }
                else
                {
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
                    // Отправляем письмо с просьбой оставить отзыв
                    if (!string.IsNullOrWhiteSpace(booking.Email))
                    {
                        var emailService = new WebApplication1.Services.EmailService();
                        string reviewLink = Url.Action("Create", "Review", new { questId = questId, userId = booking.UserId, date = booking.Date, time = booking.Time }, Request.Scheme) ?? string.Empty;
                        emailService.SendReviewRequest(booking.Email, quest?.Name ?? "Квест", reviewLink);
                    }
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
                        UserId = userId,
                        Success = true
                    };
                    return View(vm);
                }
            }
            // Если есть ошибки, возвращаем форму с ошибками
            var allSlotsErr = new List<string>();
            for (int hour = 10; hour < 20; hour++)
            {
                allSlotsErr.Add($"{hour:D2}:00");
                allSlotsErr.Add($"{hour:D2}:30");
            }
            var bookedSlotsErr = new HashSet<string>((await _mongo.GetBookingsForQuestAndDateAsync(questId, selectedDate)).ConvertAll(b => b.Time));
            var vmErr = new BookingViewModel
            {
                Quest = quest,
                SelectedDate = selectedDate,
                AllSlots = allSlotsErr,
                BookedSlots = bookedSlotsErr,
                Name = name,
                Phone = phone,
                Email = email,
                SelectedTime = selectedTime,
                UserId = userId,
                Success = false
            };
            return View(vmErr);
        }

        // GET: /Bron/Index
        public async Task<IActionResult> Index()
        {
            var quests = await _mongo.GetQuestsAsync();
            return View(quests);
        }
    }
}
