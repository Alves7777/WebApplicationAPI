using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace WebApplicationAPI.Extensions
{
    public static class ControllerExtensions
    {
        public static int GetUserId(this ControllerBase controller)
        {
            var userIdClaim = controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                           ?? controller.User.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("Usuário não autenticado");
            }

            return int.Parse(userIdClaim);
        }

        public static string GetUserEmail(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.Email)?.Value 
                ?? controller.User.FindFirst("email")?.Value 
                ?? throw new UnauthorizedAccessException("Email não encontrado no token");
        }

        public static string GetUserRole(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.Role)?.Value 
                ?? controller.User.FindFirst("role")?.Value 
                ?? "User";
        }
    }
}
