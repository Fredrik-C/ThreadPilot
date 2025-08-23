using Microsoft.AspNetCore.Mvc;
using ThreadPilot.Insurances.Application.Services;
using ThreadPilot.Insurances.Application.Contracts;
using ThreadPilot.Insurances.Domain.Validators;
using ThreadPilot.Insurances.Api.DTOs;

namespace ThreadPilot.Insurances.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class InsurancesController(InsuranceService insuranceService) : ControllerBase
{
    /// <summary>
    /// Gets all insurances for a personal ID
    /// </summary>

    /// <param name="personalId">The personal ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of insurances</returns>
    [HttpGet]
    [HttpGet("{personalId?}")]
    [ProducesResponseType(typeof(EnrichedInsuranceDto[]), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 503)]
    public async Task<ActionResult<EnrichedInsuranceDto[]>> GetInsurancesByPersonalId(
        string? personalId,
        CancellationToken cancellationToken = default)
    {
        var id = personalId ?? string.Empty;
        // Validate the personal ID first
        var validationResult = SwedishPersonalIdValidator.Validate(id);
        if (!validationResult.IsValid)
        {
            var problemDetails = new ProblemDetails
            {
                Title = "Invalid Personal ID",
                Detail = validationResult.ErrorMessage,
                Status = 400,
                Type = "https://tools.ietf.org/html/rfc7231#section-6.5.1"
            };
            return new ObjectResult(problemDetails)
            {
                StatusCode = problemDetails.Status
            };
        }

        var result = await insuranceService.GetInsurancesAsync(id, cancellationToken).ConfigureAwait(false);

        if (result.IsSuccess)
        {
            var insuranceDtos = result.Insurances!.Select(EnrichedInsuranceDto.FromApplication).ToArray();
            return Ok(insuranceDtos);
        }

        var errorProblemDetails = result.ToProblemDetails();
        return new ObjectResult(errorProblemDetails)
        {
            StatusCode = errorProblemDetails.Status
        };
    }
}
