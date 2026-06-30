using System.Runtime.CompilerServices;

namespace QwenOllama.ConsoleMemoryLab.Infrastructure;

public static class JsonStreamReader
{
    public static async IAsyncEnumerable<string> ReadLinesAsync(
        Stream stream,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var reader = new StreamReader(stream);

        while (!reader.EndOfStream)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var line = await reader.ReadLineAsync(cancellationToken);
            if (!string.IsNullOrWhiteSpace(line))
            {
                yield return line;
            }
        }
    }
}
