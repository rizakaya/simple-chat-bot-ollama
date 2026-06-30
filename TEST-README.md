# QwenOllama.ConsoleMemoryLab Test Senaryoları

Bu dosya uygulamayı elle denerken hangi girdileri yazacağını ve hangi davranışı gözlemleyeceğini gösterir. Unit testler zaten memory ve komut mantığını doğrular; buradaki senaryolar uygulamayı gerçek Ollama ile uçtan uca anlamak içindir.

## Hazırlık

1. Ollama’yı çalıştır.
2. Kullanılacak modeli indir:

```powershell
ollama pull qwen3.6:latest
```

3. Uygulamayı başlat:

```powershell
cd QwenOllama
dotnet run --project QwenOllama.ConsoleMemoryLab
```

## Senaryo 1: Basit Chat

Girdi:

```text
Merhaba, bana iki cümleyle kendini tanıt.
```

Beklenen davranış:

- Uygulama Ollama’ya istek gönderir.
- AI cevabı console’da görünür.
- Streaming açıksa cevap parça parça akar.
- Cevap bittikten sonra assistant mesajı memory’ye eklenir.

Kontrol:

```text
/history
```

History içinde `system`, `user`, `assistant` sırasını görmelisin.

## Senaryo 2: History ile Hatırlama

Girdiler:

```text
Benim adım Rıza. En sevdiğim programlama dili C#.
Benim adım neydi ve hangi dili seviyorum?
```

Beklenen davranış:

- İkinci soruda model önceki kullanıcı mesajını görür.
- Cevapta Rıza ve C# bilgilerini kullanabilmelidir.

Kontrol:

```text
/history
```

History’de ilk bilgi mesajı, assistant cevabı, ikinci soru ve yeni assistant cevabı görünmelidir.

## Senaryo 3: Clear Komutu

Girdiler:

```text
/clear
/history
```

Beklenen davranış:

- Konuşma geçmişi temizlenir.
- `/history` çıktısında sadece `system` mesajı kalır.

Ek kontrol:

```text
Benim adım neydi?
```

Beklenen davranış:

- Model artık önceki adı güvenilir şekilde bilememelidir, çünkü history temizlenmiştir.

## Senaryo 4: Streaming Aç/Kapat

Girdiler:

```text
/stream off
Bana memory yönetimini üç maddede açıkla.
/stream on
Bana streaming cevabın ne işe yaradığını üç maddede açıkla.
```

Beklenen davranış:

- `/stream off` sonrasında cevap tek seferde görünür.
- `/stream on` sonrasında cevap parça parça akar.
- Her iki cevap da bittikten sonra history’ye assistant mesajı olarak eklenir.

Kontrol:

```text
/memory
```

Streaming durumunu ve mesaj sayısını görmelisin.

## Senaryo 5: Memory Limit Kırpma

Varsayılan `MaxMessages` değeri 10’dur. System prompt hariç son 10 konuşma mesajı tutulur.

Girdiler:

```text
Mesaj 1: sadece "1 tamam" diye cevap ver.
Mesaj 2: sadece "2 tamam" diye cevap ver.
Mesaj 3: sadece "3 tamam" diye cevap ver.
Mesaj 4: sadece "4 tamam" diye cevap ver.
Mesaj 5: sadece "5 tamam" diye cevap ver.
Mesaj 6: sadece "6 tamam" diye cevap ver.
/history
```

Beklenen davranış:

- Her kullanıcı mesajına bir assistant cevabı geldiği için system dışı mesaj sayısı hızla artar.
- Limit aşılınca en eski user/assistant mesajları kırpılır.
- `/history` çıktısında system prompt kalır.
- System dışındaki mesaj sayısı en fazla 10 olur.
- Mesaj sırası bozulmaz.

## Senaryo 6: Komut Yardımı ve Bilinmeyen Komut

Girdiler:

```text
/help
/xyz
```

Beklenen davranış:

- `/help` komut listesi gösterir.
- `/xyz` için bilinmeyen komut mesajı gösterilir.
- Bilinmeyen komut Ollama’ya gönderilmez.

## Senaryo 7: Uygulamadan Çıkış

Girdi:

```text
/exit
```

Beklenen davranış:

- Uygulama kapanır.

## Unit Test Komutu

Kod davranışlarını otomatik doğrulamak için:

```powershell
cd QwenOllama
dotnet test
```

Beklenen sonuç:

```text
Başarılı
```
