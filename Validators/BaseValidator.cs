using FluentValidation;
using System.Text.RegularExpressions;

namespace Tiennthe171977_Oceanteach.Validators
{
    public abstract class BaseValidator<T> : AbstractValidator<T>
    {
        
        protected bool BeAValidPhoneNumber(string phoneNumber)
        {
            
            return !string.IsNullOrEmpty(phoneNumber) &&
                   Regex.IsMatch(phoneNumber, @"^0[3579]\d{8}$");
        }

        protected bool BeAValidCCCD(string cccd)
        {
            
            return !string.IsNullOrEmpty(cccd) &&
                   Regex.IsMatch(cccd, @"^\d{12}$");
        }

        protected bool BeAValidAge(int? age)
        {

            return age.HasValue && age.Value >= 18 && age.Value <= 100;
        }

        protected bool BeAValidDate(DateOnly date)
        {
            return date <= DateOnly.FromDateTime(DateTime.Now);
        }

    }
}
