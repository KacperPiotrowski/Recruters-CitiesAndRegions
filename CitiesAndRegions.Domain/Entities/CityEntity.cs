using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CitiesAndRegions.Domain.Entities;

public sealed class CityEntity : IEntity<uint>, IEntityTypeConfiguration<CityEntity>
{
    public uint Id { get; set; }
    public string Name { get; set; }
    public ulong Population { get; set; }
    public CountryEntity Country { get; set; }

    public void Configure(EntityTypeBuilder<CityEntity> builder)
    {
        builder.ToTable("cities");
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
               .ValueGeneratedOnAdd();

        builder.Property(x => x.Name)
               .IsRequired();

        builder.Property(x => x.Population)
               .IsRequired();

        builder.HasOne(x => x.Country)
               .WithMany(x => x.Cities)
               .IsRequired();
    }
}
