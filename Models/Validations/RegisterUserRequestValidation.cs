using FluentValidation;
using System.Text.RegularExpressions;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class RegisterUserRequestValidation : AbstractValidator<RegisterUserRequest>
    {
        public RegisterUserRequestValidation()
        {
            RuleFor(x => x.FullName).NotEmpty().OverridePropertyName("Ad Soyad").Length(3, 30).Matches(@"[a-zA-Z]+");

            RuleFor(p => p.Password).NotEmpty().OverridePropertyName("Parola")
                .MinimumLength(6)
                .MaximumLength(20)
                .Matches(@"[a-zA-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
            //.Matches(@"[A-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
            //.Matches(@"[a-z]+").WithMessage("Your password must contain at least one lowercase letter.")
            //.Matches(@"[\!\?\*\.]+").WithMessage("Your password must contain at least one (!? *.).")

            RuleFor(p => p.PhoneNumber).NotEmpty().OverridePropertyName("Telefon Numarası")
               .NotNull()
               .MinimumLength(10)
               .MaximumLength(20);
           //.Matches(new Regex(@"((\(\d{3}\) ?)|(\d{3}-))?\d{3}-\d{4}")).WithMessage("PhoneNumber not valid")
        }
    }
}
