namespace QwenOllama.ConsoleMemoryLab.Services;

public enum ChatCommandType
{
    None,
    Help,
    Clear,
    History,
    Memory,
    StreamOn,
    StreamOff,
    Exit,
    Unknown
}

public sealed record ChatCommand(ChatCommandType Type, string RawInput)
{
    public bool IsCommand => Type != ChatCommandType.None;
}
