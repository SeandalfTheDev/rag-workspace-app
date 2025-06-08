using CleverDocs.Core.Abstractions.Authentication;
using CleverDocs.Core.Abstractions.Repositories;
using CleverDocs.Core.Entities;
using CleverDocs.Shared.Abstractions;
using CleverDocs.Shared.Authentication.Dtos;
using CleverDocs.Shared.Authentication.Models;

namespace CleverDocs.Infrastructure.Authentication;

public class AuthService(
    IUserRepository userRepository,
    ITokenService tokenService,
    IPasswordService passwordService,
    Serilog.ILogger logger
) : IAuthService
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await userRepository.GetUserByEmailAsync(email);
    }

    public async Task<Result<AuthResponse>> LoginAsync(LoginRequest request)
    {
        var user = await userRepository.GetUserByEmailAsync(request.Email);
        if (user is null)
        {
            return Result<AuthResponse>.Failure("User not found");
        }

        if (!passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            return Result<AuthResponse>.Failure("Invalid password");
        }

        var token = tokenService.GenerateAccessToken(user);
        var refreshToken = tokenService.GenerateRefreshToken();

        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName
        };

        return Result<AuthResponse>.Success(new AuthResponse
        {
            User = userDto,
            Token = token,
            RefreshToken = refreshToken
        });
    }

    public async Task<Result<AuthResponse>> RegisterAsync(RegisterRequest request)
    {
        var existingUser = await userRepository.GetUserByEmailAsync(request.Email);
        if (existingUser is not null)
        {
            return Result<AuthResponse>.Failure("User already exists");
        }

        var hashedPassword = passwordService.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        try
        {
            await userRepository.CreateUserAsync(user);

            var token = tokenService.GenerateAccessToken(user);
            var refreshToken = tokenService.GenerateRefreshToken();

            var userDto = new UserDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName
            };

            return Result<AuthResponse>.Success(new AuthResponse
                {
                    User = userDto,
                    Token = token,
                    RefreshToken = refreshToken
                });
            }
        catch (Exception ex)
        {
            logger.Error(ex, "Error registering user");
            return Result<AuthResponse>.Failure(ex.Message);
        }
    }
}