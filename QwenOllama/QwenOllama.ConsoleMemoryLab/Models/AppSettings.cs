using System.Text.Json;

namespace QwenOllama.ConsoleMemoryLab.Models;

public sealed class AppSettings
{
    public OllamaSettings Ollama { get; set; } = new();

    public MemorySettings Memory { get; set; } = new();

    public static AppSettings Load(string path)
    {
        var resolvedPath = File.Exists(path)
            ? path
            : Path.Combine(AppContext.BaseDirectory, path);

        if (!File.Exists(resolvedPath))
        {
            return new AppSettings();
        }

        var json = File.ReadAllText(resolvedPath);
        return JsonSerializer.Deserialize<AppSettings>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        }) ?? new AppSettings();
    }
}

public sealed class OllamaSettings
{
    public string BaseUrl { get; set; } = "http://localhost:11434";

    public string Model { get; set; } = "qwen2.5:7b";

    public bool UseStreaming { get; set; } = true;
}

public sealed class MemorySettings
{
    public int MaxMessages { get; set; } = 10;

    public string SystemPrompt { get; set; } =
        "Sen Turkce cevap veren, kisa ve net aciklamalar yapan yardimci bir asistansin.";
}
