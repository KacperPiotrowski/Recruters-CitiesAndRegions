
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using System.Diagnostics;
using Microsoft.AspNetCore.Http.Extensions;
using CitiesAndRegions.Domain;
using CitiesAndRegions.Application.Services;
using CitiesAndRegions.Domain.Filtering;
using CitiesAndRegions.Domain.Entities;

namespace CitiesAndRegions.Api.Controllers;

[Route("api/[controller]")]
public sealed class CityController : Controller
{
    private const string PolicyName = "CityCachePolicy";
    
    private readonly ICityService _cityService;
    private readonly IOutputCacheStore _cache;

    public CityController(ICityService cityService, IOutputCacheStore outputCacheStore)
    {
        Debug.Assert(cityService is not null);
        Debug.Assert(outputCacheStore is not null);

        _cityService = cityService;
        _cache = outputCacheStore;
    }

    [HttpGet("{id}")]
    [OutputCache(PolicyName = PolicyName)]
    public async Task<ActionResult<CityDTO>> GetCityByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        CityDTO result = await _cityService.GetByIdAsync(id, cancellationToken);

        if(result is null)
            return NotFound();

        return Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<uint>> AddNewCityAsync([FromBody]CityDTO newCity, CancellationToken cancellationToken = default)
    {
        uint result = await _cityService.AddNewAsync(newCity, cancellationToken);
        await _cache.EvictByTagAsync(PolicyName, cancellationToken);
        return Created(new Uri(Request.GetEncodedUrl() + "/" + result), result);
    }

    [HttpPost("addRange")]
    public async Task<ActionResult<uint[]>> AddNewCitiesAsync([FromBody] IEnumerable<CityDTO> cityDTOs,
                                                              [FromQuery] bool addOnlyValid = false,
                                                              CancellationToken cancellationToken = default)
    {
        IEnumerable<uint> result = await _cityService.AddRangeAsync(cityDTOs, addOnlyValid, cancellationToken);
        await _cache.EvictByTagAsync(PolicyName, cancellationToken);
        return Created(new Uri(Request.GetEncodedUrl() + "/{id}"), result);
    }

    [HttpGet("getByCountry/{countryName}")]
    public async Task<ActionResult<IEnumerable<CityDTO>>> GetAllCitiesFromRegionAsync(string countryName,
                                                                                      [FromQuery] Filter filter,
                                                                                      CancellationToken cancellationToken = default)
    {
        IEnumerable<CityDTO> result = await _cityService.GetAllByRegionAsync(countryName, filter, cancellationToken);

        if (result is null)
            return NotFound();
        return Ok(result);
    }

    [HttpPatch("updatePopulation/{cityId}")]
    public async Task<ActionResult> UpdatePopulationByCityId(uint cityId,
                                                             [FromBody] ulong newPopulation,
                                                             CancellationToken cancellationToken = default)
    {
        await _cityService.UpdatePopulation(cityId, newPopulation,cancellationToken);

        return NoContent();
    }
}
