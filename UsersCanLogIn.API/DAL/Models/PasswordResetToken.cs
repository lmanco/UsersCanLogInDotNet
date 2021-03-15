using Microsoft.EntityFrameworkCore;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using UsersCanLogIn.API.Controllers.Util;

namespace UsersCanLogIn.API.DAL.Models
{
    public class PasswordResetToken
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.None)]
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public DateTime Expiration { get; set; }
    }

    public class PasswordResetTokenRequestDTO
    {
        [RequiredIfMissing("Username")]
        public string Email { get; set; }
        public string Username { get; set; }
        public string SiteUrlOverride { get; set; }
    }
}
