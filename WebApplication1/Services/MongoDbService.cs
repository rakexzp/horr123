using MongoDB.Driver;
using WebApplication1.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebApplication1.Services
{
    public class MongoDbService
    {
        private readonly IMongoCollection<Quest> _quests;

        public MongoDbService(string connectionString, string dbName)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);
            _quests = db.GetCollection<Quest>("Quests");
        }

        public async Task<List<Quest>> GetQuestsAsync() => await _quests.Find(_ => true).ToListAsync();
        public async Task<Quest> GetQuestByIdAsync(int id) => await _quests.Find(q => q.Id == id).FirstOrDefaultAsync();
        public async Task AddQuestAsync(Quest quest) => await _quests.InsertOneAsync(quest);

        // Добавить бронирование к квесту
        public async Task AddBookingAsync(int questId, Booking booking)
        {
            var update = Builders<Quest>.Update.Push(q => q.Bookings, booking);
            await _quests.UpdateOneAsync(q => q.Id == questId, update);
        }

        // Получить все бронирования квеста на дату
        public async Task<List<Booking>> GetBookingsForQuestAndDateAsync(int questId, string date)
        {
            var quest = await GetQuestByIdAsync(questId);
            return quest?.Bookings?.FindAll(b => b.Date == date && b.Status == "Booked") ?? new List<Booking>();
        }

        // Добавить отзыв к квесту
        public async Task AddReviewAsync(int questId, Review review)
        {
            var update = Builders<Quest>.Update.Push(q => q.Reviews, review);
            await _quests.UpdateOneAsync(q => q.Id == questId, update);
        }

        // Получить отзывы квеста
        public async Task<List<Review>> GetReviewsAsync(int questId)
        {
            var quest = await GetQuestByIdAsync(questId);
            return quest?.Reviews ?? new List<Review>();
        }

        // Обновить статус оплаты
        public async Task UpdateBookingPaymentStatusAsync(int questId, string date, string time, string userId, string newStatus)
        {
            var filter = Builders<Quest>.Filter.And(
                Builders<Quest>.Filter.Eq(q => q.Id, questId),
                Builders<Quest>.Filter.ElemMatch(q => q.Bookings, b => b.Date == date && b.Time == time && b.UserId == userId)
            );
            var update = Builders<Quest>.Update.Set("Bookings.$.PaymentStatus", newStatus);
            await _quests.UpdateOneAsync(filter, update);
        }
    }
}
