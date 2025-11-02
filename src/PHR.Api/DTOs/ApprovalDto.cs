using System;
using System.ComponentModel.DataAnnotations;

namespace PHR.Api.DTOs
{
    public class ApprovalDto
    {
        [Required(ErrorMessage = "Start date is required")]
        public DateTime Start { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DateGreaterThan(nameof(Start), ErrorMessage = "End date must be greater than start date")]
        public DateTime End { get; set; }
    }

    public class DateGreaterThanAttribute : ValidationAttribute
    {
        private readonly string _comparisonProperty;

        public DateGreaterThanAttribute(string comparisonProperty)
        {
            _comparisonProperty = comparisonProperty;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var currentValue = (DateTime?)value;

            var property = validationContext.ObjectType.GetProperty(_comparisonProperty);
            if (property == null)
                return new ValidationResult($"Unknown property: {_comparisonProperty}");

            var comparisonValue = (DateTime?)property.GetValue(validationContext.ObjectInstance);

            if (currentValue.HasValue && comparisonValue.HasValue && currentValue <= comparisonValue)
                return new ValidationResult(ErrorMessage ?? $"{validationContext.DisplayName} must be greater than {_comparisonProperty}");

            return ValidationResult.Success;
        }
    }
}
