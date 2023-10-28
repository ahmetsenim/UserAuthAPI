using FluentValidation;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class UserLoginRequestValidation : AbstractValidator<UserLoginRequest>
    {
        public UserLoginRequestValidation()
        {
            RuleFor(x => x.OtpToken).NotEmpty().WithMessage("Doğrulama token değeri hatalı!");
            RuleFor(x => x.OtpCode).NotEmpty().WithMessage("Tek kullanımlık doğrulama şifrenizi giriniz!");
        }
    }
}
