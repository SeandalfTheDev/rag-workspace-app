using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System.Text.Json;

namespace CleverDocs.Infrastructure.Data.Converters;

public class DictionaryJsonConverter : ValueConverter<Dictionary<string, string>, string>
{
    public DictionaryJsonConverter() : base(
        v => JsonSerializer.Serialize(v, JsonOptions),
        v => JsonSerializer.Deserialize<Dictionary<string, string>>(v, JsonOptions) ?? new Dictionary<string, string>())
    {
    }

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
} 