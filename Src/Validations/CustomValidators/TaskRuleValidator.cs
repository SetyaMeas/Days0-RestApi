using FluentValidation;

namespace RestApi.Src.Validations.CustomValidators
{
    public static class TaskRuleValidator
    {
        private static GeneralMessage msg = new ();
        public static IRuleBuilderOptions<T, string> TaskValidator<T>(
            this IRuleBuilder<T, string> rule
        )
        {
            return rule.NotEmpty().WithMessage(msg.NotEmptyMsg("task"));
        }
    }
}
