using FluentValidation;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class GetLoginOTPRequestValidation : AbstractValidator<GetLoginOTPRequest>
    {
        public GetLoginOTPRequestValidation()
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası alanı boş olamaz!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre alanı boş olamaz!");
        }
    }
}
