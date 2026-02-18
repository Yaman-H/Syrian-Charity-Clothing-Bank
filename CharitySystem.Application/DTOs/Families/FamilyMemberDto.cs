using CharitySystem.Domain.Enums;

namespace CharitySystem.Application.DTOs.Families
{
    public class FamilyMemberDto
    {
        public int MemberID { get; set; }
        public string? FullName { get; set; }
        public int? Age { get; set; }
        public Gender Gender { get; set; }
    }
}
