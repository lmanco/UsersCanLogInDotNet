using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace UsersCanLogIn.API.Controllers.Util
{
    public class RequiredIfMissingAttribute : ValidationAttribute
    {
        private string[] _fields;

        public RequiredIfMissingAttribute(params string[] fields)
        {
            _fields = fields;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (!string.IsNullOrEmpty(value as string) || _fields.Any(field => GetPropertyByName(validationContext.ObjectInstance, field) != null))
                return ValidationResult.Success;
            string errorMessage = string.Join(" or ", new string[] { validationContext.DisplayName }.Concat(_fields.Select(field => field.ToLower()))) + " is required.";
            return new ValidationResult(errorMessage);
        }

        private object GetPropertyByName(object obj, string name)
        {
            return obj.GetType().GetProperty(name).GetValue(obj, null);
        }
    }
}
