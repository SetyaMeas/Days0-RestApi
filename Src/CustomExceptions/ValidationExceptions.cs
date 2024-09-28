using FluentValidation.Results;

namespace RestApi.Src.CustomExceptions
{
    public class ValidationExceptions : Exception
    {
        public List<ValidationFailure> Errors;

        public ValidationExceptions(List<ValidationFailure> errors)
        {
            Errors = errors;
        }
    }
}
