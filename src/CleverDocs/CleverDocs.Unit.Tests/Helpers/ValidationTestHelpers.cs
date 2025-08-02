using CleverDocs.Application.Auth.DTOs;

namespace CleverDocs.Unit.Tests.Helpers;

public static class ValidationTestHelpers
{
    public static LoginUserDto ValidLoginDto() => new()
    {
        Email = "test@example.com",
        Password = "ValidPass123"
    };
    
    public static LoginUserDto LoginWithEmptyEmail() => new()
    {
        Email = "",
        Password = "ValidPass123"
    };
    
    public static LoginUserDto LoginWithInvalidEmail() => new()
    {
        Email = "invalid-email",
        Password = "ValidPass123"
    };
    
    public static LoginUserDto LoginWithLongEmail() => new()
    {
        Email = new string('a', 250) + "@example.com",
        Password = "ValidPass123"
    };

    public static LoginUserDto LoginWithLongPassword() => new()
    {
        Email = "test@example.com",
        Password = new string('a', 101)
    };

    public static RegisterUserDto ValidRegisterDto() => new()
    {
        Email = "test@eaxample.com",
        FirstName = "John",
        LastName = "Doe",
        Password = "ValidPass123",
        ConfirmPassword = "ValidPass123"
    };

    public static RegisterUserDto RegisterWithLongFields() => new()
    {
        Email = new string('a', 250) + "@example.com",
        FirstName = new string('a', 101),
        LastName = new string('b', 101),
        Password = new string('c', 101),
        ConfirmPassword = new string('c', 101)
    };

    public static RegisterUserDto RegisterWithMismatchedPasswords() => new()
    {
        Email = "test@example.com",
        FirstName = "John",
        LastName = "Doe",
        Password = "Password123",
        ConfirmPassword = "DifferentPassword123"
    };
}