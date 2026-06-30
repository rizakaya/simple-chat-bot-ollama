using QwenOllama.ConsoleMemoryLab.Services;

namespace QwenOllama.ConsoleMemoryLab.Tests;

public sealed class CommandParserTests
{
    [Theory]
    [InlineData("/help", ChatCommandType.Help)]
    [InlineData("/clear", ChatCommandType.Clear)]
    [InlineData("/history", ChatCommandType.History)]
    [InlineData("/memory", ChatCommandType.Memory)]
    [InlineData("/stream on", ChatCommandType.StreamOn)]
    [InlineData("/stream off", ChatCommandType.StreamOff)]
    [InlineData("/exit", ChatCommandType.Exit)]
    public void Parse_Should_Return_Known_Command(string input, ChatCommandType expectedType)
    {
        var command = CommandParser.Parse(input);

        Assert.Equal(expectedType, command.Type);
        Assert.True(command.IsCommand);
    }

    [Fact]
    public void Parse_Should_Return_None_For_Normal_Message()
    {
        var command = CommandParser.Parse("Merhaba benim adim Riza.");

        Assert.Equal(ChatCommandType.None, command.Type);
        Assert.False(command.IsCommand);
    }

    [Fact]
    public void Parse_Should_Return_Unknown_For_Unknown_Slash_Command()
    {
        var command = CommandParser.Parse("/does-not-exist");

        Assert.Equal(ChatCommandType.Unknown, command.Type);
        Assert.True(command.IsCommand);
    }
}
