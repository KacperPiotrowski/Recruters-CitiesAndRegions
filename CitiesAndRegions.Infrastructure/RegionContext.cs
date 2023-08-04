using Microsoft.EntityFrameworkCore;
using System.Reflection;
using CitiesAndRegions.Domain.Entities;

namespace CitiesAndRegions.Infrastructure;

public sealed class RegionContext : DbContext
{
	public DbSet<CityEntity> Cities { get; set; }
	public DbSet<CountryEntity> Countries { get; set; }

    public RegionContext(DbContextOptions<RegionContext> options) : base(options)
    {
    }

    public RegionContext() : base()
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var typesToRegister = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.GetInterfaces().Any(gi => gi.IsGenericType && gi.GetGenericTypeDefinition() == typeof(IEntityTypeConfiguration<>)))
            .ToList();

        foreach (var type in typesToRegister)
        {
            dynamic instance = Activator.CreateInstance(type);
            modelBuilder.ApplyConfiguration(instance);
        }

        base.OnModelCreating(modelBuilder);
    }
}