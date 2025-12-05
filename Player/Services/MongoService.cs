using MongoDB.Driver;
using Player.Models;

namespace Player.Services
{
    public class MongoService
    {
        private readonly IMongoCollection<User> _userCollection;

        // Конструктор класса
        public MongoService()
        {
            var client = new MongoClient("mongodb+srv://Darkivan:Python-js@cluster0.jj6jc.mongodb.net/");
            var database = client.GetDatabase("Base");
            _userCollection = database.GetCollection<User>("Users");
        }

        //Регистрация пользователя 
        public async Task AddUserAsync(User user)
        {
            await _userCollection.InsertOneAsync(user);
        }

        //Поиск пользователя в бд по логину
        public async Task<User> GetUserByLoginAsync(string login)
        {
            return await _userCollection
            .Find(u => u.Login == login)
            .FirstOrDefaultAsync();
        }

        //Поиск пользователя по логину(Для сброса пароля)
        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userCollection
            .Find(u => u.Email == email)
            .FirstOrDefaultAsync();
        }
    }
}