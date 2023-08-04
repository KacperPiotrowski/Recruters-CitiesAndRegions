using System.Net;

namespace CitiesAndRegions.Domain.Exceptions;

public class ApiException : Exception
{
    public string Error { get; protected init; }
    public HttpStatusCode Code { get; protected init; }

    protected ApiException() { }
}
