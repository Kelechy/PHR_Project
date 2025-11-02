using PHR.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PHR.Api.Services
{
    public interface IAccessRequestService
    {
        Task<AccessRequest> RequestAccessAsync(Guid patientRecordId, Guid userId, string reason);
        Task<IEnumerable<AccessRequest>> GetPendingAsync();
        Task<bool> ApproveAsync(Guid requestId, Guid approverId, DateTime start, DateTime end);
        Task<bool> DeclineAsync(Guid requestId, Guid approverId);
    }
}
