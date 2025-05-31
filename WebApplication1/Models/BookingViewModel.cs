using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models
{
    public class BookingViewModel
    {
        public Quest Quest { get; set; } = null!;
        public string SelectedDate { get; set; } = string.Empty;
        public List<string> AllSlots { get; set; } = new List<string>();
        public HashSet<string> BookedSlots { get; set; } = new HashSet<string>();
        public string Name { get; set; } = string.Empty;
        [RegularExpression(@"^\+7\d{10}$", ErrorMessage = "Телефон должен быть в формате +7XXXXXXXXXX")]
        public string Phone { get; set; } = string.Empty;
        [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.[a-zA-Z]{2,}$", ErrorMessage = "Введите корректный email (например, example@mail.ru)")]
        [EmailAddress(ErrorMessage = "Введите корректный email")]
        public string Email { get; set; } = string.Empty;
        public string SelectedTime { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public bool Success { get; set; }
    }
}
