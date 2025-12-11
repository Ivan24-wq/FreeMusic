using Player.Models;
using Player.Services;
using Player.Helper;
using Player.MainScreen;

namespace Player;

public partial class VerificationPage : ContentPage
{
    private readonly MongoService _mongoService;

    public VerificationPage()
    {
        InitializeComponent();
        _mongoService = new MongoService();
    }

    // Заполнение по одному символу
    private void OnTextChanged(object sender, TextChangedEventArgs e)
    {
        var boxes = new[] { Box1, Box2, Box3, Box4, Box5, Box6 };
        var entry = sender as Entry;

        int index = Array.IndexOf(boxes, entry);

        // Переход вперёд
        if (!string.IsNullOrEmpty(entry.Text))
        {
            if (index < boxes.Length - 1)
                boxes[index + 1].Focus();
        }
        else // Переход назад при удалении
        {
            if (index > 0)
                boxes[index - 1].Focus();

            return;
        }

        // Когда заполнены все 6 чисел — проверяем
        if (boxes.All(b => !string.IsNullOrEmpty(b.Text)))
        {
            Verify();
        }
    }

    // Проверка введённого кода и автоматическая регистрация
    private async void Verify()
    {
        string enteredCode =
            $"{Box1.Text}{Box2.Text}{Box3.Text}{Box4.Text}{Box5.Text}{Box6.Text}";

        string savedCode = Preferences.Get("VerificationCode", "");

        if (enteredCode == savedCode)
        {
            SetBoxesColor(Colors.Green);
            StatusLabel.Text = "Код верный!";

            //Проверка решистрация или востановление
            bool IsRegistration = Preferences.Get("IsRegestration", true);
            if (IsRegistration)
            {
                // Автоматическая регистрация
                await CompleteRegistrationAsync();
            }
            else
            {
                await Navigation.PushAsync(new Player.ResetPassword.ResetPassword());
            }
            
        }
        else
        {
            SetBoxesColor(Colors.Red);
            StatusLabel.Text = "Вы ввели некорректный код!";
        }
    }

    // Метод для смены цвета всех боксов
    private void SetBoxesColor(Color color)
    {
        Box1.TextColor = color;
        Box2.TextColor = color;
        Box3.TextColor = color;
        Box4.TextColor = color;
        Box5.TextColor = color;
        Box6.TextColor = color;
    }

    // Метод автоматической регистрации пользователя
    private async Task CompleteRegistrationAsync()
    {
        string email = Preferences.Get("PendingUserEmail", "");
        string password = Preferences.Get("PendingUserPassword", "");

        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Ошибка!", "Данный пользователь не найден!", "OK");
            return;
        }

        string salt = PasswordHelper.GenaretionSalt();
        string hash = PasswordHelper.PasswordHashed(password, salt);

        var newUser = new User(
            login: email,
            password: hash,
            email: email,
            code: null,
            salt: salt
        );

        try
        {
            await _mongoService.AddUserAsync(newUser);

            Preferences.Remove("VerificationCode");
            Preferences.Remove("PendingUserEmail");
            Preferences.Remove("PendingUserPassword");

            await Navigation.PushAsync(new Player.MainScreen.MainScreen());
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка!", $"Не удалось сохранить пользователя: {ex.Message}", "OK");
        }
    }
}
