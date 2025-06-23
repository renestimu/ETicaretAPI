using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Infrastructure.Services
{
    public class MailService : IMailService
    {
        readonly IConfiguration _configuration;
        public MailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

   

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            await SendEmailAsync(new string[] { to }, subject, body, isBodyHtml);
        }

        public async Task SendEmailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
        {
            MailMessage mailMessage = new MailMessage();
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = isBodyHtml;
            foreach (var to in tos)
            {
                mailMessage.To.Add(to);
            }
            mailMessage.From = new(_configuration["Mail:UserName"], "R Ticaret", Encoding.UTF8);
            SmtpClient smtpClient = new(); // SMTP sunucusu ve port numarası
            smtpClient.Credentials = new NetworkCredential(_configuration["Mail:UserName"], _configuration["Mail:Password"]); // SMTP kimlik doğrulama bilgileri
            smtpClient.Port = 587;
            smtpClient.EnableSsl = false; // SSL kullanımı
        
            smtpClient.Host = _configuration["Mail:Host"]; // SMTP sunucu adresi
            await smtpClient.SendMailAsync(mailMessage);
        }

        public async Task SendPasswordResetEmailAsync(string to, string userId, string resetToken)
        {
            StringBuilder body = new StringBuilder();
            body.AppendLine("Merhaba <br> Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.<br>" +
                " <strong><a target=\"_blank\" href=\"");
            body.AppendLine($"{_configuration["AngularClientURl"]}"); // Reset password URL from configuration
            body.AppendLine("/update-password/");
            body.AppendLine($"{userId}/{resetToken}\">Şifreni Yenile</a></strong>");
            body.AppendLine("<br><br><br><span style=\"font-size:12px;color:red;\"> Eğer bu talebi siz yapmadıysanız bu maili ciddiye almamanız yeterlidir.</span><br> <br> İyi günler dileriz.");

            await SendEmailAsync(to, "Şifre Yenileme Talebi", body.ToString());      

        }
        public async Task SendCompletedOrderEmailAsync(string to, string orderCode, DateTime orderDate,string nameSurName)
        {
            string body= $"Merhaba {nameSurName},<br><br>" +
                $"Siparişiniz başarıyla alınmıştır.<br>" +
                $"Sipariş Kodu: <strong>{orderCode}</strong><br>" +
                $"Sipariş Tarihi: <strong>{orderDate.ToString("dd/MM/yyyy")}</strong><br><br>" +
                "Siparişinizin detayları için lütfen hesabınıza giriş yapın.<br><br>" +
                "İyi günler dileriz.";
            await SendEmailAsync(to, "Sipariş Tamamlandı", body);
        }
    }
}
