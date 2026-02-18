using CharitySystem.Domain.Enums;
namespace CharitySystem.Application.DTOs.Families
{
    public class FamilyProfileDto
    {
        public int FamilyId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string FamilyCode { get; set; } = string.Empty;  
        public int FamilyPoints { get; set; }
        public string FamilyAddress { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public AccountStatus AccountStatus { get; set; }
        public string? FamilyNotes { get; set; } 
        public List<FamilyMemberDto> Members { get; set; }
    }
}
