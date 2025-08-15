using Application.Wrappers;
using FluentValidation;
using MediatR;

namespace Application.Pipelines
{
    public class ValidationPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>, IValidateMe
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationPipelineBehaviour(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            if (_validators.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResults = await Task
                    .WhenAll(
                    _validators.Select(x => x.ValidateAsync(context, cancellationToken)));

                if (validationResults.Any(vr => vr.IsValid))
                {
                    List<string> errors = new List<string>();

                    var failures = validationResults.SelectMany(x => x.Errors)
                        .Where(f=>f!=null).ToList();

                    foreach (var error in failures)
                    {
                        errors.Add(error.ErrorMessage);
                    }

                    return (TResponse)await ResponseWrapper.FailAsync(messages: errors);
                }
            }
            return await next();
        }
    }


}
