using CitiesAndRegions.Domain.Entities;
using CitiesAndRegions.Domain.Filtering;

namespace CitiesAndRegions.Infrastructure.Repositories;

public interface ICityRepository
{
    Task<uint> AddNewAsync(CityEntity newCity, CancellationToken cancellationToken);
    Task<IEnumerable<uint>> AddRangeAsync(IEnumerable<CityEntity> newCity, CancellationToken cancellationToken);
    Task<IEnumerable<CityEntity>> GetAllByCountryAsync(string region, Filter filter, CancellationToken cancellationToken);
    Task<CityEntity> GetByIdAsync(uint id, CancellationToken cancellationToken);
    Task UpdatePopulation(uint cityId, ulong newPopulation, CancellationToken cancellationToken);
}
