using System.Net.Http.Json;
using System.Text.Json.Serialization;
using CleverDocs.Core.Abstractions.Documents;
using Microsoft.Extensions.Configuration;

namespace CleverDocs.Infrastructure.Documents;

public class EmbeddingService: IEmbeddingService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _config;
    private readonly Serilog.ILogger _logger;

    public EmbeddingService(IConfiguration config, Serilog.ILogger logger)
    {
        _httpClient = new HttpClient();
        _config = config;
        _logger = logger;
        
        _httpClient.BaseAddress = new Uri(_config["Ollama:BaseUrl"]!);
    }
    public async Task<List<float[]>> GenerateEmbeddingsAsync(List<string> chunks, CancellationToken token)
    {
        var allEmbeddings = new List<float[]>();
        
        try
        {
            const int batchSize = 10;
            
            for (int i = 0; i < chunks.Count; i += batchSize)
            {
                var batch = chunks.Skip(i).Take(batchSize).ToList();
                foreach (var text in batch)
                {
                    var request = new
                    {
                        model = _config["Ollama:EmbeddingModel"],
                        prompt = text
                    };

                    var response = await _httpClient.PostAsJsonAsync(
                        "/api/embeddings", request, token);
                    response.EnsureSuccessStatusCode();

                    var result = await response.Content
                        .ReadFromJsonAsync<OllamaEmbeddingResponse>(cancellationToken: token);
                
                    allEmbeddings.Add(result!.Embedding);
                }
            }
            
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Failed to generate embeddings");
        }
        
        return allEmbeddings;
    }
}

sealed class OllamaEmbeddingResponse {
    [JsonPropertyName("embedding")]
    public float[] Embedding { get; set; }
}