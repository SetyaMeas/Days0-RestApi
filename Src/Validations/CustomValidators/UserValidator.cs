using FluentValidation;

namespace RestApi.Src.Validations.CustomValidators
{
    public static class UserValidator
    {
        private static readonly GeneralMessage msg = new();

        public static IRuleBuilderOptions<T, string> UsernameValidator<T>(
            this IRuleBuilder<T, string> rule
        )
        {
            const string property = "username";
            return rule.NotEmpty()
                .WithMessage(msg.NotEmptyMsg(property))
                .MinimumLength(6)
                .WithMessage(msg.MinLenMsg(property, 6))
                .MaximumLength(20)
                .WithMessage(msg.MaxLenMsg(property, 20));
        }

        public static IRuleBuilderOptions<T, string> PasswordValidator<T>(
            this IRuleBuilder<T, string> rule
        )
        {
            const string property = "password";
            return rule.NotEmpty()
                .WithMessage(msg.NotEmptyMsg(property))
                .MinimumLength(8)
                .WithMessage(msg.MinLenMsg(property, 8))
                .MaximumLength(20)
                .WithMessage(msg.MinLenMsg(property, 20));
        }

        public static IRuleBuilderOptions<T, string> EmailValidator<T>(
            this IRuleBuilder<T, string> rule
        )
        {
            const string property = "email";
            const string regex = @"^[A-Za-z0-9]+@[A-Za-z0-9]+(\.com)$";
            return rule.NotEmpty().WithMessage(msg.NotEmptyMsg(property)).Matches(regex).WithMessage("invalid email provided");
        }
    }
}
