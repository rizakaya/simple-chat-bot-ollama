using System.Text;

namespace QwenOllama.ConsoleMemoryLab.Services;

public sealed class ConsoleChatRunner
{
    private readonly IConversationMemory _memory;
    private readonly IOllamaClient _ollamaClient;
    private bool _useStreaming;

    public ConsoleChatRunner(IConversationMemory memory, IOllamaClient ollamaClient, bool useStreaming)
    {
        _memory = memory;
        _ollamaClient = ollamaClient;
        _useStreaming = useStreaming;
    }

    public async Task RunAsync(CancellationToken cancellationToken = default)
    {
        Console.OutputEncoding = Encoding.UTF8;
        PrintWelcome();

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.Write("Sen: ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
            {
                continue;
            }

            var command = CommandParser.Parse(input);
            if (command.IsCommand)
            {
                if (HandleCommand(command))
                {
                    return;
                }

                continue;
            }

            await SendMessageAsync(input, cancellationToken);
        }
    }

    private async Task SendMessageAsync(string input, CancellationToken cancellationToken)
    {
        _memory.AddUserMessage(input);
        Console.Write("AI: ");

        try
        {
            string answer;

            if (_useStreaming)
            {
                var builder = new StringBuilder();

                await foreach (var chunk in _ollamaClient.ChatStreamAsync(_memory.GetMessages(), cancellationToken))
                {
                    Console.Write(chunk);
                    builder.Append(chunk);
                }

                answer = builder.ToString();
                Console.WriteLine();
            }
            else
            {
                answer = await _ollamaClient.ChatAsync(_memory.GetMessages(), cancellationToken);
                Console.WriteLine(answer);
            }

            _memory.AddAssistantMessage(answer);
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine();
            Console.WriteLine($"Ollama baglantisi basarisiz: {ex.Message}");
            Console.WriteLine("Ollama calisiyor mu ve model indirildi mi kontrol et.");
        }
    }

    private bool HandleCommand(ChatCommand command)
    {
        switch (command.Type)
        {
            case ChatCommandType.Help:
                PrintHelp();
                return false;
            case ChatCommandType.Clear:
                _memory.Clear();
                Console.WriteLine("History temizlendi. System prompt korunuyor.");
                return false;
            case ChatCommandType.History:
                PrintHistory();
                return false;
            case ChatCommandType.Memory:
                PrintMemory();
                return false;
            case ChatCommandType.StreamOn:
                _useStreaming = true;
                Console.WriteLine("Streaming acildi.");
                return false;
            case ChatCommandType.StreamOff:
                _useStreaming = false;
                Console.WriteLine("Streaming kapatildi.");
                return false;
            case ChatCommandType.Exit:
                Console.WriteLine("Gorusmek uzere.");
                return true;
            default:
                Console.WriteLine("Bilinmeyen komut. /help yazabilirsin.");
                return false;
        }
    }

    private static void PrintWelcome()
    {
        Console.WriteLine("Qwen Ollama Console Memory Lab");
        Console.WriteLine("Komutlari gormek icin /help yaz.");
        Console.WriteLine();
    }

    private static void PrintHelp()
    {
        Console.WriteLine("/help       Komutlari gosterir");
        Console.WriteLine("/clear      History'yi temizler");
        Console.WriteLine("/history    Mevcut mesaj gecmisini gosterir");
        Console.WriteLine("/memory     Memory durumunu gosterir");
        Console.WriteLine("/stream on  Streaming acar");
        Console.WriteLine("/stream off Streaming kapatir");
        Console.WriteLine("/exit       Cikis yapar");
    }

    private void PrintHistory()
    {
        Console.WriteLine("--- HISTORY ---");
        foreach (var message in _memory.GetMessages())
        {
            Console.WriteLine($"[{message.Role}] {message.Content}");
        }

        Console.WriteLine("---------------");
    }

    private void PrintMemory()
    {
        var messages = _memory.GetMessages();
        var conversationCount = messages.Count(message => message.Role != "system");

        Console.WriteLine($"Toplam mesaj: {messages.Count}");
        Console.WriteLine($"Konusma mesaji: {conversationCount}");
        Console.WriteLine($"System prompt korunuyor: {messages.Any(message => message.Role == "system")}");
        Console.WriteLine($"Streaming: {(_useStreaming ? "acik" : "kapali")}");
    }
}
