using Microsoft.EntityFrameworkCore;
using PHR.Api.Models;
using System;

namespace PHR.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
        public DbSet<PatientRecord> PatientRecords => Set<PatientRecord>();
        public DbSet<AccessRequest> AccessRequests => Set<AccessRequest>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
            modelBuilder.Entity<RolePermission>().HasKey(rp => new { rp.RoleId, rp.PermissionId });

            // Seed roles/permissions/users
            var adminRoleId = Guid.NewGuid();
            var creatorRoleId = Guid.NewGuid();
            var viewerRoleId = Guid.NewGuid();

            var viewPermId = Guid.NewGuid();
            var createPermId = Guid.NewGuid();
            var approvePermId = Guid.NewGuid();
            var manageRolesPermId = Guid.NewGuid();

            modelBuilder.Entity<Role>().HasData(
                new Role { Id = adminRoleId, Name = "Admin" },
                new Role { Id = creatorRoleId, Name = "Creator" },
                new Role { Id = viewerRoleId, Name = "Viewer" }
            );

            modelBuilder.Entity<Permission>().HasData(
                new Permission { Id = viewPermId, Name = "viewPatientRecords" },
                new Permission { Id = createPermId, Name = "createPatientRecords" },
                new Permission { Id = approvePermId, Name = "approveAccessRequests" },
                new Permission { Id = manageRolesPermId, Name = "manageRoles" }
            );

            modelBuilder.Entity<RolePermission>().HasData(
                new RolePermission { RoleId = adminRoleId, PermissionId = viewPermId },
                new RolePermission { RoleId = adminRoleId, PermissionId = createPermId },
                new RolePermission { RoleId = adminRoleId, PermissionId = approvePermId },
                new RolePermission { RoleId = adminRoleId, PermissionId = manageRolesPermId },

                new RolePermission { RoleId = creatorRoleId, PermissionId = createPermId },

                new RolePermission { RoleId = viewerRoleId, PermissionId = viewPermId }
            );

            var adminId = Guid.NewGuid();
            var doctorId = Guid.NewGuid();
            var viewerId = Guid.NewGuid();

            modelBuilder.Entity<User>().HasData(
                new User { Id = adminId, Email = "admin@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), RoleId = adminRoleId },
                new User { Id = doctorId, Email = "doctor@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), RoleId = creatorRoleId },
                new User { Id = viewerId, Email = "viewer@example.com", PasswordHash = BCrypt.Net.BCrypt.HashPassword("Password123!"), RoleId = viewerRoleId }
            );
        }
    }
}
