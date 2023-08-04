/*
 * dotnet ef migrations add FixColumns --project ./CitiesAndRegions.Infrastructure -s ./CitiesAndRegions.Api -c RegionContext --verbose
 * 
 * dotnet ef migrations add CreateDatabase --project ./CitiesAndRegions.Infrastructure -s ./CitiesAndRegions.Api -c RegionContext --verbose
 * dotnet ef migrations remove CreateDatabase --project ./CitiesAndRegions.Infrastructure -s ./CitiesAndRegions.Api -c RegionContext --verbose
 */

/**
 * TODO:
 * 3. Clear Program.cs
 * 4. Connection to many DBs
 * 5. Fix bugs (if any)
 * 6. Finish middlewares
 * 7. Logging to a file/server e.g. aws
 * 8. Tests
*/

using CitiesAndRegions.Application.Services;
using CitiesAndRegions.Domain.Mappers;
using CitiesAndRegions.Infrastructure;
using CitiesAndRegions.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using CitiesAndRegions.Infrastructure.Middlewares;
using CitiesAndRegions.Infrastructure.Hubs;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
    .Build();
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSignalR();
builder.Services.AddTransient<ICityService, CityService>();
builder.Services.AddTransient<ICityRepository, CityRepository>();
builder.Services.AddDbContext<RegionContext>(options =>
{
    //TODO
    //options.UseSqlServer("Server=127.0.0.1;Database=Region;Trusted_Connection=True;", opt => opt.MigrationsAssembly(typeof(Program).Assembly.FullName));

    //for developing
    options.UseSqlite(config.GetValue<string>("connectionString"));
});

builder.Services.AddAutoMapper(cfg =>
{
    cfg.AddProfile<CityMapping>();
});
builder.Services.AddLogging(cfg =>
{
    cfg.AddConsole();
    cfg.AddDebug();
});
builder.Services.AddSwaggerGen(cfg =>
{
    cfg.SwaggerDoc("v1", new()
    {
        Title = "CitiesAndRegions",
        Version = "v1"
    });
    cfg.AddSignalRSwaggerGen(ssgOptions => ssgOptions.ScanAssemblies(typeof(CityHub).Assembly));
});
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Cache());
});

WebApplication app = builder.Build();

app.UseCors();
// move to extensions
app.MapHub<CityHub>("/city");

app.AddMiddlewares();
app.UseOutputCache();
app.UseCors(cfg =>
{
    cfg.AllowAnyOrigin()
       .AllowAnyMethod()
       .AllowAnyHeader();
});
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

app.MapControllers();

using var scope = app.Services.CreateScope();
await using var dbContext = scope.ServiceProvider.GetRequiredService<RegionContext>();
await dbContext.Database.MigrateAsync();

app.Run();
