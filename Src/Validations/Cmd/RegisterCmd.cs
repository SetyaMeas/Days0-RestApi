using FluentValidation;
using MediatR;
using RestApi.Src.Validations.CustomValidators;

namespace RestApi.Src.Validations
{
    public class RegisterCmd : IRequest<Unit>
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Passwd { get; set; } = string.Empty;
    }
    public class RegisterCmdValidator : AbstractValidator<RegisterCmd>
    {
        public RegisterCmdValidator()
        {
            RuleFor(x => x.Username).UsernameValidator();
            RuleFor(x => x.Passwd).PasswordValidator();
            RuleFor(x => x.Email).EmailValidator();
        }
    }
    public class RegisterCmdHandler : IRequestHandler<RegisterCmd, Unit>
    {
        public Task<Unit> Handle(RegisterCmd req, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
