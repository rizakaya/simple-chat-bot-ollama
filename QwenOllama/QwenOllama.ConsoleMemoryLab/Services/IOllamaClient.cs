using QwenOllama.ConsoleMemoryLab.Models;

namespace QwenOllama.ConsoleMemoryLab.Services;

public interface IOllamaClient
{
    Task<string> ChatAsync(IReadOnlyList<ChatMessage> messages, CancellationToken cancellationToken = default);

    IAsyncEnumerable<string> ChatStreamAsync(
        IReadOnlyList<ChatMessage> messages,
        CancellationToken cancellationToken = default);
}
