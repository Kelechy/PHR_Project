using System.Linq;
using System.Security.Claims;

namespace PHR.Api.Services
{
    public static class PermissionsHelper
    {
        public static bool HasPermission(this ClaimsPrincipal user, string permission)
        {
            if (user == null) return false;
            if (user.IsInRole("Admin")) return true;
            return user.Claims.Any(c => c.Type == "permission" && c.Value == permission);
        }
    }
}
