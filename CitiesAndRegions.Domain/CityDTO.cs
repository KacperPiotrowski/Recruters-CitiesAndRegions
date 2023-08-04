namespace CitiesAndRegions.Domain;

public sealed class CityDTO
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public ulong Population { get; set; }
    public string Country { get; set; }
    public uint CountryId { get; set; } 
}