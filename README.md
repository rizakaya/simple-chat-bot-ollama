# QwenOllama.ConsoleMemoryLab

Bu proje, Ollama üzerinde çalışan Qwen modeli ile konuşan küçük bir console laboratuvarıdır. Amaç sadece “chat bot” yazmak değil; LLM konuşmalarında history, memory management, streaming ve console komutlarını ayrı ayrı görmektir.

## Ana Fikir

LLM geçmişi kendiliğinden hatırlamaz. Uygulama her istekte önceki mesajları tekrar modele gönderdiği için konuşma devam ediyormuş gibi görünür.

Bu uygulama şu akışı uygular:

1. Kullanıcı console’dan mesaj yazar.
2. Mesaj `ConversationMemory` içine `user` rolüyle eklenir.
3. System prompt ve konuşma geçmişi Ollama `/api/chat` endpoint’ine gönderilir.
4. Cevap normal ya da streaming olarak alınır.
5. Assistant cevabı tekrar memory içine eklenir.
6. History büyürse `MemoryManager` eski mesajları kırpar.

## Proje Yapısı

```text
QwenOllama
├── QwenOllama.slnx
├── QwenOllama.ConsoleMemoryLab
│   ├── Program.cs
│   ├── appsettings.json
│   ├── Infrastructure
│   │   └── JsonStreamReader.cs
│   ├── Models
│   │   ├── AppSettings.cs
│   │   ├── ChatMessage.cs
│   │   ├── OllamaChatRequest.cs
│   │   └── OllamaChatResponse.cs
│   └── Services
│       ├── CommandParser.cs
│       ├── ConsoleChatRunner.cs
│       ├── ConversationMemory.cs
│       ├── IConversationMemory.cs
│       ├── IOllamaClient.cs
│       ├── MemoryManager.cs
│       └── OllamaClient.cs
└── QwenOllama.ConsoleMemoryLab.Tests
    ├── CommandParserTests.cs
    ├── ConversationMemoryTests.cs
    └── MemoryManagerTests.cs
```

## Gereksinimler

- .NET 8 SDK veya daha yeni bir SDK
- Ollama
- Ollama içinde indirilmiş bir Qwen modeli

Örnek model indirme:

```powershell
ollama pull qwen3.6:latest
```

Ollama varsayılan olarak şu adreste çalışmalıdır:

```text
http://localhost:11434
```

## Ayarlar

Uygulama ayarları [appsettings.json](QwenOllama/QwenOllama.ConsoleMemoryLab/appsettings.json) dosyasındadır.

```json
{
  "Ollama": {
    "BaseUrl": "http://localhost:11434",
    "Model": "qwen3.6:latest",
    "UseStreaming": true
  },
  "Memory": {
    "MaxMessages": 10,
    "SystemPrompt": "Sen Turkce cevap veren, kisa ve net aciklamalar yapan yardimci bir asistansin."
  }
}
```

`MaxMessages`, system prompt dışındaki son kaç konuşma mesajının tutulacağını belirler. System prompt her zaman korunur.

## Çalıştırma

```powershell
cd QwenOllama
dotnet run --project QwenOllama.ConsoleMemoryLab
```

Örnek konuşma:

```text
Sen: Merhaba benim adım Rıza.
AI: Merhaba Rıza, nasıl yardımcı olayım?

Sen: Benim adım neydi?
AI: Adın Rıza.
```

Burada modelin adı “hatırlaması”, önceki mesajların history olarak tekrar gönderilmesiyle olur.

## Console Komutları

```text
/help       Komutları gösterir
/clear      History'yi temizler, system prompt kalır
/history    Mevcut mesaj geçmişini gösterir
/memory     Memory durumunu gösterir
/stream on  Streaming modunu açar
/stream off Streaming modunu kapatır
/exit       Uygulamadan çıkar
```

## Öğrenilecek Konular

### 1. Mesajlaşma Tarihi

Mesajlar `system`, `user` ve `assistant` rolleriyle saklanır. Ollama’ya sadece son kullanıcı mesajı değil, konuşma geçmişi gönderilir.

### 2. ConversationMemory

`ConversationMemory` şu işleri yapar:

- System prompt’u ilk mesaj olarak tutar.
- Kullanıcı mesajını `user` rolüyle ekler.
- Model cevabını `assistant` rolüyle ekler.
- `/clear` sonrası sadece system prompt’u bırakır.
- Mesaj listesini dışarıya kopya olarak verir.

### 3. Streaming

Streaming açıkken cevap parça parça console’a yazılır. Aynı parçalar içeride biriktirilir ve cevap bitince tek assistant mesajı olarak history’ye eklenir.

### 4. Memory Management

History sonsuza kadar büyürse her istekte daha çok token gönderilir, cevap süresi uzar ve model context limiti dolabilir. Bu yüzden ilk strateji basit tutuldu:

- System prompt her zaman kalsın.
- System dışındaki son `MaxMessages` mesaj tutulsun.
- Daha eski konuşma mesajları silinsin.

### 5. Unit Testler

Testler gerçek Ollama bağlantısına ihtiyaç duymaz. Şu davranışları doğrular:

- System mesajı ilk sırada mı?
- User ve assistant mesajları doğru rol ile ekleniyor mu?
- Sıra korunuyor mu?
- `/clear` system prompt’u koruyor mu?
- Limit aşılınca eski mesajlar kırpılıyor mu?
- Komutlar doğru ayrıştırılıyor mu?

Testleri çalıştırmak için:

```powershell
cd QwenOllama
dotnet test
```

## Test Senaryoları

Elle deneme senaryoları için [TEST-README.md](TEST-README.md) dosyasına bakabilirsin. Orada history, clear, streaming, memory limit ve normal chat davranışını adım adım test eden girişler var.

## Sonraki Genişletme Fikirleri

- Summary memory
- JSON dosyasına kalıcı history yazma
- SQLite conversation storage
- Structured output
- Tool calling
- Randevu oluşturma gibi gerçek agent tool’ları
