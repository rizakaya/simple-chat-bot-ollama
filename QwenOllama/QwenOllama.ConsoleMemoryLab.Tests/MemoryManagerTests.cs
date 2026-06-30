using QwenOllama.ConsoleMemoryLab.Models;
using QwenOllama.ConsoleMemoryLab.Services;

namespace QwenOllama.ConsoleMemoryLab.Tests;

public sealed class MemoryManagerTests
{
    [Fact]
    public void Trim_Should_Keep_System_Message()
    {
        var memory = new ConversationMemory("System prompt", new MemoryManager(2));

        memory.AddUserMessage("Bir");
        memory.AddAssistantMessage("Iki");
        memory.AddUserMessage("Uc");

        var messages = memory.GetMessages();

        Assert.Equal("system", messages[0].Role);
        Assert.Equal(3, messages.Count);
    }

    [Fact]
    public void Trim_Should_Keep_Last_MaxMessages_In_Order()
    {
        var messages = new List<ChatMessage>
        {
            new("system", "System"),
            new("user", "1"),
            new("assistant", "2"),
            new("user", "3"),
            new("assistant", "4")
        };
        var manager = new MemoryManager(3);

        manager.Trim(messages);

        Assert.Equal(["System", "2", "3", "4"], messages.Select(message => message.Content));
    }

    [Fact]
    public void Constructor_Should_Reject_Zero_MaxMessages()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new MemoryManager(0));
    }
}
