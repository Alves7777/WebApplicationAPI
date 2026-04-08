using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;

namespace WebApplicationAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<IEnumerable<UserResponse>> GetAllUsersAsync();
        Task<UserResponse?> GetUserByIdAsync(int id);
        Task<UserResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request);
        Task<bool> DeleteUserAsync(int id);
        Task<PagedUserResponse> SearchUsersAsync(SearchUsersRequest request);
    }
}
