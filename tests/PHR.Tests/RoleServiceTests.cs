using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using PHR.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PHR.Tests
{
    public class RoleServiceTests
    {
        private AppDbContext GetDb()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new AppDbContext(opts);
            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task CreateRole_AssignsPermission()
        {
            var db = GetDb();
            var svc = new RoleService(db);
            var role = await svc.CreateRoleAsync("Tester");
            Assert.NotNull(role);

            // create permission
            var perm = new Permission { Id = Guid.NewGuid(), Name = "testPerm" };
            db.Permissions.Add(perm);
            await db.SaveChangesAsync();

            await svc.AssignPermissionAsync(role.Id, perm.Id);
            var rp = await db.RolePermissions.FindAsync(role.Id, perm.Id);
            Assert.NotNull(rp);
        }
    }
}
