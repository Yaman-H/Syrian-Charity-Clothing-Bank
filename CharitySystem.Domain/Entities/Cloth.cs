using CharitySystem.Domain.Enums;

namespace CharitySystem.Domain.Entities
{
    public class Cloth
    {
        public int ClothID { get; set; }
        public string? ClothName { get; set; }
        public string? ClothCode { get; set; }
        public string? ClothSize { get; set; }
        public string? Description { get; set; }
        public ClothCategory Category { get; set; }
        public int ClothPoints { get; set; }
        public string? ImageUrl { get; set; } 
        public bool IsAvailable { get; set; } = true;
    }
}