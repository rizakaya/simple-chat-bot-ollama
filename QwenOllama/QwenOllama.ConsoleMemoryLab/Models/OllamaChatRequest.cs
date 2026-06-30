using System.Text.Json.Serialization;

namespace QwenOllama.ConsoleMemoryLab.Models;

public sealed class OllamaChatRequest
{
    [JsonPropertyName("model")]
    public string Model { get; set; } = string.Empty;

    [JsonPropertyName("messages")]
    public IReadOnlyList<ChatMessage> Messages { get; set; } = Array.Empty<ChatMessage>();

    [JsonPropertyName("stream")]
    public bool Stream { get; set; }
}
