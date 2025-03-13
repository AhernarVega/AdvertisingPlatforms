using System.Net;

namespace AdvertisingPlatforms.Presentation.Dtos.Responses;

public record ExceptionResponse
{
    public HttpStatusCode StatusCode { get; set; }
    public string Description { get; set; }

    public ExceptionResponse(HttpStatusCode statusCode, string description)
    {
        StatusCode = statusCode;
        Description = description;
    }
}