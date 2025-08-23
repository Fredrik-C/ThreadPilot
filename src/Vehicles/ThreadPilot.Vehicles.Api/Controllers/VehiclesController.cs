using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using ThreadPilot.Vehicles.Application.Services;
using ThreadPilot.Vehicles.Api.DTOs;
using ThreadPilot.Vehicles.Domain.ValueObjects;

namespace ThreadPilot.Vehicles.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[SuppressMessage("Design", "CA1515", Justification = "Public controller required for MVC discovery and integration tests")]
public sealed class VehiclesController(VehicleService vehicleService) : ControllerBase
{

    /// <summary>
    /// Gets vehicle information by registration number
    /// </summary>
    /// <param name="registrationNumber">The vehicle registration number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Vehicle information if found</returns>
    [HttpGet("{registrationNumber}")]
    [ProducesResponseType(typeof(VehicleDto), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 503)]
    public async Task<ActionResult<VehicleDto>> GetVehicleByRegistrationNumber(
        [ModelBinder] LicenseNumber registrationNumber,
        CancellationToken cancellationToken = default)
    {
        var result = await vehicleService.GetVehicleAsync(registrationNumber.Value, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var vehicleDto = VehicleDto.FromDomain(result.Vehicle!);
            return Ok(vehicleDto);
        }

        var problemDetails = result.ToProblemDetails();
        return new ObjectResult(problemDetails)
        {
            StatusCode = problemDetails.Status
        };
    }
}