using QwenOllama.ConsoleMemoryLab.Models;
using QwenOllama.ConsoleMemoryLab.Services;

var settings = AppSettings.Load("appsettings.json");
var memoryManager = new MemoryManager(settings.Memory.MaxMessages);
var memory = new ConversationMemory(settings.Memory.SystemPrompt, memoryManager);
var ollamaClient = new OllamaClient(new HttpClient(), settings.Ollama);
var runner = new ConsoleChatRunner(memory, ollamaClient, settings.Ollama.UseStreaming);

await runner.RunAsync();
