using WebApplicationAPI.DTO;
using System.Threading.Tasks;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponse> RegisterAsync(RegisterRequest request);
        Task<AuthResponse> LoginAsync(LoginRequest request);
    }
}
