using FluentValidation;
using RestApi.Src.CustomExceptions;
using MediatR;

namespace RestApi.Src.Validations
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : class
    {
        private readonly IValidator<TRequest> _validator;

        public ValidationBehavior(IValidator<TRequest> validator)
        {
            _validator = validator;
        }

        public async Task<TResponse> Handle(
            TRequest req,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken
        )
        {
            var result = await _validator.ValidateAsync(req);
            if (result.IsValid)
            {
                return await next();
            }
            throw new ValidationExceptions(result.Errors);
        }
    }
}
