namespace QwenOllama.ConsoleMemoryLab.Services;

public static class CommandParser
{
    public static ChatCommand Parse(string? input)
    {
        var rawInput = input?.Trim() ?? string.Empty;

        if (!rawInput.StartsWith('/'))
        {
            return new ChatCommand(ChatCommandType.None, rawInput);
        }

        var normalized = rawInput.ToLowerInvariant();
        var type = normalized switch
        {
            "/help" => ChatCommandType.Help,
            "/clear" => ChatCommandType.Clear,
            "/history" => ChatCommandType.History,
            "/memory" => ChatCommandType.Memory,
            "/stream on" => ChatCommandType.StreamOn,
            "/stream off" => ChatCommandType.StreamOff,
            "/exit" => ChatCommandType.Exit,
            _ => ChatCommandType.Unknown
        };

        return new ChatCommand(type, rawInput);
    }
}
