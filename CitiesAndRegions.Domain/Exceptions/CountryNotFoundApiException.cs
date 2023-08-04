namespace CitiesAndRegions.Domain.Exceptions;

public class CountryNotFoundApiException : ApiException
{
    private CountryNotFoundApiException(string message)
    {
        Error = message;
    }

    public static void ThrowIfNotFound(uint id)
    {
        throw new CountryNotFoundApiException($"Country with id: {id} not found");
    }
}

