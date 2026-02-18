namespace CharitySystem.Domain.Entities
{
    public class Family
    {
        public int FamilyID { get; set; }
        public string? FamilyCode { get; set; }
        public int FamilyPoints { get; set; }
        public string? FamilyAddress { get; set; }
        public string? FamilyNotes { get; set; }
        
        public int UserID { get; set; }
        public User? User { get; set; } 

        public ICollection<FamilyMember> Members { get; set; } = [];
        public ICollection<Order> Orders { get; set; } = [];
    }
}