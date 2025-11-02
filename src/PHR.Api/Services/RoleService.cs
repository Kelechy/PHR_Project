using PHR.Api.Data;
using PHR.Api.Models;
using System;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public class RoleService : IRoleService
    {
        private readonly AppDbContext _db;
        public RoleService(AppDbContext db) { _db = db; }

        public async Task<Role> CreateRoleAsync(string name)
        {
            var role = new Role { Id = Guid.NewGuid(), Name = name };
            _db.Roles.Add(role);
            await _db.SaveChangesAsync();
            return role;
        }

        public async Task AssignPermissionAsync(Guid roleId, Guid permissionId)
        {
            _db.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            await _db.SaveChangesAsync();
        }
    }
}
