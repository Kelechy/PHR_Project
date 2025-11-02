using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using PHR.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PHR.Tests
{
    public class PatientServiceTests
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
        public async Task Create_And_Get_Record()
        {
            var db = GetDb();
            var svc = new PatientService(db);
            var userId = Guid.NewGuid();
            var rec = new PatientRecord
            {
                PatientName = "John Doe",
                DateOfBirth = DateTime.UtcNow.AddYears(-30),
                CreatedBy = userId
            };
            var created = await svc.CreateAsync(rec);
            Assert.NotEqual(Guid.Empty, created.Id);

            var fetched = await svc.GetAsync(created.Id);
            Assert.NotNull(fetched);
            Assert.Equal("John Doe", fetched.PatientName);
        }
    }
}
