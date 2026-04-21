using System.Security.Claims;

namespace WebApplicationAPI.Helpers
{
    public class UserContext
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserContext(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }

            return userId;
        }

        public string? GetCurrentUserEmail()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Email)?.Value;
        }

        public string? GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Name)?.Value;
        }

        public string? GetCurrentUserRole()
        {
            return _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.Role)?.Value;
        }

        public bool IsAdmin()
        {
            return GetCurrentUserRole() == "Admin";
        }

        public bool IsAuthenticated()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
        }

        public int GetUserIdOrDefault()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                return 0;
            }

            return userId;
        }
    }
}
