using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;

namespace UsersCanLogIn.API.Util
{
    public interface ISmtpClientFactory
    {
        SmtpClient Create(SmtpConfig smtpConfig);
    }

    public class SmtpClientFactory : ISmtpClientFactory
    {
        public SmtpClient Create(SmtpConfig smtpConfig)
        {
            return new SmtpClient(smtpConfig.Host)
            {
                Port = smtpConfig.Port,
                Credentials = new NetworkCredential(smtpConfig.Email, smtpConfig.Password),
                EnableSsl = smtpConfig.EnableSsl
            };
        }
    }

    public class SmtpConfig
    {
        [Required]
        public string Host { get; set; }
        [Required]
        public int Port { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public bool EnableSsl { get; set; }
    }
}
