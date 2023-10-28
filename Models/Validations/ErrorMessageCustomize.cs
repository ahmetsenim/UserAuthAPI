using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;
using UserAuthAPI.Models.Dtos;

namespace UserAuthAPI.Models.Validations
{
    public class ErrorMessageCustomize
    {
        public static DataResult ErrorDetail { get; set; } = new DataResult();
        public static IActionResult MakeValidationResponse(ActionContext context)
        {
            List<MessageItem> em = new List<MessageItem>();
            foreach (var keyModelStatePair in context.ModelState)
            {
                var errors = keyModelStatePair.Value.Errors;
                if (errors != null)
                {
                    foreach (var item in errors)
                    {
                        em.Add(new MessageItem { Message = item.ErrorMessage, Field = keyModelStatePair.Key });
                    }
                }
            }
            ErrorDetail.Success = false;
            ErrorDetail.Messages = em;

            var result = new BadRequestObjectResult(ErrorDetail.Messages);
            result.ContentTypes.Add("application/problem+json");
            return result;
        }
    }
}