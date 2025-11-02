using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using PHR.Api.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace PHR.Tests
{
    public class AccessRequestServiceTests
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
        public async Task Request_And_Approve()
        {
            var db = GetDb();
            var svc = new AccessRequestService(db);
            var patientId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            var req = await svc.RequestAccessAsync(patientId, userId, "Need access");
            Assert.Equal("Pending", req.Status);

            var ok = await svc.ApproveAsync(req.Id, Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddHours(1));
            Assert.True(ok);

            var fetched = await db.AccessRequests.FindAsync(req.Id);
            Assert.Equal("Approved", fetched.Status);
        }
    }
}
