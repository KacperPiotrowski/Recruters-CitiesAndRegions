using CitiesAndRegions.Domain;
using CitiesAndRegions.Domain.Filtering;

namespace CitiesAndRegions.Application.Services;

public interface ICityService
{
    Task<CityDTO> GetByIdAsync(uint id, CancellationToken cancellationToken);
    Task<uint> AddNewAsync(CityDTO newCity, CancellationToken cancellationToken);
    Task<IEnumerable<uint>> AddRangeAsync(IEnumerable<CityDTO> cityDTOs, bool addOnlyValid, CancellationToken cancellationToken);
    Task<IEnumerable<CityDTO>> GetAllByRegionAsync(string region, Filter filter, CancellationToken cancellationToken);
    Task UpdatePopulation(uint cityId, ulong newPopulation, CancellationToken cancellationToken);
}
