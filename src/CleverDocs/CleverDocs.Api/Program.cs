using CleverDocs.Api;
using CleverDocs.Api.Auth;
using CleverDocs.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabase();
builder.AddAuthServices();
builder.AddApplicationServices();
builder.AddValidators();

builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();
    await app.SeedInitialDataAsync();
}

app.MapAuthenticationEndpoints();

app.UseHttpsRedirection();

app.Run();


public partial class Program { }
