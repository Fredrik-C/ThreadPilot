using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using ThreadPilot.Insurances.Domain.ValueObjects;

namespace ThreadPilot.Insurances.Api.ModelBinding;

internal sealed class SwedishPersonalIdModelBinderProvider : IModelBinderProvider
{
    public IModelBinder? GetBinder(ModelBinderProviderContext context)
    {
        if (context.Metadata.ModelType == typeof(SwedishPersonalId))
        {
            return new BinderTypeModelBinder(typeof(SwedishPersonalIdModelBinder));
        }
        return null;
    }
}

