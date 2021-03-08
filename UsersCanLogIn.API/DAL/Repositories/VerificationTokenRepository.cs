using UsersCanLogIn.API.DAL.Models;
using UsersCanLogIn.API.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.DAL.Repositories
{
    public interface IVerificationTokenRepository
    {
        Task<VerificationToken> GetById(string id);
        Task Create(VerificationToken verificationToken);
        Task CreateDefaultAndEmail(string userEmail, string username, string siteUrl);
        Task Delete(string id);
        Task DeleteByEmail(string email);
    }

    public class VerificationTokenRepository : IVerificationTokenRepository
    {
        private const int ExpirationDays = 60;
        private const string SiteActivationPath = "/activate";

        private readonly VerificationTokenContext _context;
        private readonly IMailer _mailer;

        public VerificationTokenRepository(VerificationTokenContext context, IMailer mailer)
        {
            _context = context;
            _mailer = mailer;
        }

        public async Task<VerificationToken> GetById(string id)
        {
            return await _context.VerificationTokens.FindAsync(id);
        }

        public async Task Create(VerificationToken verificationToken)
        {
            _context.VerificationTokens.Add(verificationToken);
            await _context.SaveChangesAsync();
        }

        public async Task CreateDefaultAndEmail(string userEmail, string username, string siteUrl)
        {
            string id = string.Join("", Guid.NewGuid().ToString("N"), Guid.NewGuid().ToString("N"));
            await Create(new VerificationToken
            {
                Id = id,
                Email = userEmail,
                Expiration = DateTime.Now.AddDays(ExpirationDays)
            });
            _mailer.SendVerificationEmail(userEmail, username, $"{siteUrl}{SiteActivationPath}/{id}");
        }

        public async Task Delete(string id)
        {
            VerificationToken verificationToken = await _context.VerificationTokens.FindAsync(id);
            _context.VerificationTokens.Remove(verificationToken);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByEmail(string email)
        {
            IEnumerable<VerificationToken> tokens = _context.VerificationTokens.Where(token => token.Email == email);
            if (tokens.Any())
                _context.VerificationTokens.RemoveRange(tokens);
            await _context.SaveChangesAsync();
        }
    }
}
