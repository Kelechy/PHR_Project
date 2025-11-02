using System.ComponentModel.DataAnnotations;


namespace PHR.Api.DTOs
{
    public class PatientDto
    {
        [Key]
        public Guid Id { get; set; }
        [Required(ErrorMessage = "Patient name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Patient name must be between 2 and 100 characters")]
        public string PatientName { get; set; } = null!;

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        [PastDate(ErrorMessage = "Date of birth must be in the past")]
        public DateTime DateOfBirth { get; set; }

        [StringLength(255, ErrorMessage = "Diagnosis must not exceed 255 characters")]
        public string? Diagnosis { get; set; }

        [StringLength(500, ErrorMessage = "Treatment plan must not exceed 500 characters")]
        public string? TreatmentPlan { get; set; }

        [StringLength(1000, ErrorMessage = "Medical history must not exceed 1000 characters")]
        public string? MedicalHistory { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid? LastModifiedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }

    public class PastDateAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                if (date > DateTime.UtcNow.Date)
                    return new ValidationResult(ErrorMessage ?? "Date must not be in the future");
            }
            return ValidationResult.Success;
        }
    }
}
