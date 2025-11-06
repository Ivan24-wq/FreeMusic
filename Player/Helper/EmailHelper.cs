using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
namespace Player.Helper;

public static class EmailHelper
{
    public static async Task<bool> SendEmailAsync(
        string smtpHost,
        int smtpPort,
        string fromEmail,
        string fromPassword,
        string toEmail,
        string subject,
        string body,
        bool enableSsl = true)
    {
        try
        {
            using var smtp = new SmtpClient(smtpHost, smtpPort)
            {
                Credentials = new NetworkCredential(fromEmail, fromPassword),
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            using var message = new MailMessage(fromEmail, toEmail, subject, body);
            await smtp.SendMailAsync(message);
            return true;
        }
        catch(Exception ex)
        {
            Console.WriteLine($"Не получилось отправить код! {ex.Message}");
            return false;
        }  
    }

}
