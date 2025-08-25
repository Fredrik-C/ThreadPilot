using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Primitives;
using Shouldly;
using ThreadPilot.Vehicles.Api.ModelBinding;
using ThreadPilot.Vehicles.Domain.ValueObjects;

namespace ThreadPilot.Vehicles.UnitTests;

public class LicenseNumberModelBinderTests
{
    private static DefaultModelBindingContext CreateBindingContext(string modelName, string? value)
    {
        var modelMetadataProvider = new EmptyModelMetadataProvider();
        var modelState = new ModelStateDictionary();
        var valueProvider = new TestValueProvider(new Dictionary<string, StringValues>
        {
            [modelName] = new StringValues(value)
        });

        return new DefaultModelBindingContext
        {
            ModelMetadata = modelMetadataProvider.GetMetadataForType(typeof(LicenseNumber)),
            ModelName = modelName,
            ModelState = modelState,
            ValueProvider = valueProvider
        };
    }

    private sealed class TestValueProvider(Dictionary<string, StringValues> values) : IValueProvider
    {
        public bool ContainsPrefix(string prefix) => values.Keys.Any(k => string.Equals(k, prefix, StringComparison.OrdinalIgnoreCase) || k.StartsWith(prefix + ".", StringComparison.OrdinalIgnoreCase));
        public ValueProviderResult GetValue(string key) => values.TryGetValue(key, out var v) ? new ValueProviderResult(v) : ValueProviderResult.None;
    }

    [Fact]
    public async Task BindModelAsync_ShouldBind_ForValidRegistrationNumber()
    {
        var binder = new LicenseNumberModelBinder();
        var context = CreateBindingContext("registrationNumber", "ABC123");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        context.Result.Model.ShouldBeOfType<LicenseNumber>();
        var obj = (LicenseNumber)context.Result.Model!;
        obj.Value.ShouldBe("ABC123");
    }

    [Fact]
    public async Task BindModelAsync_ShouldFail_AndAddError_ForInvalidRegistrationNumber()
    {
        var binder = new LicenseNumberModelBinder();
        var context = CreateBindingContext("registrationNumber", "");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeFalse();
        context.ModelState.ContainsKey("registrationNumber").ShouldBeTrue();
    }

    [Fact]
    public async Task BindModelAsync_ShouldBind_ForConfiguredScenarioId()
    {
        var binder = new LicenseNumberModelBinder();
        var context = CreateBindingContext("registrationNumber", "XYZ789");

        await binder.BindModelAsync(context);

        context.Result.IsModelSet.ShouldBeTrue();
        ((LicenseNumber)context.Result.Model!).Value.ShouldBe("XYZ789");
    }

    [Fact]
    public void BinderProvider_ShouldReturnBinder_ForLicenseNumber()
    {
        var provider = new LicenseNumberModelBinderProvider();
        var providerContext = new TestModelBinderProviderContext(typeof(LicenseNumber));

        var binder = provider.GetBinder(providerContext);
        binder.ShouldNotBeNull();
    }

    private sealed class TestModelBinderProviderContext(Type modelType) : ModelBinderProviderContext
    {
        public override BindingInfo? BindingInfo => null;
        public override ModelMetadata Metadata => new EmptyModelMetadataProvider().GetMetadataForType(modelType);
        public override IModelMetadataProvider MetadataProvider => new EmptyModelMetadataProvider();
        public override IModelBinder CreateBinder(ModelMetadata metadata) => throw new NotImplementedException();
    }
}

