using FluentValidation;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class GetResetPasswordOTPRequestValidation : AbstractValidator<GetResetPasswordOTPRequest>
    {
        public GetResetPasswordOTPRequestValidation()
        {
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası alanı boş olamaz!");
        }
    }
}
