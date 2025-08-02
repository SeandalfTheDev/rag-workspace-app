using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Application.Auth.Interfaces;
using CleverDocs.Application.Auth.Mapping;
using CleverDocs.Domain.Auth;
using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;

namespace CleverDocs.Application.Auth.Services;

public class AuthService(        
    UserManager<IdentityUser> userManager,
    AppIdentityDbContext identityDbContext,
    AppDbContext applicationDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> jwtAuthOptions) : IAuthService
{
    public async Task<AuthResult<AccessTokensDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        var authMapper = new AuthMapper();
        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync();
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction());

        try
        {
            // Create an identity user
            var identityUser = new IdentityUser
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.Email
            };

            // Create the user in the identity system
            var createUserResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                return AuthResult<AccessTokensDto>.Failure(
                    "Unable to register user, please try again",
                    errors,
                    AuthErrorType.Validation);
            }

            // Add a user to a role
            var addToRoleResult = await userManager.AddToRoleAsync(identityUser, Roles.AppUser);
            if (!addToRoleResult.Succeeded)
            {
                var errors = addToRoleResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                return AuthResult<AccessTokensDto>.Failure(
                    "Unable to assign role to user, please try again",
                    errors,
                    AuthErrorType.ServerError);
            }

            // Create an application user
            var user = authMapper.MapRegisterDtoToUser(registerUserDto);
            user.IdentityId = identityUser.Id;
            
            applicationDbContext.Users.Add(user);
            await applicationDbContext.SaveChangesAsync();

            // Generate tokens
            var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email, [Roles.AppUser]);
            AccessTokensDto accessTokens = tokenProvider.Create(tokenRequest);
            
            var refreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = identityUser.Id,
                Token = accessTokens.RefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.Value.RefreshTokenExpirationDays)
            };
            identityDbContext.RefreshTokens.Add(refreshToken);

            await identityDbContext.SaveChangesAsync();

            // Commit transaction if everything succeeded
            await transaction.CommitAsync();

            return AuthResult<AccessTokensDto>.Success(accessTokens);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return AuthResult<AccessTokensDto>.Failure(
                "An error occurred while registering the user",
                null);
        }
    }

    public async Task<AuthResult<AccessTokensDto>> LoginUserAsync(LoginUserDto loginUserDto)
    {
        var authMapper = new AuthMapper();

        try
        {
            var identityUser = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (identityUser == null)
            {
                return AuthResult<AccessTokensDto>.Failure(
                    "Invalid email or password",
                    new Dictionary<string, string[]>
                    {
                        ["email"] = new[] { "Invalid email or password" }
                    },
                    AuthErrorType.Validation);
            }
            
            var roles = await userManager.GetRolesAsync(identityUser);

            var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, roles);
            var accessTokens = tokenProvider.Create(tokenRequest);

            var refreshToken = new RefreshToken
            {
                Id = Guid.CreateVersion7(),
                UserId = identityUser.Id,
                Token = accessTokens.RefreshToken,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.Value.RefreshTokenExpirationDays)
            };
            identityDbContext.RefreshTokens.Add(refreshToken);

            await identityDbContext.SaveChangesAsync();
            
            var authResult = AuthResult<AccessTokensDto>.Success(accessTokens);
            return authResult;
        }
        catch (Exception ex)
        {
            return AuthResult<AccessTokensDto>.Failure(
                "An error occurred while logging in the user",
                null);
        }
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await identityDbContext.Users.AnyAsync(u => u.Email == email);
    }
}