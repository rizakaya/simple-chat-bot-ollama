using System.Runtime.CompilerServices;
using System.Net.Http.Json;
using System.Text.Json;
using QwenOllama.ConsoleMemoryLab.Infrastructure;
using QwenOllama.ConsoleMemoryLab.Models;

namespace QwenOllama.ConsoleMemoryLab.Services;

public sealed class OllamaClient : IOllamaClient
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly HttpClient _httpClient;
    private readonly OllamaSettings _settings;

    public OllamaClient(HttpClient httpClient, OllamaSettings settings)
    {
        _httpClient = httpClient;
        _settings = settings;
        _httpClient.BaseAddress = new Uri(settings.BaseUrl.TrimEnd('/') + "/");
    }

    public async Task<string> ChatAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(messages, stream: false);
        using var response = await _httpClient.PostAsJsonAsync("api/chat", request, JsonOptions, cancellationToken);
        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<OllamaChatResponse>(JsonOptions, cancellationToken);
        return body?.Message?.Content ?? string.Empty;
    }

    public async IAsyncEnumerable<string> ChatStreamAsync(
        IReadOnlyList<ChatMessage> messages,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var request = CreateRequest(messages, stream: true);
        using var httpRequest = new HttpRequestMessage(HttpMethod.Post, "api/chat")
        {
            Content = JsonContent.Create(request, options: JsonOptions)
        };
        using var response = await _httpClient.SendAsync(
            httpRequest,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        await foreach (var line in JsonStreamReader.ReadLinesAsync(stream, cancellationToken))
        {
            var chunk = JsonSerializer.Deserialize<OllamaChatResponse>(line, JsonOptions);
            var content = chunk?.Message?.Content;

            if (!string.IsNullOrEmpty(content))
            {
                yield return content;
            }
        }
    }

    private OllamaChatRequest CreateRequest(IReadOnlyList<ChatMessage> messages, bool stream)
    {
        return new OllamaChatRequest
        {
            Model = _settings.Model,
            Messages = messages,
            Stream = stream
        };
    }
}
