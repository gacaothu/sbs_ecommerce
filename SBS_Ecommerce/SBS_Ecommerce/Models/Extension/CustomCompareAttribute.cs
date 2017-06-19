using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SBS_Ecommerce.Models.Extension
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CustomCompareAttribute : ValidationAttribute
    {
        private readonly string comparisionProperty;

        public CustomCompareAttribute(string otherProperty)
        {
            comparisionProperty = otherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ErrorMessage = ErrorMessageString;
            var currentValue = (double)value;

            var property = validationContext.ObjectType.GetProperty(comparisionProperty);
            if (property == null)
            {
                throw new ArgumentException("Property with this name not found");
            }
            var comparisionValue = (double)property.GetValue(validationContext.ObjectInstance);
            if (currentValue <= comparisionValue)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }
    }
}