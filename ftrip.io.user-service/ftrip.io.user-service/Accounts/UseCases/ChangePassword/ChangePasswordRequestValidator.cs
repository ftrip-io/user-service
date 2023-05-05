using FluentValidation;
using ftrip.io.framework.Globalization;

namespace ftrip.io.user_service.Accounts.UseCases.ChangePassword
{
    public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
    {
        public ChangePasswordRequestValidator(IStringManager stringManager)
        {
            RuleFor(request => request.NewPassword)
               .NotEmpty()
               .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "New password"))
               .MinimumLength(8)
               .WithMessage(stringManager.Format("Common_Validation_FieldLengthAtLeast", "New password", 8));
        }
    }
}