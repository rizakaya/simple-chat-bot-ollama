using QwenOllama.ConsoleMemoryLab.Models;

namespace QwenOllama.ConsoleMemoryLab.Services;

public interface IConversationMemory
{
    void AddUserMessage(string content);

    void AddAssistantMessage(string content);

    IReadOnlyList<ChatMessage> GetMessages();

    void Clear();
}
