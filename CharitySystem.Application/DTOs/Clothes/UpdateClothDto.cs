using CharitySystem.Domain.Enums;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Clothes
{
    public class UpdateClothDto
    {
        public string? ClothName { get; set; }
        public string? ClothSize { get; set; }
        public ClothCategory Category { get; set; }
        public int ClothPoints { get; set; }
        public IFormFile? Image { get; set; }
        public string? Description { get; set; }
    }
}
