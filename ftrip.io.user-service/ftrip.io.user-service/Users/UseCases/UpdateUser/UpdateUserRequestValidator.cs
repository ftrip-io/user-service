using FluentValidation;
using ftrip.io.framework.Globalization;
using ftrip.io.user_service.Common;

namespace ftrip.io.user_service.Users.UseCases.UpdateUser
{
    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        public UpdateUserRequestValidator(IStringManager stringManager)
        {
            RuleFor(request => request.FirstName)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "First name"))
                .Matches(RegexConstants.ALPHA_REGEX)
                .WithMessage(stringManager.Format("Common_Validation_FieldMustContainAlphaChars", "First name"));

            RuleFor(request => request.LastName)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "Last name"))
                .Matches(RegexConstants.ALPHA_REGEX)
                .WithMessage(stringManager.Format("Common_Validation_FieldMustContainAlphaChars", "Last name"));

            RuleFor(request => request.Email)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "Email"))
                .Matches(RegexConstants.EMAIL_REGEX)
                .WithMessage(stringManager.Format("Common_Validation_FieldMustBeEmail", "Email"));

            RuleFor(request => request.City)
                .NotEmpty()
                .WithMessage(stringManager.Format("Common_Validation_FieldIsRequired", "City"))
                .Matches(RegexConstants.ALPHANUMERIC_REGEX)
                .WithMessage(stringManager.Format("Common_Validation_FieldMustContainAlphaNumChars", "City"));
        }
    }
}