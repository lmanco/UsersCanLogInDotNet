using UsersCanLogIn.API.DAL.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersCanLogIn.API.DAL.Repositories
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> List();
        Task<User> GetById(long id);
        Task<User> GetByEmail(string email);
        Task<User> GetByUsername(string username);
        Task Create(User user);
        Task Update(User user);
        Task Delete(long id);
        bool ExistsInCurrentContext(long id);
    }

    public class UserRepository : IUserRepository
    {
        private readonly UserContext _context;

        public UserRepository(UserContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<User>> List()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<User> GetById(long id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> GetByEmail(string email)
        {
            string emailLower = email.ToLower();
            return await _context.Users.SingleOrDefaultAsync(user => user.Email.ToLower() == emailLower);
        }

        public async Task<User> GetByUsername(string username)
        {
            string usernameLower = username.ToLower();
            return await _context.Users.SingleOrDefaultAsync(user => user.Username.ToLower() == usernameLower);
        }

        public async Task Create(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task Update(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task Delete(long id)
        {
            User user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }

        public bool ExistsInCurrentContext(long id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
