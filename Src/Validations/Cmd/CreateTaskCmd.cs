using FluentValidation;
using MediatR;
using RestApi.Src.Validations.CustomValidators;

namespace RestApi.Src.Validations.Cmd
{
    public class CreateTaskCmd : IRequest<Unit>
    {
        public string Task { get; set; } = string.Empty;
    }

    public class CreateTaskCmdValidator : AbstractValidator<CreateTaskCmd>
    {
        public CreateTaskCmdValidator()
        {
            RuleFor(x => x.Task).TaskValidator();
        }
    }

    public class CreateTaskCmdHandler : IRequestHandler<CreateTaskCmd, Unit>
    {
        public Task<Unit> Handle(CreateTaskCmd request, CancellationToken cancellationToken)
        {
            return Task.FromResult(Unit.Value);
        }
    }
}
