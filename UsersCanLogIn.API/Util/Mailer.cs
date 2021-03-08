using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Net.Mail;

namespace UsersCanLogIn.API.Util
{
    public interface IMailer
    {
        void SendVerificationEmail(string recipient, string recipientUsername, string verificationUrl);
        void SendPasswordResetEmail(string recipient, string recipientUsername, string passwordResetUrl);
        void SendMail(IEnumerable<string> recipients, string subject, string body, bool isBodyHtml = false);
    }

    public class Mailer : IMailer
    {
        private const string RecipientUsernamePlaceholder = "{recipientUsername}";
        private const string VerificationUrlPlaceholder = "{verificationUrl}";
        private const string PasswordResetUrlPlaceholder = "{passwordResetUrl}";
        private static readonly string VerificationSubject = $"{Program.AppDisplayName} Account Activation";
        private static readonly string VerificationBody = string.Join("",
            $"Hello {RecipientUsernamePlaceholder},<br/><br/>",
            $"You have registered a new account for {Program.AppDisplayName}. ",
            "To verify your account and log in, click on the following activation link:<br/>",
            $"<a href=\"{VerificationUrlPlaceholder}\">{VerificationUrlPlaceholder}</a><br/><br/>",
            "If you feel you have received this email in error, please delete and disregard it.<br/><br/><br/>",
            "Regards,<br/><br/>",
            $"{Program.AppDisplayName} Team"
            );
        private static readonly string PasswordResetSubject = $"{Program.AppDisplayName} Password Reset";
        private static readonly string PasswordResetBody = string.Join("",
            $"Hello {RecipientUsernamePlaceholder},<br/><br/>",
            $"You are receiving this email because you have requested to reset your password for {Program.AppDisplayName}. ",
            "To set a new password, click on the following password reset link:<br/>",
            $"<a href=\"{PasswordResetUrlPlaceholder}\">{PasswordResetUrlPlaceholder}</a><br/><br/>",
            "If you feel you have received this email in error, please delete and disregard it.<br/><br/><br/>",
            "Regards,<br/><br/>",
            $"{Program.AppDisplayName} Team"
            );

        private readonly SmtpClient _smptClient;
        private readonly string _sender;

        public Mailer(IOptionsMonitor<SmtpConfig> smtpConfigOptions, ISmtpClientFactory smtpClientFactory)
        {
            SmtpConfig smtpConfig = smtpConfigOptions.CurrentValue;
            _smptClient = smtpClientFactory.Create(smtpConfig);
            _sender = smtpConfig.Email;
        }

        public void SendVerificationEmail(string recipient, string recipientUsername, string verificationUrl)
        {
            string verificationBody = VerificationBody.Replace(RecipientUsernamePlaceholder, recipientUsername)
                .Replace(VerificationUrlPlaceholder, verificationUrl);
            SendMail(new string[] { recipient }, VerificationSubject, verificationBody, true);
        }

        public void SendPasswordResetEmail(string recipient, string recipientUsername, string passwordResetUrl)
        {
            string passwordResetBody = PasswordResetBody.Replace(RecipientUsernamePlaceholder, recipientUsername)
                .Replace(PasswordResetUrlPlaceholder, passwordResetUrl);
            SendMail(new string[] { recipient }, PasswordResetSubject, passwordResetBody, true);
        }

        public void SendMail(IEnumerable<string> recipients, string subject, string body, bool isBodyHtml = false)
        {
            try
            {
                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_sender),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = isBodyHtml
                };
                foreach (string recipient in recipients)
                    mailMessage.To.Add(recipient);
                _smptClient.Send(mailMessage);
            }
            catch (SmtpException ex)
            {
                Console.Error.WriteLine(ex);
            }
        }
    }

}
