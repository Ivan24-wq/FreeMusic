using Player.Helper;
using Player.Services;
namespace Player.ResetPassword;

public partial class ResetPassword : ContentPage
{
    private readonly MongoService _mongoService;
    public ResetPassword()
    {
        InitializeComponent();
        _mongoService = new MongoService();
    }

    private async void Pass_Res(object sender, EventArgs e)
    {
        //Ввод пароля и подтверждение нового
        string password = Pass.Text;
        string confirm = Confirm.Text;

        //пароли не совпали
        if(password != confirm)
        {
            await DisplayAlert("Ошибка!", "Пароли не совпадают", "OK");
            return;
        }

        //Находи пользователя по логину(эмейл)
        string email = Preferences.Get("PendingUserEmail", "");
        //Пользователя найти не удлаось
        if (string.IsNullOrEmpty(email))
        {
            await DisplayAlert("Ошибка", "Данного пользователя нати не удалось!", "OK");
            return;
        }

        //Новый хэш пароля
        string salt = PasswordHelper.GenaretionSalt();
        string hash = PasswordHelper.PasswordHashed(password, salt);

        //Замена пароля на новый
        bool update = await _mongoService.UpdatePasswordAsync(email, hash, salt);
        if (update)
        {
            await DisplayAlert("Отлично!", "Вы сменили пароль!", "Продолжить");

            Preferences.Remove("PendingUserEmail");
            Preferences.Remove("VerificationCode");

            await Navigation.PopToRootAsync();
        }
        else
        {
            await DisplayAlert("Нам очень жаль!", "Не удалось сменить пароль!", "Обратитесь в поддержку!");
        }
    }
}