using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using PHR.Api.Services;
using System.Threading.Tasks;
using Xunit;
using PHR.Api.DTOs;
using System;
using Microsoft.Extensions.Configuration;

namespace PHR.Tests
{
    public class AuthServiceTests
    {
        private AppDbContext GetDb()
        {
            var opts = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;
            var db = new AppDbContext(opts);
            db.Roles.Add(new Role { Id = Guid.NewGuid(), Name = "Admin" });
            db.SaveChanges();
            return db;
        }

        [Fact]
        public async Task Register_CreatesUser()
        {
            var db = GetDb();
            var cfg = new Microsoft.Extensions.Configuration.ConfigurationBuilder().AddInMemoryCollection().Build();
            cfg["Jwt:Key"] = "EoU0oz0iWqDQ6a6OlXnpxWBTgTQpQehxDyaV6G9+OglETq5UrUonlZzJX2ps88T0";
            cfg["Jwt:ExpiryMinutes"] = "60";

            var svc = new AuthService(db, cfg);
            var res = await svc.RegisterAsync(new RegisterDto { Email = "test@example.com", Password = "P@ssword1" });
            Assert.NotNull(res);
            Assert.False(string.IsNullOrEmpty(res.Token));
            Assert.False(string.IsNullOrEmpty(res.RefreshToken));
        }
    }
}
