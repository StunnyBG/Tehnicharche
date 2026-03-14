
using System.ComponentModel.DataAnnotations;

namespace Tehnicharche.GCommon
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class PriceRangeAttribute : ValidationAttribute
    {
        private readonly decimal minValue;
        private readonly decimal maxValue;

        public PriceRangeAttribute(double min = 0.01, double max = 1000000)
        {
            minValue = (decimal)min;
            maxValue = (decimal)max;
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
                return ValidationResult.Success;

            var modelType = value.GetType();

            var minProp = modelType.GetProperty("MinPrice");
            var maxProp = modelType.GetProperty("MaxPrice");

            if (minProp == null || maxProp == null)
                return ValidationResult.Success;

            var minValObj = minProp.GetValue(value);
            var maxValObj = maxProp.GetValue(value);

            decimal? minPrice = minValObj as decimal?;
            decimal? maxPrice = maxValObj as decimal?;

            if (minPrice.HasValue)
            {
                if (minPrice < minValue || minPrice > maxValue)
                    return new ValidationResult($"MinPrice must be between {minValue} and {maxValue}.");
            }

            if (maxPrice.HasValue)
            {
                if (maxPrice < minValue || maxPrice > maxValue)
                    return new ValidationResult($"MaxPrice must be between {minValue} and {maxValue}.");
            }

            if (minPrice.HasValue && maxPrice.HasValue && minPrice > maxPrice)
            {
                return new ValidationResult("MinPrice cannot be greater than MaxPrice.");
            }

            return ValidationResult.Success;
        }
    }
}