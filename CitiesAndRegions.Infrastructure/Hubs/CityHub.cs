using Microsoft.AspNetCore.SignalR;
using SignalRSwaggerGen.Attributes;

namespace CitiesAndRegions.Infrastructure.Hubs;

[SignalRHub]
public interface ICityHub
{
    Task SendUpdatedPopulation(uint cityId, ulong population, [SignalRHidden] CancellationToken cancellationToken);
}

public sealed class CityHub : Hub
{
    //[HubMethodName("SendUpdatedPopulation")]
    //public Task Send(uint cityId, ulong population, CancellationToken cancellationToken = default)
    //{
    //    return Clients.All.SendAsync("SendUpdatedPopulation", cityId, population, cancellationToken);
    //}
}
