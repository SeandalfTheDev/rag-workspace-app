using CleverDocs.Domain.Shared;

namespace CleverDocs.Domain.Errors;

public static class CommonErrors
{
    public static readonly AppError NotFound = new(
        "Entity.NotFound",
        "The requested entity was not found.");
    
    public static readonly AppError UnprocessableRequest = new(
        "Request.Unprocessable",
        "The server could not process the request due to a validation error.");
    
    public static readonly AppError InternalServerError = new(
        "Server.Internal",
        "The server encountered an unexpected error.");
}