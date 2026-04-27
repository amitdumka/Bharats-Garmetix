using System.ComponentModel.DataAnnotations;


namespace Garmetix.Onboarding
{
    public sealed class GreaterThanAttribute : ValidationAttribute
    {
        public GreaterThanAttribute(string propertyName)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("The value cannot be null.");
            }

            object
                instance = validationContext.ObjectInstance,
                otherValue = instance.GetType().GetProperty(PropertyName)?.GetValue(instance);

            if (otherValue == null)
            {
                return new ValidationResult($"The property '{PropertyName}' cannot be null.");
            }

            if (((IComparable)value).CompareTo(otherValue) > 0)
            {
                return ValidationResult.Success;
            }

            return new ValidationResult("The current value is smaller than the other one.");
        }
    }
}
