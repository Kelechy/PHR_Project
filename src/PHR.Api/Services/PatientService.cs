using Microsoft.EntityFrameworkCore;
using PHR.Api.Data;
using PHR.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _db;
        public PatientService(AppDbContext db) { _db = db; }

        public async Task<PatientRecord> CreateAsync(PatientRecord record)
        {
            record.Id = Guid.NewGuid();
            record.CreatedDate = DateTime.UtcNow;
            _db.PatientRecords.Add(record);
            await _db.SaveChangesAsync();
            return record;
        }

        public async Task<PatientRecord?> GetAsync(Guid id)
        {
            return await _db.PatientRecords.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<IEnumerable<PatientRecord>> GetAllAccessibleAsync(Guid userId)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            var rolePerms = await (from rp in _db.RolePermissions
                                   join p in _db.Permissions on rp.PermissionId equals p.Id
                                   where rp.RoleId == user!.RoleId
                                   select p.Name).ToListAsync();

            if (rolePerms.Contains("viewPatientRecords"))
            {
                return await _db.PatientRecords.Where(p => !p.IsDeleted).ToListAsync();
            }

            var own = _db.PatientRecords.Where(p => p.CreatedBy == userId && !p.IsDeleted);

            var now = DateTime.UtcNow;
            var access = from ar in _db.AccessRequests
                         where ar.RequestingUserId == userId && ar.Status == "Approved"
                         && ar.StartDateTime <= now && ar.EndDateTime >= now
                         join pr in _db.PatientRecords on ar.PatientRecordId equals pr.Id
                         where !pr.IsDeleted
                         select pr;

            return await own.Concat(access).Distinct().ToListAsync();
        }

        public async Task<PatientRecord?> UpdateAsync(PatientRecord record, Guid userId)
        {
            var existing = await _db.PatientRecords.FindAsync(record.Id);
            if (existing == null || existing.IsDeleted) return null;
            if (existing.CreatedBy != userId) return null;
            existing.PatientName = record.PatientName;
            existing.DateOfBirth = record.DateOfBirth;
            existing.Diagnosis = record.Diagnosis;
            existing.TreatmentPlan = record.TreatmentPlan;
            existing.MedicalHistory = record.MedicalHistory;
            existing.LastModifiedBy = userId;
            existing.LastModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, Guid userId)
        {
            var existing = await _db.PatientRecords.FindAsync(id);
            if (existing == null || existing.IsDeleted) return false;
            if (existing.CreatedBy != userId) return false;
            existing.IsDeleted = true;
            existing.LastModifiedBy = userId;
            existing.LastModifiedDate = DateTime.UtcNow;
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CanViewAsync(Guid userId, Guid recordId)
        {
            var user = await _db.Users.Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return false;
            var record = await _db.PatientRecords.FindAsync(recordId);
            if (record == null || record.IsDeleted) return false;
            if (record.CreatedBy == userId) return true;

            var rolePerms = await (from rp in _db.RolePermissions
                                   join p in _db.Permissions on rp.PermissionId equals p.Id
                                   where rp.RoleId == user.RoleId
                                   select p.Name).ToListAsync();

            if (rolePerms.Contains("viewPatientRecords")) return true;

            var now = DateTime.UtcNow;
            var approved = await _db.AccessRequests.AnyAsync(ar =>
                ar.PatientRecordId == recordId &&
                ar.RequestingUserId == userId &&
                ar.Status == "Approved" &&
                ar.StartDateTime <= now && ar.EndDateTime >= now);

            return approved;
        }
    }
}
