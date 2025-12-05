using Microsoft.Maui.Storage;
using Player.Helper;
using Player.Services;
using DotNetEnv;
using Player.Models;

namespace Player.Services
{
    public class EmailService
    {
        //Метод отправки кода подтверждения
	public async Task<bool> SendVeryfincationCode(string EmailEntry)
	{
        try
        {
            Env.Load();
            
            string code = GenerationVerificationCode();
            //Чтение пароля и эмейла из env
            string fromEmail = "iv8747583@gmail.com";

            string fromPassword = "qfsu eicq esyp fgwk";

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
}