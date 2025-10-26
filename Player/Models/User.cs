using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Player;

public class User
{
    // Ключ документа
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]

    public string Id { get; private set; } = null;
    [BsonElement("login")]
    public string Login { get; private set; } = null;

    [BsonElement("password")]
    public string Password { get; private set; } = null;
    [BsonElement("email")]
    public string Email { get; private set; } = null;

    //Конструктор для создания нового юзера
    public User(string login, string password, string email)
    {
        Login = login;
        Password = password;
        Email = email;
    }

    //Для монго
    public User(){ }
}
