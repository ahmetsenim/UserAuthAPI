using FluentValidation;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class GetOTPRequestValidation : AbstractValidator<GetOTPRequest>
    {
        public GetOTPRequestValidation()
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası alanı boş olamaz!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre alanı boş olamaz!");
        }
    }
}
