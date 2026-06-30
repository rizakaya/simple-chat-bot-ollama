using QwenOllama.ConsoleMemoryLab.Models;

namespace QwenOllama.ConsoleMemoryLab.Services;

public sealed class MemoryManager
{
    public MemoryManager(int maxMessages)
    {
        if (maxMessages < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maxMessages), "MaxMessages must be greater than zero.");
        }

        MaxMessages = maxMessages;
    }

    public int MaxMessages { get; }

    public void Trim(List<ChatMessage> messages)
    {
        if (messages.Count == 0)
        {
            return;
        }

        var systemMessage = messages.FirstOrDefault(message => message.Role == "system");
        var conversationMessages = messages
            .Where(message => message.Role != "system")
            .TakeLast(MaxMessages)
            .ToList();

        messages.Clear();

        if (systemMessage is not null)
        {
            messages.Add(systemMessage);
        }

        messages.AddRange(conversationMessages);
    }
}
