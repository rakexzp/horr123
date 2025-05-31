using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace WebApplication1.Models
{
    public enum QuestType
    {
        Type1,
        Type2
    }

    public class Booking
    {
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
        public string Time { get; set; } = string.Empty; // HH:mm
        public string Name { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // новое поле для e-mail
        public string Status { get; set; } = string.Empty; // Booked, Canceled
        public string PaymentStatus { get; set; } = string.Empty; // Pending, Paid, Failed
        public string UserId { get; set; } = string.Empty;
    }

    public class Review
    {
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string Date { get; set; } = string.Empty; // yyyy-MM-dd
    }

    public class Quest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public QuestType Type { get; set; }
        public string Description { get; set; } = string.Empty;
        public string picture { get; set; } = string.Empty;
        public int Price { get; set; }
        public List<Booking> Bookings { get; set; } = new List<Booking>();
        public List<Review> Reviews { get; set; } = new List<Review>();
    }
}
