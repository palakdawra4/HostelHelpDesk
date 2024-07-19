using Microsoft.AspNetCore.Identity;

namespace WebApplication1.Models
{
    public class Admin
    {
        public Guid Id { get; set; }
        public required string Username { get; set; }
        public required byte[] PasswordHash { get; set; }
        public required byte[] PasswordSalt { get; set; }
        
    }
}
