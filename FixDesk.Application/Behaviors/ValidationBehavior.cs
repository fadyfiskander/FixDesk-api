using FluentValidation;
using Inventory.Application.Common.Interfaces;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace FixDesk.Application.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IBaseCommand
{
    private readonly IServiceProvider _provider;

    public ValidationBehavior(IServiceProvider provider)
    {
        _provider = provider;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // The pipeline receives the Application-side MediatR wrapper, while validators are
        // written against the pure contract record that lives in Inventory.Contracts.
        // Resolve the carried contract and validate it against its own validators.
        object target = request;
        if (request is IContractRequest contractCarrier)
            target = contractCarrier.Contract;

        var targetType = target.GetType();
        var validatorType = typeof(IValidator<>).MakeGenericType(targetType);

        var validators = _provider.GetServices(validatorType).Cast<IValidator>().ToArray();
        if (validators.Length == 0)
        {
            return await next();
        }

        var contextType = typeof(FluentValidation.ValidationContext<>).MakeGenericType(targetType);
        var context = (IValidationContext)Activator.CreateInstance(contextType, target)!;

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var errorsDictionary = validationResults
            .SelectMany(x => x.Errors)
            .Where(x => x != null)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propertyName, errorMessages) => new
                {
                    Key = propertyName,
                    Values = errorMessages.Distinct().ToArray()
                })
            .ToDictionary(x => x.Key, x => x.Values);

        if (errorsDictionary.Any())
        {
            throw new ValidationException(
                "Validation error",
                errorsDictionary.SelectMany(kvp => kvp.Value.Select(v => new FluentValidation.Results.ValidationFailure(kvp.Key, v)))
            );
        }

        return await next();
    }
}
