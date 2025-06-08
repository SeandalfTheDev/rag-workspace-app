### Create Migrations

- dotnet ef migrations add MIGRATIONNAME --project CleverDocs.Infrastructure --startup-project CleverDocs.WebApi

- dotnet ef database update --project CleverDocs.Infrastructure --startup-project CleverDocs.WebApi