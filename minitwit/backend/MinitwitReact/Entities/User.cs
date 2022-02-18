using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace MinitwitReact.Entities
{
    public partial class User
    {
        public long UserId { get; set; }
        public string Username { get; set; } = null!;
        
        [EmailAddress]
        public string Email { get; set; } = null!;
        public string PwHash { get; set; } = null!;
    }
}
