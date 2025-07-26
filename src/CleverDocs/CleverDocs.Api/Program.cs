using CleverDocs.Api;
using CleverDocs.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddDatabase();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    await app.ApplyMigrationsAsync();
}

app.UseHttpsRedirection();

app.Run();
