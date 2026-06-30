using QwenOllama.ConsoleMemoryLab.Models;

namespace QwenOllama.ConsoleMemoryLab.Services;

public sealed class ConversationMemory : IConversationMemory
{
    private readonly MemoryManager _memoryManager;
    private readonly ChatMessage _systemMessage;
    private readonly List<ChatMessage> _messages;

    public ConversationMemory(string systemPrompt, MemoryManager memoryManager)
    {
        _memoryManager = memoryManager;
        _systemMessage = new ChatMessage("system", systemPrompt);
        _messages = [_systemMessage];
    }

    public void AddUserMessage(string content)
    {
        AddMessage("user", content);
    }

    public void AddAssistantMessage(string content)
    {
        AddMessage("assistant", content);
    }

    public IReadOnlyList<ChatMessage> GetMessages()
    {
        return _messages
            .Select(message => new ChatMessage(message.Role, message.Content)
            {
                CreatedAt = message.CreatedAt
            })
            .ToList();
    }

    public void Clear()
    {
        _messages.Clear();
        _messages.Add(_systemMessage);
    }

    private void AddMessage(string role, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return;
        }

        _messages.Add(new ChatMessage(role, content.Trim()));
        _memoryManager.Trim(_messages);
    }
}
