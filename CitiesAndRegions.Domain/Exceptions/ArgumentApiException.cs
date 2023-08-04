using System.Net;

namespace CitiesAndRegions.Domain.Exceptions;

public class ArgumentApiException : ApiException
{
    private ArgumentApiException()
    {
        Code = HttpStatusCode.BadRequest;
    }

    private ArgumentApiException(string message):this()
    {
        Error = message;
    }

    public static void ThrowIfZero(int value)
    {
        if (value == 0)
        {
            throw new ArgumentApiException("Value must not be zero.");
        }
    }

    public static void ThrowIfNullOrEmpty(string value)
    {
        if(string.IsNullOrEmpty(value))
        {
            throw new ArgumentApiException("Value must have a value and cannot be empty.");
        }
    }
}