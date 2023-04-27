using FluentValidation;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Common;

namespace ftrip.io.user_service.Accounts.UseCases.CreateAccount
{
    public class CreateAccountRequestValidator : AbstractValidator<CreateAccountRequest>
    {
        public CreateAccountRequestValidator(IStringManager stringManager)
        {
            RuleFor(request => request.Username)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "Username"))
                .Matches(RegexConstants.ALPHANUMERIC_REGEX)
                .WithMessage(stringManager.Format("Common_Validation_FieldMustContainAlphaNumChars", "Username"));

            RuleFor(request => request.Password)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "Password"))
                .MinimumLength(8)
                .WithMessage(stringManager.Format("Common_Validation_FieldLengthAtLeast", "Password", 8));
        }
    }
}