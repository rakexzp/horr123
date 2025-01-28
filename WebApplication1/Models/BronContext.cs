using Microsoft.EntityFrameworkCore;

namespace WebApplication1.Models
{
    public class BronContext : DbContext
    {
        public BronContext(DbContextOptions<BronContext> options) : base(options) { }
        

        public DbSet<Quest> Quests { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Date> Dates { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API configurations

            modelBuilder.Entity<Quest>()
                .HasKey(q => q.Id);

            modelBuilder.Entity<Booking>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Date>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Quest)
                .WithMany(q => q.Bookings)
                .HasForeignKey(b => b.QuestId);

            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Date)
                .WithMany(d => d.Bookings)
                .HasForeignKey(b => b.DateId);
        }
    }
}