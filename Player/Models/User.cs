using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Player.Models;

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

    //Запись кода активации
    [BsonElement("code")]
    public string Code { get; private set; } = null;

    //Безопасное храение пароля
    [BsonElement("salt")]
    public string Salt { get; private set; } = null;

    //Конструктор для создания нового юзера
    public User(string login, string password, string email, string code, string salt)
    {
        Login = login;
        Password = password;
        Email = email;
        Code = code;
        Salt = salt;
    }

    //Для монго
    public User(){}
}