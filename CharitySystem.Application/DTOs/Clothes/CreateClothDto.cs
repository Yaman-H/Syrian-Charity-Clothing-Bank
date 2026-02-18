using Microsoft.AspNetCore.Http;
using CharitySystem.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace CharitySystem.Application.DTOs.Clothes
{
    public class CreateClothDto
    {
        [Required]
        public string? ClothName { get; set; }
        [Required]
        public string? ClothSize { get; set; }
        [Required]
        public ClothCategory Category { get; set; }
        [Range(0, 50)]
        public int ClothPoints { get; set; }
        [Required]
        public IFormFile? Image { get; set; }
        public string? Description { get; set; }
    }
}
