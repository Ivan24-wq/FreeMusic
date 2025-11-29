using Player.Models;
using Player.Services;
using Player.Helper;

namespace Player;
public partial class VerificationPage : ContentPage
{
    private readonly MongoService _mongoService;
    public VerificationPage()
    {
        InitializeComponent();
        _mongoService = new MongoService();
    }

    private async void VerifyCodeClicked(object sender, EventArgs e)
    {
        string enteredCode = CodeEntry.Text?.Trim();
        string savedCode = Preferences.Get("VerificationCode", "");
        if (string.IsNullOrEmpty(enteredCode))
        {
            StatusLabel.Text = "Введите код подтверждения!";
            return;
        }

        if(enteredCode == savedCode)
        {
            //Временные данные пользователя
            string email = Preferences.Get("PendingUserEmail", " ");
            string password = Preferences.Get("PendingUserPassword", "");

            //Проверка на не зарегистрированного пользователя
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                await DisplayAlert("Ошибка!", "Данный пользователь не найден!", "OK");
                return;
            }
          

            //Создание пользователя в БД и хэшируем пароль
            string salt = PasswordHelper.GenaretionSalt();
            string hash = PasswordHelper.PasswordHashed(password, salt);

            //новый юзер
            var newUser = new User(
                login: email,
                password: hash,
                email: email,
                code: null,
                salt: salt);

            try
            {
                await _mongoService.AddUserAsync(newUser);
                
                //Очистка временных данных
                Preferences.Remove("VerificationCode");
                Preferences.Remove("PendingUserEmail");
                Preferences.Remove("PendingUserPassword");


            await DisplayAlert("Успех!", "Вы успешно зарегистрировались!", "OK");
            await Navigation.PopToRootAsync();
            }
            catch(Exception ex)
            {
                await DisplayAlert("Ошибка!", $"Не удалось сохранить пользователя: {ex.Message}", "OK");
            }
        }
        else
        {
            StatusLabel.Text = "Неверный код!";
        }
    }
}