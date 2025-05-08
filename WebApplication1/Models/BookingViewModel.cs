using System;
using System.Collections.Generic;

namespace WebApplication1.Models
{
    public class BookingViewModel
    {
        public Quest Quest { get; set; }
        public string SelectedDate { get; set; }
        public List<string> AllSlots { get; set; }
        public HashSet<string> BookedSlots { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string SelectedTime { get; set; }
        public string UserId { get; set; }
        public bool Success { get; set; }
    }
}
