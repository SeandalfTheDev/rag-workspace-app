using CleverDocs.Application.Auth.DTOs;
using CleverDocs.Domain.Auth;
using Riok.Mapperly.Abstractions;

namespace CleverDocs.Application.Auth.Mapping;

[Mapper]
public partial class AuthMapper
{
    [MapperIgnoreSource(nameof(RegisterUserDto.Password))]
    [MapperIgnoreSource(nameof(RegisterUserDto.ConfirmPassword))]
    [MapperIgnoreTarget(nameof(User.CreatedAtUtc))]
    [MapperIgnoreTarget(nameof(User.UpdatedAtUtc))]
    [MapperIgnoreTarget(nameof(User.IdentityId))]
    [MapValue(nameof(User.Id), Use = nameof(NewId))]
    public partial User MapRegisterDtoToUser(RegisterUserDto registerUserDto);

    string NewId() => User.NewId();
}