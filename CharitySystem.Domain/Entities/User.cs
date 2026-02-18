using CharitySystem.Domain.Enums;

namespace CharitySystem.Domain.Entities
{
    public class User
    {
        public string? Fullname { get; set; }
        public int UserID { get; set; }
        public string? Username { get; set; }
        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; } 
        public Gender Gender { get; set; }
        public UserRole Role { get; set; }
        public AccountStatus AccountStatus { get; set; } = AccountStatus.Active;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}