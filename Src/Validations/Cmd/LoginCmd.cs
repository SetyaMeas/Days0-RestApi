using MediatR;
using FluentValidation;
using RestApi.Src.Validations.CustomValidators;

namespace RestApi.Src.Validations.Cmd
{
    public class LoginCmd : IRequest<Unit>
    {
        public string Email { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
    }
    public class LoginCmdValidator : AbstractValidator<LoginCmd> {
        public LoginCmdValidator()
        {
            RuleFor(x => x.Email).EmailValidator();
            RuleFor(x => x.Passwd).PasswordValidator();
        }
    }
    public class LoginCmdHandler : IRequestHandler<LoginCmd, Unit>
    {
        public Task<Unit> Handle(LoginCmd req, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
