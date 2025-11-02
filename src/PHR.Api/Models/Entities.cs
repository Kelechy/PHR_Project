using System;
using System.ComponentModel.DataAnnotations;

namespace PHR.Api.Models
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Email { get; set; } = null!;
        [Required]
        public string PasswordHash { get; set; } = null!;
        public Guid RoleId { get; set; }
        public Role? Role { get; set; }
    }

    public class Role
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
    }

    public class Permission
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; } = null!;
    }

    public class RolePermission
    {
        public Guid RoleId { get; set; }
        public Role? Role { get; set; }
        public Guid PermissionId { get; set; }
        public Permission? Permission { get; set; }
    }

    public class PatientRecord
    {
        [Key]
        public Guid Id { get; set; }
        [Required]
        public string PatientName { get; set; } = null!;
        public DateTime DateOfBirth { get; set; }
        public string? Diagnosis { get; set; }
        public string? TreatmentPlan { get; set; }
        public string? MedicalHistory { get; set; }
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class AccessRequest
    {
        [Key]
        public Guid Id { get; set; }
        public Guid PatientRecordId { get; set; }
        public Guid RequestingUserId { get; set; }
        public string Reason { get; set; } = null!;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Approved, Declined, Expired
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
    }


    public class RefreshToken
    {
        [Key]
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string Token { get; set; } = null!;
        public DateTime Expires { get; set; }
        public bool Revoked { get; set; } = false;
    }
}
