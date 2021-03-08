using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.DAL.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task<PasswordResetToken> GetById(string id);
        Task Create(PasswordResetToken passwordResetToken);
        Task CreateDefaultAndEmail(string userEmail, string username, string siteUrl);
        Task Delete(string id);
        Task DeleteByEmail(string email);
    }

    public class PasswordResetTokenRepository : IPasswordResetTokenRepository
    {
        private const int ExpirationHours = 1;
        private const string SitePasswordResetPath = "/reset-password";

        private readonly PasswordResetTokenContext _context;
        private readonly IMailer _mailer;

        public PasswordResetTokenRepository(PasswordResetTokenContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;
        }

        public async Task<PasswordResetToken> GetById(string id)
        {
            return await _context.PasswordResetTokens.FindAsync(id);
        }

        public async Task Create(PasswordResetToken passwordResetToken)
        {
            _context.PasswordResetTokens.Add(passwordResetToken);
            await _context.SaveChangesAsync();
        }

        public async Task CreateDefaultAndEmail(string userEmail, string username, string siteUrl)
        {
            string id = string.Join("", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
            await Create(new PasswordResetToken
            {
                Id = id,
                Email = userEmail,
                Expiration = DateTime.Now.AddHours(ExpirationHours)
            });
            _mailer.SendPasswordResetEmail(userEmail, username, $"{siteUrl}{SitePasswordResetPath}/{id}");
        }

        public async Task Delete(string id)
        {
            PasswordResetToken passwordResetTokens = await _context.PasswordResetTokens.FindAsync(id);
            _context.PasswordResetTokens.Remove(passwordResetTokens);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByEmail(string email)
        {
            IEnumerable<PasswordResetToken> tokens = _context.PasswordResetTokens.Where(token => token.Email == email);
            if (tokens.Any())
                _context.PasswordResetTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}
