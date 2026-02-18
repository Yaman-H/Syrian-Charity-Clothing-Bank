using CharitySystem.Application.DTOs.Clothes;
using CharitySystem.Domain.Enums;

namespace CharitySystem.Application.Interfaces
{
    public interface IClothService
    {
        Task<IEnumerable<ClothDto>> GetAllClothesAsync(bool onlyAvailable, ClothCategory? category = null);
        
        Task<ClothDto> GetClothByIdAsync(int id);
        
        Task AddClothAsync(CreateClothDto Dto);
        
        Task<ClothDto> UpdateClothAsync(int id, UpdateClothDto Dto);
        
        Task<bool> DeleteClothAsync(int id);
    }
}
