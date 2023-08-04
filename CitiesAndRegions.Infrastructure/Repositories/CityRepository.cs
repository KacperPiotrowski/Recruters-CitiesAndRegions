using System.Diagnostics;
using CitiesAndRegions.Domain.Entities;
using CitiesAndRegions.Domain.Exceptions;
using CitiesAndRegions.Domain.Filtering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace CitiesAndRegions.Infrastructure.Repositories;

public sealed class CityRepository : ICityRepository
{
    private readonly RegionContext _regionContext;
    private readonly ILogger<CityRepository> _logger;

    public CityRepository(RegionContext regionContext, ILogger<CityRepository> logger)
    {
        _regionContext = regionContext;
        _logger = logger;
    }

    public async Task<uint> AddNewAsync(CityEntity newCity, CancellationToken cancellationToken = default)
    {
        Debug.Assert(newCity is not null);

        if (!await _regionContext.Countries.AnyAsync(x => x.Id == newCity.Country.Id, cancellationToken))
            CountryNotFoundApiException.ThrowIfNotFound(newCity.Country.Id);

        newCity.Country = await _regionContext.Countries.FirstAsync(x => x.Id == newCity.Country.Id, cancellationToken);

        await _regionContext.Cities.AddAsync(newCity, cancellationToken);

        await _regionContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("Successfully added new city with id {id}.", newCity.Id);

        return newCity.Id;
    }

    public async Task<IEnumerable<uint>> AddRangeAsync(IEnumerable<CityEntity> newCites, CancellationToken cancellationToken = default)
    {
        Debug.Assert(newCites is not null);

        await _regionContext.Cities.AddRangeAsync(newCites, cancellationToken);

        await _regionContext.SaveChangesAsync(cancellationToken);

        IEnumerable<uint> newIds = newCites.Select(x => x.Id);

        _logger.LogInformation("Successfully added new cities with ids {ids}.", newIds);

        return newIds;
    }

    public async Task<IEnumerable<CityEntity>> GetAllByCountryAsync(string countryName,
                                                                    Filter filter,
                                                                    CancellationToken cancellationToken = default)
    {
        IQueryable<CityEntity> query = _regionContext.Cities.AsNoTracking()
                                                               .AsSingleQuery()
                                                               .Where(x => x.Country.Name == countryName);

        if (filter.CountOfResult.HasValue)
            query = query.Take(..filter.CountOfResult.Value);
        if (filter.CityName != null && filter.CityName.Length != 0)
            query = query.Where(x => x.Name.Contains(filter.CityName));
        if (filter.CityPopulation.HasValue)
            query = query.Where(x => x.Population > filter.CityPopulation.Value);

        List<CityEntity> result = await query.ToListAsync(cancellationToken)
                                             .ConfigureAwait(false);

        _logger.LogInformation("For region {region} exists {count} cities.", countryName, result.Count);

        return result;
    }

    public async Task<CityEntity> GetByIdAsync(uint id, CancellationToken cancellationToken = default)
    {
        CityEntity result = await (from city in _regionContext.Cities
                                   join country in _regionContext.Countries on city.Country.Id equals country.Id
                                   select new CityEntity
                                   {
                                       Id = city.Id,
                                       Country = new()
                                       {
                                           Id = country.Id,
                                           Name = country.Name
                                       },
                                       Name = city.Name,
                                       Population = city.Population
                                   })
                                   .AsNoTracking()
                                   .FirstOrDefaultAsync(x => x.Id == id, cancellationToken)
                                   .ConfigureAwait(false);

        _logger.LogInformation("For id {id}, is there a result: {isNull}.", id, result is null);

        return result;
    }

    public async Task UpdatePopulation(uint cityId, ulong newPopulation, CancellationToken cancellationToken = default)
    {
        CityEntity city = await _regionContext.Cities.FirstOrDefaultAsync(x => x.Id == cityId, cancellationToken);
        city.Population = newPopulation;

        await _regionContext.SaveChangesAsync(cancellationToken);
    }
}
