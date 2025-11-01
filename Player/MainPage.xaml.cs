using Player.Helper;
using Player.Models;
using Player.Services;
namespace Player;

public partial class MainPage : ContentPage
{
	private readonly MongoService _mongoService;
	public MainPage()
	{
		InitializeComponent();
		_mongoService = new MongoService();
	}

	//Вход если пользователь уже существует
	private async void LogOn(object sender, EventArgs e)
	{
		string login = EmailEntry.Text?.Trim();
		string password = PasswordEntry.Text;

		// Проверка на пустые поля
		if (string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlert("Ошибка", "Введите логин или пароль!", "ОК");
			return;
		}

		// Есть ли вообще пользователь
		var user = await _mongoService.GetUserByLoginAsync(login);
		if (user == null)
		{
			await DisplayAlert("Ошибка!", "Данного пользователя не существует!", "OK");
			return;
		}

		//Проверка захэшированного пароля
		bool IsValid = PasswordHelper.Verification(password, user.Password, user.Salt);
        if (!IsValid)
        {
			await DisplayAlert("Ошибка!", "Неверный пароль!", "ОК");
			return;
        }

		await DisplayAlert("Успех", $"Добро пожаловать, {user.Login}!", "OK");

		//Кэшируем логин
		Preferences.Set("loggedUser", user.Email);
	}


	private async void Register(object sender, EventArgs e)
	{
		await Navigation.PushAsync(new SecondPage("Регистрация"));
	}
}