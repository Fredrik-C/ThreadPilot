using Microsoft.AspNetCore.Mvc.ModelBinding;
using ThreadPilot.Vehicles.Domain.ValueObjects;

namespace ThreadPilot.Vehicles.Api.ModelBinding;

internal sealed class LicenseNumberModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        ArgumentNullException.ThrowIfNull(bindingContext);

        var valueProviderResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName);
        if (valueProviderResult == ValueProviderResult.None)
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, "Registration number is required.");
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        var value = valueProviderResult.FirstValue;
        if (!LicenseNumber.TryParse(value, out var parsed, out var error))
        {
            bindingContext.ModelState.AddModelError(bindingContext.ModelName, error ?? "Invalid registration number.");
            bindingContext.Result = ModelBindingResult.Failed();
            return Task.CompletedTask;
        }

        bindingContext.Result = ModelBindingResult.Success(parsed);
        return Task.CompletedTask;
    }
}

