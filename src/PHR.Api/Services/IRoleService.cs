using PHR.Api.Models;
using System;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public interface IRoleService
    {
        Task<Role> CreateRoleAsync(string name);
        Task AssignPermissionAsync(Guid roleId, Guid permissionId);
    }
}
