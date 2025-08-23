using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using ThreadPilot.Insurances.Application.Services;
using ThreadPilot.Insurances.Application.Contracts;
using ThreadPilot.Insurances.Domain.Validators;
using ThreadPilot.Insurances.Api.DTOs;
using ThreadPilot.Insurances.Domain.ValueObjects;
using Microsoft.AspNetCore.Mvc.ModelBinding;

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
    [HttpGet("{personalId}")]
    [ProducesResponseType(typeof(EnrichedInsuranceDto[]), 200)]
    [ProducesResponseType(typeof(ProblemDetails), 400)]
    [ProducesResponseType(typeof(ProblemDetails), 404)]
    [ProducesResponseType(typeof(ProblemDetails), 503)]
    public async Task<ActionResult<EnrichedInsuranceDto[]>> GetInsurancesByPersonalId(
        [ModelBinder] SwedishPersonalId personalId,
        CancellationToken cancellationToken = default)
    {
        var result = await insuranceService.GetInsurancesAsync(personalId.Value, cancellationToken).ConfigureAwait(false);

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
