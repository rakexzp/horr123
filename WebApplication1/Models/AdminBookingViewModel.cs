namespace WebApplication1.Models
{
    public class AdminBookingViewModel
    {
        public int QuestId { get; set; }
        public string QuestName { get; set; } = string.Empty;
        public string Date { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PaymentStatus { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
