using AutoMapper;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using CitiesAndRegions.Domain;
using CitiesAndRegions.Domain.Entities;
using CitiesAndRegions.Infrastructure.Repositories;
using CitiesAndRegions.Domain.Filtering;
using CitiesAndRegions.Domain.Exceptions;
using Microsoft.AspNetCore.SignalR;
using CitiesAndRegions.Infrastructure.Hubs;

namespace CitiesAndRegions.Application.Services;

public sealed class CityService : ICityService
{
    private readonly ICityRepository _cityRepository;
    private readonly IMapper _mapper;
    private readonly IHubContext<CityHub> _context;

    public CityService(ICityRepository cityRepository,
                       IMapper mapper,
                       IHubContext<CityHub> context)
    {
        _cityRepository = cityRepository;
        _mapper = mapper;
        _context = context;
    }

    public Task<uint> AddNewAsync(CityDTO newCity, CancellationToken cancellationToken = default)
    {
        Debug.Assert(newCity is not null);

        // validation
        ArgumentApiException.ThrowIfNullOrEmpty(newCity.Name);
        ArgumentApiException.ThrowIfNullOrEmpty(newCity.Country);

        // action
        CityEntity mapped = _mapper.Map<CityDTO, CityEntity>(newCity);
        return _cityRepository.AddNewAsync(mapped, cancellationToken);
    }

    public Task<IEnumerable<uint>> AddRangeAsync(IEnumerable<CityDTO> cityDTOs,
                                                 bool addOnlyValid,
                                                 CancellationToken cancellationToken = default)
    {
        CityDTO first = cityDTOs.First();

        ref var start = ref first;
        ref CityDTO end = ref Unsafe.Add(ref start, cityDTOs.TryGetNonEnumeratedCount(out int nonEnumertedCount) ? nonEnumertedCount : cityDTOs.Count());
        IEnumerable<CityEntity> toAdd = Enumerable.Empty<CityEntity>();

        if (addOnlyValid)
        {
            while (Unsafe.IsAddressLessThan(ref start, ref end))
            {
                if (!string.IsNullOrEmpty(start.Name) && !string.IsNullOrEmpty(start.Country))
                {
                    CityEntity mapped = _mapper.Map<CityDTO, CityEntity>(start);
                    toAdd = toAdd.Append(mapped);
                }
                start = ref Unsafe.Add(ref start, 1);
            }
            return _cityRepository.AddRangeAsync(toAdd, cancellationToken);
        }

        while (Unsafe.IsAddressLessThan(ref start, ref end))
        {
            ArgumentApiException.ThrowIfNullOrEmpty(start.Name);
            ArgumentApiException.ThrowIfNullOrEmpty(start.Country);

            CityEntity mapped = _mapper.Map<CityDTO, CityEntity>(start);
            toAdd = toAdd.Append(mapped);

            start = ref Unsafe.Add(ref start, 1);
        }
        return _cityRepository.AddRangeAsync(toAdd, cancellationToken);
    }

    public async Task<IEnumerable<CityDTO>> GetAllByRegionAsync(string region,
                                                                      Filter filter,
                                                                      CancellationToken cancellationToken = default)
    {
        // validation
        ArgumentApiException.ThrowIfNullOrEmpty(region);

        // action
        IEnumerable<CityEntity> all = await _cityRepository.GetAllByCountryAsync(region, filter, cancellationToken);
        return _mapper.Map<IEnumerable<CityEntity>, IEnumerable<CityDTO>>(all);
    }

    public async Task<CityDTO> GetByIdAsync(uint id, CancellationToken cancellationToken = default)
    {        
        // action
        CityEntity city = await _cityRepository.GetByIdAsync(id, cancellationToken);
        return _mapper.Map<CityEntity, CityDTO>(city);
    }

    public async Task UpdatePopulation(uint cityId, ulong newPopulation, CancellationToken cancellationToken)
    {
        await _cityRepository.UpdatePopulation(cityId, newPopulation, cancellationToken);
        await _context.Clients.All.SendAsync("SendUpdatedPopulation", cityId, newPopulation, cancellationToken);
    }
}