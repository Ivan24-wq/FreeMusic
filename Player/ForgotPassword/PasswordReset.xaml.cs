using Intents;
using Player.Models;
using Player.Services;
using Player.ResetPassword;
namespace Player.ForgotPassword
{
    public partial class PasswordReset : ContentPage
    {
        private readonly MongoService _mongoService;
        public PasswordReset() 
        {
            InitializeComponent();
            _mongoService = new MongoService();

        }
        //Кнопка Отправка кода
        private async void Reset(object sender, EventArgs e)
        {
            string email = Email.Text?.Trim();

            //Провкрка на пустое поле
            if (string.IsNullOrWhiteSpace(email))
            {
                await DisplayAlert("Ошибка!", "Поле не должно быть пустым!", "OK");
                return;
            }

            //Проверка на зарегистрированного пользователя
            var user = await _mongoService.GetUserByEmailAsync(email);
            if(user == null)
            {
                await DisplayAlert("Ошибка!", "Данного пользователя не существует!", "ОК");
                return;
            }

            //Отравка кода подтверждения
            var EmailService = new EmailService();
        bool ok = await EmailService.SendVeryfincationCode(email);

        if (!ok)
        {
            await DisplayAlert("Ошибка!", "Не удалось отправить письмо!", "OK");
            return;
        }
        await DisplayAlert("Письмо с кодом подтверждения отправден на почту", "Введите код", "OK");

        //Созраниени email(регистрация или востановление)
        Preferences.Set("PendingUserEmail", email);
        Preferences.Set("IsRegestration", false);
        await Navigation.PushAsync(new VerificationPage());
        }
    }
}