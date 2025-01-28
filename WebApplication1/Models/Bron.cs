using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public enum QuestType
    {
        Type1,
        Type2
    }

    public class Quest
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public QuestType Type { get; set; }
        public string Description { get; set; }
        public ICollection<Booking> Bookings { get; set; }
        
        public string picture { get; set; }
        
        public int Price { get; set; }
    }

    public class Booking
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Quest")]
        public int QuestId { get; set; }
        public Quest Quest { get; set; }
        
        [ForeignKey("Date")]
        public int DateId { get; set; }
        public Date Date { get; set; }
        
        public BookingStatus Status { get; set; }
    }

    public class Date
    {
        [Key]
        public int Id { get; set; }
        public DateTime DateTimeValue { get; set; }
        public TimeSpan Time { get; set; }
        public ICollection<Booking> Bookings { get; set; }
    }

    public enum BookingStatus
    {
        Booked,
        Canceled
    }
}
