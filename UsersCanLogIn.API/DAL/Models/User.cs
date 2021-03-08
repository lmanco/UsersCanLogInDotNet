using UsersCanLogIn.API.Util;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UsersCanLogIn.API.DAL.Models
{
    public class User
    {
        public long Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Username { get; set; }
        [Required]
        public string PasswordHash { get; set; }
        [Required]
        public UserRole Role { get; set; }
        [Required]
        public bool Verified { get; set; }
    }

    public class UserContext : DbContext
    {
        public UserContext(DbContextOptions<UserContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }

    public class UserRequestDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "{0} may only contain alphanumeric characters, ., and _.")]
        public string Username { get; set; }
        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^\S*$", ErrorMessage = "{0} may not contain white space.")]
        public string Password { get; set; }
        public string SiteUrlOverride { get; set; }
    }

    public class UserUpdateRequestDTO
    {
        [Required]
        public string Password { get; set; }
        [StringLength(int.MaxValue, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 3)]
        [RegularExpression("^[a-zA-Z0-9._]+$", ErrorMessage = "{0} may only contain alphanumeric characters, ., and _.")]
        public string Username { get; set; }
    }

    public class UserPasswordResetUpdateDTO
    {
        [Required]
        public string PasswordResetTokenId { get; set; }
        [Required]
        [StringLength(int.MaxValue, ErrorMessage = "{0} must be at least {2} characters long.", MinimumLength = 8)]
        [RegularExpression(@"^\S*$", ErrorMessage = "{0} may not contain white space.")]
        public string Password { get; set; }
    }

    public class UserVerificationRequestDTO
    {
        [Required]
        public string VerificationTokenId { get; set; }
    }

    public class UserResponseDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
    }
}
