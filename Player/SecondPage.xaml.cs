using Player.Models;
using Player.Services;
using Player.Helper;
using DotNetEnv;

namespace Player;

public partial class SecondPage : ContentPage
{
    private readonly MongoService _mongoService;

    public SecondPage(string title)
    {
        InitializeComponent();
        this.Title = title;
        _mongoService = new MongoService();
    }

    private async void RegisterClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text?.Trim();
        string password = PasswordEntry.Text;
        string confirm = ConfirmPassword.Text;

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            await DisplayAlert("Ошибка", "Поля не должны быть пустыми!", "OK");
            return;
        }

        if (password != confirm)
        {
            await DisplayAlert("Ошибка!", "Пароли не совпадают", "OK");
            return;
        }

        var existingUser = await _mongoService.GetUserByLoginAsync(email);
        if (existingUser != null)
        {
            await DisplayAlert("Ошибка", "Такой пользователь уже существует!", "OK");
            return;
        }

        // Хэшируем пароль
        string salt = PasswordHelper.GenaretionSalt();
        string hash = PasswordHelper.PasswordHashed(password, salt);

        var newUser = new User(email, hash, salt);
        await _mongoService.AddUserAsync(newUser);
        bool sent = await SendVeryfincationCode(email);
        if (!sent)
        {
            await DisplayAlert("Ошибка", "Не удалось отправить код подтверждения", "OK");
        }

        await DisplayAlert("Успех", "Вы успешно зарегистрировались!", "OK");
        await Navigation.PushAsync(new VerificationPage());

        EmailEntry.Text = "";
        PasswordEntry.Text = "";
        ConfirmPassword.Text = "";
    }

    //Метод отправки кода подтверждения
	private async Task<bool> SendVeryfincationCode(string EmailEntry)
	{
		Env.Load();
		string code = GenerationVerificationCode();
		bool sent = await EmailHelper.SendEmailAsync(
			smtpHost: "smtp.yandex.ru",
			smtpPort: 465,
			fromEmail: "EMAIL",
			fromPassword: "PASSWORD",
			toEmail: EmailEntry, subject: " Коод подтверждения",
			body: $"Ваш код: {code}");

		if (sent)
		{
			Preferences.Set("VerificationCode", code);
		}
		return sent;
	}
	
	//Генерация кода
	private string GenerationVerificationCode()
    {
		return new Random().Next(100000, 999999).ToString();
    }
}
