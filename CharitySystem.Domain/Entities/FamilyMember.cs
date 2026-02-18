using CharitySystem.Domain.Enums;

namespace CharitySystem.Domain.Entities
{
    public class FamilyMember
    {
        public int MemberID { get; set; }
        public string? FullName { get; set; }
        public int? Age { get; set; }
        public Gender Gender { get; set; }

        // Foreign Key
        public int FamilyID { get; set; }
        public Family? Family { get; set; }
    }
}