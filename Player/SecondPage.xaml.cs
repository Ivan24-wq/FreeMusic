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

        //Отпрака кода подтверждени]
        bool sent = await SendVeryfincationCode(email);
        if (!sent)
        {
            await DisplayAlert("Ошибка", "Не удалось отправить код подтверждения", "OK");
            return;
        }


        //Временное храниение
        Preferences.Set("PendingUserEmail", email);
        Preferences.Set("PendingUserPassword", password);
        await DisplayAlert("Письмо с кодом подтверждения отправден на почту", "Введите код", "OK");
        await Navigation.PushAsync(new VerificationPage());

        EmailEntry.Text = string.Empty;
        PasswordEntry.Text = string.Empty;
        ConfirmPassword.Text = string.Empty;
    }

    //Метод отправки кода подтверждения
	private async Task<bool> SendVeryfincationCode(string EmailEntry)
	{
        try
        {
            Env.Load();
            string code = GenerationVerificationCode();
            //Чтение пароля и эмейла из env
            string fromEmail = Environment.GetEnvironmentVariable("EMAIL")!;
            string fromPassword = Environment.GetEnvironmentVariable("PASSWORD")!;

            //ОТладка на .env
            if(string.IsNullOrEmpty(fromEmail) || string.IsNullOrEmpty(fromPassword))
            {
                Console.WriteLine("Ошибка: Email или Password не указаны в .env");
                return false;
            }

            string subject = "Код подтверждения регистрации";
            string body = $"<h2>Приветствуем!</h2><p>Ваш код: <b>{code}. Его нужно использовать для подтверждения регистрации! С уважением, разработчик FreeMusic</b></p>";

            bool sent = await EmailHelper.SendEmailAsync(
                smtpHost: "smtp.gmail.com",
                smtpPort: 587,
                fromEmail: fromEmail,
                fromPassword: fromPassword,
                toEmail: EmailEntry, subject: subject,
                body: body);
                
            if (sent)
		    {
			Preferences.Set("VerificationCode", code);
		    }
            return sent;
            
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Ошибка при отправке письма! {ex.Message}");
            return false;
        }

		
	}
	
	//Генерация кода
	private string GenerationVerificationCode()
    {
		return new Random().Next(100000, 999999).ToString();
    }
}
