using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using ThreadPilot.Vehicles.Domain.ValueObjects;

namespace ThreadPilot.Vehicles.Api.ModelBinding;

internal sealed class LicenseNumberModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(LicenseNumber))
            return new BinderTypeModelBinder(typeof(LicenseNumberModelBinder));
        return null;
    }
}
