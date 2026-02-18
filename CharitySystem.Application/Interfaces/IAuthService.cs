using CharitySystem.Application.DTOs.Auth;

namespace CharitySystem.Application.Interfaces
{
    public interface IAuthService
    {   
        Task<AuthResponseDto> LoginAsync(LoginDto request); 
    }
}
