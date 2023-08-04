namespace CitiesAndRegions.Domain.Entities;

public interface IEntity<TId> where TId: unmanaged
{
    TId Id { get; set; }
}
