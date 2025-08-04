using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Application.Auth.Interfaces;
using CleverDocs.Application.Auth.Mapping;
using CleverDocs.Domain.Auth;
using CleverDocs.Domain.Errors;
using CleverDocs.Domain.Shared;
using CleverDocs.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace CleverDocs.Application.Auth.Services;

public class AuthService(        
    UserManager<IdentityUser> userManager,
    AppIdentityDbContext identityDbContext,
    AppDbContext applicationDbContext,
    TokenProvider tokenProvider,
    IOptions<JwtAuthOptions> jwtAuthOptions) : IAuthService
{
    public async Task<AppResult<AccessTokensDto>> RegisterUserAsync(RegisterUserDto registerUserDto)
    {
        try
        {
            var authMapper = new AuthMapper();
            
            var identityUser = new IdentityUser
            {
                Email = registerUserDto.Email,
                UserName = registerUserDto.Email
            };
            
            var createUserResult = await userManager.CreateAsync(identityUser, registerUserDto.Password);
            if (!createUserResult.Succeeded)
            {
                var authError = new AppError(
                    "Identity.Failure",
                    "An error occurred while creating the user");
                
                return await Task.FromResult<AppResult<AccessTokensDto>>(authError);
            }
            
            var addToRoleResult = await userManager.AddToRoleAsync(identityUser, Roles.AppUser);
            if (!addToRoleResult.Succeeded)
            {
                var errors = addToRoleResult.Errors.ToDictionary(e => e.Code, e => new[] { e.Description });
                var authError = new AppError(
                    "IdentityRole.Failure",
                    "An error occurred while adding the user to the 'AppUser' role");
            }
            
            var user = authMapper.MapRegisterDtoToUser(registerUserDto);
            user.IdentityId = identityUser.Id;
            
            applicationDbContext.Users.Add(user);
            await applicationDbContext.SaveChangesAsync();
            
            var tokenRequest = new TokenRequest(identityUser.Id, identityUser.Email!, [Roles.AppUser]);
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
            
            return await Task.FromResult<AppResult<AccessTokensDto>>(accessTokens);
        }
        catch (Exception ex)
        {
            var authError = new AppError(
                "Identity.Failure",
                "An error occurred while registering the user");
            
            return await Task.FromResult<AppResult<AccessTokensDto>>(authError);
        }
    }

    public async Task<AppResult<AccessTokensDto>> LoginUserAsync(LoginUserDto loginUserDto)
    {
        try
        {
            var identityUser = await userManager.FindByEmailAsync(loginUserDto.Email);
            if (identityUser == null)
            {
                return await Task.FromResult<AppResult<AccessTokensDto>>(CommonErrors.NotFound);
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
            
            return await Task.FromResult<AppResult<AccessTokensDto>>(accessTokens);
        }
        catch (Exception ex)
        {
            var authError = new AppError(
                "Identity.Failure",
                "An error occurred while logging in the user");
            
            return await Task.FromResult<AppResult<AccessTokensDto>>(authError);
        }
    }

    public async Task<AppResult<AccessTokensDto>> RefreshTokenAsync(RefreshTokenDto refreshTokenDto)
    {
        var refreshToken = await identityDbContext.RefreshTokens
            .Include(rt => rt.User)
            .FirstOrDefaultAsync(rt => rt.Token == refreshTokenDto.RefreshToken);

        if (refreshToken == null)
        {
            return await Task.FromResult<AppResult<AccessTokensDto>>(CommonErrors.NotFound);
        }

        if (refreshToken.ExpiresAtUtc < DateTime.UtcNow)
        {
            var authError = new AppError(
                "Identity.Failure",
                "The refresh token has expired");
            return await Task.FromResult<AppResult<AccessTokensDto>>(authError);
        }
        
        var roles = await userManager.GetRolesAsync(refreshToken.User);
        
        var tokenRequest = new TokenRequest(refreshToken.User.Id, refreshToken.User.Email!, roles);
        var accessTokens = tokenProvider.Create(tokenRequest);

        refreshToken.Token = accessTokens.RefreshToken;
        refreshToken.ExpiresAtUtc = DateTime.UtcNow.AddDays(jwtAuthOptions.Value.RefreshTokenExpirationDays);

        await identityDbContext.SaveChangesAsync();
        
        return await Task.FromResult<AppResult<AccessTokensDto>>(accessTokens);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await identityDbContext.Users.AnyAsync(u => u.Email == email);
    }
}