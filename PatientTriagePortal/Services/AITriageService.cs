using System.Text;
using System.Text.Json;
using PatientTriagePortal.Models;

namespace PatientTriagePortal.Services;

public class AITriageService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AITriageService> _logger;

    public AITriageService(IHttpClientFactory httpClientFactory, IConfiguration configuration, ILogger<AITriageService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<(string TriageLevel, string Department)> EvaluateSymptomsAsync(string description)
    {
        var apiKey = _configuration["LLM:ApiKey"] ?? "MOCK_KEY";
        var endpoint = _configuration["LLM:Endpoint"] ?? "https://api.openai.com/v1/chat/completions";

        var payload = new
        {
            model = "gpt-4",
            messages = new[]
            {
                new { role = "system", content = "You are an AI Triage system. Respond only with a JSON object containing 'TriageLevel' (Emergency, Urgent, Routine) and 'Department'." },
                new { role = "user", content = $"Patient symptoms: {description}" }
            }
        };

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

            var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            
            // Commenting out the actual call since we lack a real API key, simulating a response delay instead
            // var response = await client.PostAsync(endpoint, content);
            // var resultString = await response.Content.ReadAsStringAsync();
            await Task.Delay(500); // simulate network latency
            
            // Fallback mock logic for safe testing
            var desc = description.ToLowerInvariant();
            if (desc.Contains("chest pain") || desc.Contains("heart attack")) return ("Emergency", "Cardiology / ER");
            if (desc.Contains("bone") || desc.Contains("fracture")) return ("Urgent", "Orthopedics");
            if (desc.Contains("fever") || desc.Contains("cough")) return ("Routine", "General Practice");

            return ("Routine", "General Practice");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to connect to LLM endpoint.");
            return ("Routine", "General Practice (Fallback)");
        }
    }
}
