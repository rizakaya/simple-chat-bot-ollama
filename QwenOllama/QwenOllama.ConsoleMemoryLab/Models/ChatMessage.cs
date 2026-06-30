using System.Text.Json.Serialization;

namespace QwenOllama.ConsoleMemoryLab.Models;

public sealed class ChatMessage
{
    public ChatMessage()
    {
    }

    public ChatMessage(string role, string content)
    {
        Role = role;
        Content = content;
    }

    [JsonPropertyName("role")]
    public string Role { get; set; } = string.Empty;

    [JsonPropertyName("content")]
    public string Content { get; set; } = string.Empty;

    [JsonIgnore]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
