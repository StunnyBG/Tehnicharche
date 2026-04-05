
using System.ComponentModel.DataAnnotations;

namespace Tehnicharche.GCommon.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class PriceAttribute : ValidationAttribute
    {
        private readonly decimal min;
        private readonly decimal max;

        public PriceAttribute(double min = 0.01, double max = 1000000)
        {
            this.min = (decimal)min;
            this.max = (decimal)max;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return new ValidationResult("Price is required.");
            }

            if (!decimal.TryParse(value.ToString(), out decimal price))
            {
                return new ValidationResult("Invalid price format.");
            }

            if (price < min || price > max)
            {
                return new ValidationResult($"Price must be between {min} and {max}.");
            }

            if (decimal.Round(price, 2) != price)
            {
                return new ValidationResult("Price cannot have more than 2 decimal places.");
            }

            return ValidationResult.Success;
        }
    }
}