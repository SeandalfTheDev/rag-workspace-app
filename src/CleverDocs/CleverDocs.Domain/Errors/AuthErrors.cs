using CleverDocs.Domain.Shared;

namespace CleverDocs.Domain.Errors;

public static class AuthErrors
{
    public static class User
    {
        public static readonly AppError EmailInUse =
            new("User.EmailInUse", "The specified email is already in use.");
    }
}