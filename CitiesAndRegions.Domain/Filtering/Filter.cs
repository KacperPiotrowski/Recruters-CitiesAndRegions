namespace CitiesAndRegions.Domain.Filtering;

public class Filter
{
    public string CityName { get; set; }
    public ulong? CityPopulation { get; set; }
    public int? CountOfResult { get; set; }
}