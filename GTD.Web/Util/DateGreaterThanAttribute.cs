using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace GTD.Util
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class DateGreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _otherPropertyName;

        public DateGreaterThanAttribute(string otherPropertyName, string errorMessage)
            : base(errorMessage)
        {
            this._otherPropertyName = otherPropertyName;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var validationResult = ValidationResult.Success;

            var otherPropertyInfo = validationContext.ObjectType.GetProperty(this._otherPropertyName);

            DateTime toValidate = (DateTime)value;
            DateTime referenceProperty = (DateTime)otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (toValidate.CompareTo(referenceProperty) < 0)
            {
                validationResult = new ValidationResult(ErrorMessageString);
            }

            return validationResult;
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata,
            ControllerContext context)
        {
            string errorMessage = ErrorMessageString;

            var dateGreaterThanRule = new ModelClientValidationRule
            {
                ErrorMessage = errorMessage,
                ValidationType = "dategreaterthan"// This is the name the jQuery adapter will use
            };

            //"otherpropertyname" is the name of the jQuery parameter for the adapter, must be LOWERCASE!
            dateGreaterThanRule.ValidationParameters.Add("otherpropertyname", _otherPropertyName);

            yield return dateGreaterThanRule;
        }
    }
}