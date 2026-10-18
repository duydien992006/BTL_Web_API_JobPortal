using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using BLL.Interfaces;
namespace BLL
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IConfiguration config, ILogger<EmailSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task<bool> GuiAsync(string den, string tieuDe, string noiDungHtml)
        {
            string host = _config["Email:Smtp:Host"];
            if (string.IsNullOrWhiteSpace(host))
            {
                _logger.LogWarning("Chưa cấu hình Email:Smtp:Host nên không gửi được email tới {Den}", den);
                return false;
            }

            int port;
            if (!int.TryParse(_config["Email:Smtp:Port"], out port)) port = 587;
            string user = _config["Email:Smtp:User"];
            string matKhau = _config["Email:Smtp:Password"];
            string tu = _config["Email:Smtp:From"];
            if (string.IsNullOrWhiteSpace(tu)) tu = user;

            using (var client = new SmtpClient(host, port))
            {
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential(user, matKhau);
                using (var mail = new MailMessage(tu, den, tieuDe, noiDungHtml))
                {
                    mail.IsBodyHtml = true;
                    mail.SubjectEncoding = Encoding.UTF8;
                    mail.BodyEncoding = Encoding.UTF8;
                    await client.SendMailAsync(mail);
                }
            }
            return true;
        }
    }
}
