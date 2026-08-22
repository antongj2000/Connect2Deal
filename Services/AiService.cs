using System.Text;
using System.Text.Json;

namespace Connect2Deal.Services
{
    public class AiService
    {
        private const string Model = "gemini-3.6-flash";

        private readonly HttpClient _http;
        private readonly IConfiguration _config;

        public AiService(HttpClient http, IConfiguration config)
        {
            _http = http;
            _config = config;
        }

        public async Task<string?> Ask(string prompt)
        {
            var apiKey = _config["Gemini:ApiKey"];
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                return null;
            }


            var url = $"https://generativelanguage.googleapis.com/v1beta/models/{Model}:generateContent?key={apiKey}";

            var payload = new
            {
                contents = new[]
                {
            new { parts = new[] { new { text = prompt } } }
        }
            };

            var body = JsonSerializer.Serialize(payload);

            try
            {
                var response = await _http.PostAsync(url,
                    new StringContent(body, Encoding.UTF8, "application/json"));

                var raw = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return null;
                }

                using var doc = JsonDocument.Parse(raw);

                return doc.RootElement
                    .GetProperty("candidates")[0]
                    .GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString()?
                    .Trim();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<string?> Translate(string text, string targetLanguage)
        {
            var prompt = $"Translate the following message to {targetLanguage}. " +
                         $"Return only the translation, with no explanation, no quotes, no preamble.\n\n" +
                         $"{text}";

            return await Ask(prompt);
        }
    }
}