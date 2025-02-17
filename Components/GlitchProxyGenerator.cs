using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WsProxyChecker.Components
{
    internal class GlitchProxyGenerator
    {
        private readonly HttpClient _client;
        private readonly string _authToken;
        private readonly string _baseProjectDomain;
        private const int RetryDelaySeconds = 10;

        public GlitchProxyGenerator(string authToken, string baseProjectDomain)
        {
            _client = new HttpClient();
            _authToken = authToken;
            _baseProjectDomain = baseProjectDomain;
        }

        public async Task<string> CreateProxyProject()
        {
            int retryCount = 0;
            while (true)
            {
                try
                {
                    var request = new HttpRequestMessage(HttpMethod.Post, 
                        $"https://api.glitch.com/v1/projects/by/domain/{_baseProjectDomain}/remix");

                    // Set headers exactly as in the working example
                    request.Headers.Add("accept", "application/json");
                    request.Headers.Add("accept-language", "uk,en-US;q=0.9,en;q=0.8,ru;q=0.7");
                    request.Headers.Add("authorization", _authToken);
                    request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
                    request.Headers.Add("sec-ch-ua-mobile", "?0");
                    request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                    request.Headers.Add("sec-fetch-dest", "empty");
                    request.Headers.Add("sec-fetch-mode", "cors");
                    request.Headers.Add("sec-fetch-site", "same-site");
                    request.Headers.Add("referer", "https://glitch.com/");

                    // Set request body
                    var requestBody = new RemixRequest { RemixReferer = "" };
                    var jsonContent = JsonSerializer.Serialize(requestBody);
                    request.Content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                    var response = await _client.SendAsync(request);
                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Remix response: {content}");

                    if (!response.IsSuccessStatusCode)
                    {
                        if (content.Contains("NEW_ACCOUNT_RESOURCE_CREATION_LIMIT"))
                        {
                            throw new Exception($"Failed to remix project. Status: {response.StatusCode}, Content: {content}");
                        }

                        retryCount++;
                        Console.WriteLine($"Request failed (attempt {retryCount}). Waiting {RetryDelaySeconds} seconds before retry...");
                        await Task.Delay(RetryDelaySeconds * 1000);
                        continue;
                    }

                    var projectInfo = JsonSerializer.Deserialize<GlitchProject>(content);
                    return projectInfo.Domain;
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("NEW_ACCOUNT_RESOURCE_CREATION_LIMIT"))
                    {
                        throw;
                    }

                    retryCount++;
                    Console.WriteLine($"Error creating proxy project (attempt {retryCount}): {ex.Message}");
                    Console.WriteLine($"Waiting {RetryDelaySeconds} seconds before retry...");
                    await Task.Delay(RetryDelaySeconds * 1000);
                    continue;
                }
            }
        }

        private class RemixRequest
        {
            [JsonPropertyName("remixReferer")]
            public string RemixReferer { get; set; }
        }

        private class GlitchProject
        {
            [JsonPropertyName("private")]
            public bool Private { get; set; }

            [JsonPropertyName("createdAt")]
            public string CreatedAt { get; set; }

            [JsonPropertyName("updatedAt")]
            public string UpdatedAt { get; set; }

            [JsonPropertyName("id")]
            public string Id { get; set; }

            [JsonPropertyName("suspendedReason")]
            public string SuspendedReason { get; set; }

            [JsonPropertyName("showAsGlitchTeam")]
            public bool ShowAsGlitchTeam { get; set; }

            [JsonPropertyName("notSafeForKids")]
            public bool NotSafeForKids { get; set; }

            [JsonPropertyName("appType")]
            public string AppType { get; set; }

            [JsonPropertyName("baseId")]
            public string BaseId { get; set; }

            [JsonPropertyName("description")]
            public string Description { get; set; }

            [JsonPropertyName("domain")]
            public string Domain { get; set; }

            [JsonPropertyName("gitRepoUrl")]
            public string GitRepoUrl { get; set; }

            [JsonPropertyName("isEmbedOnly")]
            public bool IsEmbedOnly { get; set; }

            [JsonPropertyName("placementCategory")]
            public string PlacementCategory { get; set; }

            [JsonPropertyName("privacy")]
            public string Privacy { get; set; }

            [JsonPropertyName("remixChain")]
            public string[] RemixChain { get; set; }

            [JsonPropertyName("edgeBadgeMode")]
            public string EdgeBadgeMode { get; set; }

            [JsonPropertyName("avatarUpdatedAt")]
            public string AvatarUpdatedAt { get; set; }
        }
    }
}