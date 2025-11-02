using PHR.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public interface IPatientService
    {
        Task<PatientRecord> CreateAsync(PatientRecord record);
        Task<PatientRecord?> GetAsync(Guid id);
        Task<IEnumerable<PatientRecord>> GetAllAccessibleAsync(Guid userId);
        Task<PatientRecord?> UpdateAsync(PatientRecord record, Guid userId);
        Task<bool> SoftDeleteAsync(Guid id, Guid userId);
        Task<bool> CanViewAsync(Guid userId, Guid recordId);
    }
}
