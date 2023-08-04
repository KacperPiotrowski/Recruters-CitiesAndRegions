using AutoMapper;
using CitiesAndRegions.Domain.Entities;

namespace CitiesAndRegions.Domain.Mappers;

public sealed class CityMapping : Profile
{
    public CityMapping()
    {
        CreateMap<CityEntity, CityDTO>()
            .ForMember(dest => dest.Country,
                       opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.CountryId,
                       opt => opt.MapFrom(src => src.Country.Id));

        CreateMap<CityDTO, CityEntity>()
            .ForMember(dest => dest.Country,
                       opt => opt.MapFrom(src => new CountryEntity
                       {
                           Name = src.Country,
                           Id = src.CountryId
                       }));
    }
}
