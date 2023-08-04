using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitiesAndRegions.Domain.Entities;

public sealed class CountryEntity : IEntity<uint>, IEntityTypeConfiguration<CountryEntity>
{
    public uint Id { get; set; }// required for migrations
    public string Name { get; set; }
    public ICollection<CityEntity> Cities { get; set; }

    public void Configure(EntityTypeBuilder<CountryEntity> builder)
    {
        builder.ToTable("countries");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired();

        builder.HasMany(regionEntity => regionEntity.Cities)
               .WithOne(city => city.Country)
               .IsRequired();

    }
}