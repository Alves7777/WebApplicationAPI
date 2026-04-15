using WebApplicationAPI.DTO;
using WebApplicationAPI.Models;
using WebApplicationAPI.Repositories.Interfaces;
using WebApplicationAPI.Services.Interfaces;

namespace WebApplicationAPI.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<UserResponse>> GetAllUsersAsync(int requestUserId)
        {
            // ? Chama repository que usa SP com verificação de Admin
            var users = await _userRepository.GetAllAsync(requestUserId);
            return users.Select(u => new UserResponse
            {
                Id = u.Id,
                Name = u.Name,
                Email = u.Email,
                Role = u.Role,
                IsActive = u.IsActive
            });
        }

        public async Task<UserResponse?> GetUserByIdAsync(int id, int requestUserId)
        {
            var user = await _userRepository.GetByIdAsync(id);

            if (user is null)
                return null;

            // ? Verificar se o requestUser pode ver este usuário
            // Admin vê qualquer um, usuário comum só vê a si mesmo
            var requestUser = await _userRepository.GetByIdAsync(requestUserId);
            if (requestUser?.Role != "Admin" && id != requestUserId)
            {
                throw new UnauthorizedAccessException("Você não tem permissão para ver este usuário");
            }

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive
            };
        }

        public async Task<UserResponse> CreateUserAsync(CreateUserRequest request)
        {
            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser is not null)
            {
                throw new InvalidOperationException("Email já cadastrado");
            }

            var user = new User
            {
                Name = request.Name,
                Email = request.Email
            };

            var id = await _userRepository.CreateAsync(user);
            user.Id = id;

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<UserResponse> UpdateUserAsync(int id, UpdateUserRequest request)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser is null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            var emailInUse = await _userRepository.GetByEmailAsync(request.Email);
            if (emailInUse is not null && emailInUse.Id != id)
            {
                throw new InvalidOperationException("Email já cadastrado em outro usuário");
            }

            var user = new User
            {
                Id = id,
                Name = request.Name,
                Email = request.Email
            };

            await _userRepository.UpdateAsync(user);

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email
            };
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            var existingUser = await _userRepository.GetByIdAsync(id);
            if (existingUser is null)
            {
                throw new KeyNotFoundException($"Usuário com ID {id} não encontrado");
            }

            return await _userRepository.DeleteAsync(id);
        }

        public async Task<PagedUserResponse> SearchUsersAsync(SearchUsersRequest request)
        {
            var (users, totalRecords) = await _userRepository.SearchUsersAsync(
                request.SearchTerm,
                request.PageNumber,
                request.PageSize
            );

            return new PagedUserResponse
            {
                Items = users.Select(u => new UserResponse
                {
                    Id = u.Id,
                    Name = u.Name,
                    Email = u.Email
                }),
                PageNumber = request.PageNumber,
                PageSize = request.PageSize,
                TotalCount = totalRecords
            };
        }
    }
}
