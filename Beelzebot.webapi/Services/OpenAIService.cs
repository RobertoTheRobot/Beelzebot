using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Beelzebot.webapi.Services
{
    public interface IOpenAIService
    {
        Task<string> GetResponseToQuestionAsync(string question);
    }

    public class OpenAIService : IOpenAIService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private readonly ILogger<OpenAIService> _logger;

        public OpenAIService(string apiKey, ILogger<OpenAIService> logger)
        {
            _httpClient = new HttpClient();
            _apiKey = apiKey;
            _logger = logger;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        }

        public async Task<string> GetResponseToQuestionAsync(string question)
        {
            _logger.LogInformation("Generating response for question: {Question}", question);

            var requestBody = new OpenAIRequest()
            {
                Model = "gpt-4o-mini",
                Messages = new List<Message>
                {
                    new Message
                    {
                        Role = "system",
                        Content = "You are a helpful AI assistant and talk like the character Beelzebot from Futurama"
                    },
                    new Message
                    {
                        Role = "user",
                        Content = question
                    }
                },
                Temperature = 0.5
            };

            _logger.LogInformation("Request Body: {@RequestBody}", requestBody);

            try
            {
                var response = await _httpClient.PostAsJsonAsync("https://api.openai.com/v1/chat/completions", requestBody);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    var completionResponse = JsonSerializer.Deserialize<OpenAIResponse>(jsonResponse, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

                    _logger.LogInformation("Received response from OpenAI API: {@Response}",completionResponse);
                    return completionResponse.Choices.FirstOrDefault().Message.Content;
                }
                else
                {
                    var errorResponse = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Error from OpenAI API: {ErrorResponse}", errorResponse);
                    throw new ApplicationException($"Error from OpenAI API: {errorResponse}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception while calling OpenAI API");
                throw;
            }
        }

        private class CompletionResponse
        {
            public Choice[] Choices { get; set; }
        }

        private class Choice
        {
            public string Text { get; set; }
        }
    }



    public class OpenAIResponse
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("object")]
        public string Object { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("usage")]
        public Usage Usage { get; set; }

        [JsonPropertyName("choices")]
        public List<Choice> Choices { get; set; }
    }

    public class OpenAIRequest
    {
        [JsonPropertyName("model")]
        public string Model { get; set; }

        [JsonPropertyName("messages")]
        public List<Message> Messages { get; set; }

        [JsonPropertyName("temperature")]
        public double Temperature { get; set; }
    }

    public class Usage
    {
        [JsonPropertyName("prompt_tokens")]
        public int PromptTokens { get; set; }

        [JsonPropertyName("completion_tokens")]
        public int CompletionTokens { get; set; }

        [JsonPropertyName("total_tokens")]
        public int TotalTokens { get; set; }
    }

    public class Choice
    {
        [JsonPropertyName("message")]
        public Message Message { get; set; }

        [JsonPropertyName("logprobs")]
        public object LogProbs { get; set; }

        [JsonPropertyName("finish_reason")]
        public string FinishReason { get; set; }

        [JsonPropertyName("index")]
        public int Index { get; set; }
    }

    public class Message
    {
        [JsonPropertyName("role")]
        public string Role { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
    }
}
