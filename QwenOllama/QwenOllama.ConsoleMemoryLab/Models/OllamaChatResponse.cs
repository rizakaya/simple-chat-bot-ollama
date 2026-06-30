using System.Text.Json.Serialization;

namespace QwenOllama.ConsoleMemoryLab.Models;

public sealed class OllamaChatResponse
{
    [JsonPropertyName("message")]
    public ChatMessage? Message { get; set; }

    [JsonPropertyName("done")]
    public bool Done { get; set; }
}
