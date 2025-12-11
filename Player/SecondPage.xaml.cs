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

        //Отпрака кода подтверждени
        var EmailService = new EmailService();
        bool ok = await EmailService.SendVeryfincationCode(email);

        if (!ok)
        {
            await DisplayAlert("Ошибка!", "Не удалось отправить письмо!", "OK");
            return;
        }
    
        //Временное храниение
        Preferences.Set("PendingUserEmail", email);
        Preferences.Set("PendingUserPassword", password);
        Preferences.Set("IsRegestration", true);
        await DisplayAlert("Письмо с кодом подтверждения отправден на почту", "Введите код", "OK");
        await Navigation.PushAsync(new VerificationPage());

        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ConfirmPassword.Text = string.Empty;
    }

}
