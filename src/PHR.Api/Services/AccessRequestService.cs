using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public class AccessRequestService : IAccessRequestService
    {
        private readonly AppDbContext _db;
        public AccessRequestService(AppDbContext db) { _db = db; }

        public async Task<AccessRequest> RequestAccessAsync(Guid patientRecordId, Guid userId, string reason)
        {
            var req = new AccessRequest
            {
                Id = Guid.NewGuid(),
                PatientRecordId = patientRecordId,
                RequestingUserId = userId,
                Reason = reason,
                RequestDate = DateTime.UtcNow,
                Status = "Pending"
            };
            _db.AccessRequests.Add(req);
            await _db.SaveChangesAsync();
            return req;
        }

        public async Task<IEnumerable<AccessRequest>> GetPendingAsync()
        {
            return await _db.AccessRequests.Where(a => a.Status == "Pending").ToListAsync();
        }

        public async Task<bool> ApproveAsync(Guid requestId, Guid approverId, DateTime start, DateTime end)
        {
            var req = await _db.AccessRequests.FindAsync(requestId);
            if (req == null || req.Status != "Pending") return false;
            req.Status = "Approved";
            req.StartDateTime = start.ToUniversalTime();
            req.EndDateTime = end.ToUniversalTime();
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeclineAsync(Guid requestId, Guid approverId)
        {
            var req = await _db.AccessRequests.FindAsync(requestId);
            if (req == null || req.Status != "Pending") return false;
            req.Status = "Declined";
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
