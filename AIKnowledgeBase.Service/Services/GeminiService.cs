using AIKnowledgeBase.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AIKnowledgeBase.Core.Entities;


namespace AIKnowledgeBase.Service.Services
{
    public class GeminiService : IAIService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;

        public GeminiService(IConfiguration configuration)
        {
            // 1. Anahtarı appsettings.json'dan alıyoruz
            _apiKey = configuration["Gemini:ApiKey"];

            Console.WriteLine($"DEBUG: API Key okundu mu?: {!string.IsNullOrEmpty(_apiKey)}");

            // 2. HttpClient'ı başlatıyoruz (En sağlam ağ aracıdır)
            _httpClient = new HttpClient();
        }

        public async Task<string> AnalyzeTextAsync(string documentText, string userQuestion, List<ChatMessage> history)
        {
            try
            {
                //  URL ve Model ismini kullanıyoruz
                 string url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";


                //Gemini'nin anlayacağı formatta geçmişi ve yeni soruyu hazırlıyoruz
                var chatParts = new List<object>();

                //önce dökümanı ve geçmişini yüklüyoruz
                foreach (var msg in history)
                {
                    chatParts.Add(new
                    {
                        role = msg.Role == "user" ? "user" : "model",
                        parts = new[] { new { text = msg.Content } }
                    });
                }

                //En sona da güncel dökümanı ve soruyu ekliyoruz (Bu Gemini'nin son odaklanacağı yer olur)
                chatParts.Add(new
                {
                    role = "user",
                    parts = new[] { new { text = $"Döküman içeriğine göre cevapla: {documentText}\n\nSoru: {userQuestion}" } }
                });

                // 5. Google'ın beklediği JSON formatını oluşturuyoruz
                var requestBody = new
                {
                    contents = chatParts //tek bir parça değil tüm liste gidiyor
                    
                };

                var jsonPayload = JsonSerializer.Serialize(requestBody);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 6. İstek gönderiliyor
                var response = await _httpClient.PostAsync(url, content);
                var resultJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return $"API Hatası ({response.StatusCode}): {resultJson}";
                }

                // 7. Gelen karmaşık JSON'u temizleyip sadece AI'nın cevabını alıyoruz
                using var doc = JsonDocument.Parse(resultJson);
                var aiResponseText = doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();

                return aiResponseText ?? "Cevap alınamadı";
            }
            catch (Exception ex)
            {
                return $"Servis Hatası: {ex.Message}";
            }
        }
    }
}