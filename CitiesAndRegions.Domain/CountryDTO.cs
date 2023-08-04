namespace CitiesAndRegions.Domain;

public sealed class CountryDTO
{
    public ulong Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<CityDTO> Cities { get; set; }
}
