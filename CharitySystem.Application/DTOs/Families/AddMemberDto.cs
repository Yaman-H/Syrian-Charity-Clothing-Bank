using CharitySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Families
{
    public class AddMemberDto
    {
        [Required]
        public string FullName { get; set; } = string.Empty;
        [Range(1, 100)]
        public int Age { get; set; }
        [Required]
        public Gender Gender { get; set; }
        [Required]
        public string FamilyCode { get; set; } = string.Empty;
        [Required]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
