using FluentValidation;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class ResetPasswordRequestValidation : AbstractValidator<ResetPasswordRequest>
    {
        public ResetPasswordRequestValidation()
        {
            RuleFor(p => p.PhoneNumber).NotEmpty().OverridePropertyName("Telefon Numarası")
               .NotNull()
               .MinimumLength(10)
               .MaximumLength(20);

            RuleFor(p => p.NewPassword).NotEmpty().OverridePropertyName("Parola")
                .MinimumLength(6)
                .MaximumLength(20)
                .Matches(@"[a-zA-Z]+").WithMessage("Your password must contain at least one uppercase letter.")
                .Matches(@"[0-9]+").WithMessage("Your password must contain at least one number.");
        }
    }
}
