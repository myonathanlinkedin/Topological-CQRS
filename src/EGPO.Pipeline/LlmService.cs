using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace EGPO.Pipeline
{
    public class LlmService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl;
        private readonly string _modelName;
        private readonly double _temperature;

        public LlmService(string baseUrl, string modelName, double temperature)
        {
            _httpClient = new HttpClient();
            _baseUrl = baseUrl;
            _modelName = modelName;
            _temperature = temperature;
        }

        public async Task<string> CallLlmAsync(string prompt)
        {
            var requestBody = new
            {
                model = _modelName,
                messages = new[]
                {
                    new { role = "user", content = prompt }
                },
                temperature = _temperature
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{_baseUrl}/chat/completions", content);
                response.EnsureSuccessStatusCode();

                var responseString = await response.Content.ReadAsStringAsync();
                dynamic? result = JsonConvert.DeserializeObject(responseString);
                
                return result?.choices[0].message.content ?? "Error: No response from LLM";
            }
            catch (Exception ex)
            {
                return $"LLM Error: {ex.Message}";
            }
        }

        public string BuildPrompt(string query, List<string> patches)
        {
            var sb = new StringBuilder();
            sb.AppendLine("You are a helpful assistant. Answer the query based ONLY on the following context patches. If the answer is not in the context, say 'I don't know'.");
            sb.AppendLine();
            sb.AppendLine("Context:");
            foreach (var patch in patches)
            {
                sb.AppendLine($"- {patch}");
            }
            sb.AppendLine();
            sb.AppendLine($"Query: {query}");
            sb.AppendLine("Answer:");
            
            return sb.ToString();
        }
    }
}
