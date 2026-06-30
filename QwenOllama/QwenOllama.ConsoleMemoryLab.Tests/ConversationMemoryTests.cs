using QwenOllama.ConsoleMemoryLab.Services;

namespace QwenOllama.ConsoleMemoryLab.Tests;

public sealed class ConversationMemoryTests
{
    [Fact]
    public void Constructor_Should_Add_System_Message_First()
    {
        var memory = CreateMemory();

        var messages = memory.GetMessages();

        Assert.Single(messages);
        Assert.Equal("system", messages[0].Role);
        Assert.Equal("System prompt", messages[0].Content);
    }

    [Fact]
    public void AddUserMessage_Should_Add_Message_With_User_Role()
    {
        var memory = CreateMemory();

        memory.AddUserMessage("Merhaba");

        var messages = memory.GetMessages();
        Assert.Equal("user", messages[1].Role);
        Assert.Equal("Merhaba", messages[1].Content);
    }

    [Fact]
    public void AddAssistantMessage_Should_Add_Message_After_User_Message()
    {
        var memory = CreateMemory();

        memory.AddUserMessage("Benim adim Riza.");
        memory.AddAssistantMessage("Memnun oldum Riza.");

        var messages = memory.GetMessages();
        Assert.Equal(["system", "user", "assistant"], messages.Select(message => message.Role));
    }

    [Fact]
    public void Clear_Should_Keep_Only_System_Message()
    {
        var memory = CreateMemory();
        memory.AddUserMessage("Merhaba");
        memory.AddAssistantMessage("Merhaba.");

        memory.Clear();

        var messages = memory.GetMessages();
        Assert.Single(messages);
        Assert.Equal("system", messages[0].Role);
    }

    [Fact]
    public void GetMessages_Should_Return_Copy()
    {
        var memory = CreateMemory();

        var messages = memory.GetMessages();
        messages[0].Content = "Changed";

        Assert.Equal("System prompt", memory.GetMessages()[0].Content);
    }

    private static ConversationMemory CreateMemory()
    {
        return new ConversationMemory("System prompt", new MemoryManager(10));
    }
}
