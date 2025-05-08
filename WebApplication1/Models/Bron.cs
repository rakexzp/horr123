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
        public string Date { get; set; } // yyyy-MM-dd
        public string Time { get; set; } // HH:mm
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; } // новое поле для e-mail
        public string Status { get; set; } // Booked, Canceled
        public string PaymentStatus { get; set; } // Pending, Paid, Failed
        public string UserId { get; set; }
    }

    public class Review
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string Text { get; set; }
        public int Rating { get; set; }
        public string Date { get; set; } // yyyy-MM-dd
    }

    public class Quest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public QuestType Type { get; set; }
        public string Description { get; set; }
        public string picture { get; set; }
        public int Price { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<Review> Reviews { get; set; }
    }
}
