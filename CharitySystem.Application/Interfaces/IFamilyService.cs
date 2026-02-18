using CharitySystem.Application.DTOs.Orders;
using CharitySystem.Application.DTOs.Families;

namespace CharitySystem.Application.Interfaces
{
    public interface IFamilyService
    {
        Task ResetAllFamilyPointsAsync(int pointsPerMember = 50, int basePoints = 50);
        Task<List<FamilyProfileDto>> GetAllFamiliesAsync();
        Task<FamilyProfileDto> GetFamilyProfileAsync(int userId);
        Task<List<OrderResponseDto>> GetFamilyOrdersAsync(int userId);
        Task ArchiveAndDeleteFamilyAsync(int familyId);
        Task CreateFamilyAsync(CreateFamilyDto dto);
        Task UpdateFamilyAsync(int familyId, UpdateFamilyDto dto);
        Task AddFamilyMemberAsync(AddMemberDto dto);
        Task DeleteFamilyMemberAsync(int memberId);
    }
}
