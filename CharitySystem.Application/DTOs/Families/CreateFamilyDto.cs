using CharitySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Families
{
    public class CreateFamilyDto
    {
        // User
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Required] 
        public string Username { get; set; } = string.Empty;
        [Required]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
        [Required]
        public Gender Gender { get; set; }

        // Family Profile
        public string Address { get; set; } = string.Empty;
        [Required] 
        public int InitialPoints { get; set; } = 50;
        public string FamilyNote { get; set; } = string.Empty;

    }
}
