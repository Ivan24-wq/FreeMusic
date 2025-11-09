using System.Net;
using System.Net.Mail;
using System.Text;
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
                EnableSsl = enableSsl,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Timeout = 20000
            };

            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(fromEmail, fromPassword);

            using var message = new MailMessage()
            {
                From = new MailAddress(fromEmail, "FreeMusic Support", Encoding.UTF8),
                Subject = subject,
                Body = body,
                BodyEncoding = Encoding.UTF8,
                IsBodyHtml = true
            };

            message.To.Add(toEmail);
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
