using Microsoft.AspNetCore.Mvc.ModelBinding;
using ThreadPilot.Insurances.Domain.ValueObjects;

namespace ThreadPilot.Insurances.Api.ModelBinding;

internal sealed class SwedishPersonalIdModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Personal ID is required.");
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var value = valueProviderResult.FirstValue;
        if (!SwedishPersonalId.TryParse(value, out var parsed, out var error))
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, error ?? "Invalid personal ID.");
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsed);
        return Task.CompletedTask;
    }
}

