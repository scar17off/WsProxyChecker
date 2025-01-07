using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Serialization;

namespace WsProxyChecker.Components
{
    internal class TempMailApi
    {
        private readonly HttpClient _client;
        private readonly Random _random = new Random();
        private readonly Dictionary<string, string> _mailboxTokens = new Dictionary<string, string>();

        private readonly string[] _userAgents = new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64; rv:122.0) Gecko/20100101 Firefox/122.0",
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Edge/121.0.2277.128",
            "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.2.1 Safari/605.1.15"
        };

        private readonly string[] _chromeVersions = new[] { "131", "130", "129", "128" };
        private readonly string[] _platforms = new[] { "Windows", "macOS", "Linux", "Android" };

        private void SetRandomHeaders(HttpClient client)
        {
            var userAgent = _userAgents[_random.Next(_userAgents.Length)];
            var chromeVersion = _chromeVersions[_random.Next(_chromeVersions.Length)];
            var platform = _platforms[_random.Next(_platforms.Length)];

            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.Add("accept", "*/*");
            client.DefaultRequestHeaders.Add("accept-language", "uk,en-US;q=0.9,en;q=0.8,ru;q=0.7");
            client.DefaultRequestHeaders.Add("user-agent", userAgent);
            client.DefaultRequestHeaders.Add("sec-ch-ua", $"\"Google Chrome\";v=\"{chromeVersion}\", \"Chromium\";v=\"{chromeVersion}\", \"Not_A Brand\";v=\"24\"");
            client.DefaultRequestHeaders.Add("sec-ch-ua-mobile", _random.Next(2) == 0 ? "?0" : "?1");
            client.DefaultRequestHeaders.Add("sec-ch-ua-platform", $"\"{platform}\"");
            client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
            client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
            client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
            client.DefaultRequestHeaders.Add("referer", "https://temp-mail.org/");
        }

        public TempMailApi()
        {
            _client = new HttpClient();
        }

        public async Task<string> CreateMailbox()
        {
            try
            {
                // Set random headers
                SetRandomHeaders(_client);
                
                // Try alternative endpoints
                var endpoints = new[]
                {
                    "https://web2.temp-mail.org/mailbox",
                    "https://api.temp-mail.org/mailbox",
                    "https://api2.temp-mail.org/mailbox"
                };

                foreach (var endpoint in endpoints)
                {
                    try
                    {
                        var response = await _client.PostAsync(endpoint, null);
                        var content = await response.Content.ReadAsStringAsync();
                        
                        if (content.Contains("cloudflare") || content.Contains("You are unable to access"))
                        {
                            Console.WriteLine($"Cloudflare detected on {endpoint}, trying next...");
                            continue;
                        }

                        Console.WriteLine($"Mailbox creation response: {content}");
                        var mailboxData = JsonSerializer.Deserialize<MailboxCreationResponse>(content);
                        
                        if (mailboxData?.Mailbox != null)
                        {
                            _mailboxTokens[mailboxData.Mailbox] = mailboxData.Token;
                            _currentMailbox = mailboxData.Mailbox;
                            Console.WriteLine($"Created mailbox: {mailboxData.Mailbox}");
                            return mailboxData.Mailbox;
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error with endpoint {endpoint}: {ex.Message}");
                    }
                }

                throw new Exception("All endpoints failed to create mailbox");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating mailbox: {ex.Message}");
                throw;
            }
        }

        public async Task<EmailMessage> WaitForGlitchEmail(string email, int timeoutSeconds = 30)
        {
            if (!_mailboxTokens.TryGetValue(email, out string token))
            {
                throw new Exception($"No token found for mailbox: {email}");
            }

            Console.WriteLine($"Waiting for email at {email}");
            var startTime = DateTime.Now;

            while ((DateTime.Now - startTime).TotalSeconds < timeoutSeconds)
            {
                try
                {
                    // Set random headers and add authorization
                    SetRandomHeaders(_client);
                    _client.DefaultRequestHeaders.Add("authorization", $"Bearer {token}");
                    _client.DefaultRequestHeaders.Add("origin", "https://temp-mail.org");
                    _client.DefaultRequestHeaders.Add("priority", "u=1, i");

                    var response = await _client.GetAsync("https://web2.temp-mail.org/messages");
                    
                    if (!response.IsSuccessStatusCode)
                    {
                        Console.WriteLine($"Error response: {response.StatusCode}");
                        Console.WriteLine($"Response content: {await response.Content.ReadAsStringAsync()}");
                        continue;
                    }

                    var content = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Email check response: {content}");

                    var mailbox = JsonSerializer.Deserialize<MailboxResponse>(content);
                    var glitchEmail = mailbox?.Messages?.FirstOrDefault(m => 
                        m.From.Contains("support@glitch.com") && 
                        m.Subject == "Your Glitch login code");

                    if (glitchEmail != null)
                    {
                        var messageDetails = await GetMessageDetails(glitchEmail._id);
                        return new EmailMessage
                        {
                            Id = glitchEmail._id,
                            From = glitchEmail.From,
                            Subject = glitchEmail.Subject,
                            Text = messageDetails.Text,
                            BodyHtml = messageDetails.Html,
                            CreatedAt = DateTimeOffset.FromUnixTimeSeconds(glitchEmail.ReceivedAt).DateTime
                        };
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error checking email: {ex.Message}");
                }

                await Task.Delay(2000);
            }

            throw new TimeoutException("No email received from Glitch");
        }

        public async Task<EmailMessageContent> GetMessageDetails(string messageId)
        {
            try
            {
                if (!_mailboxTokens.TryGetValue(_currentMailbox, out string token))
                {
                    throw new Exception($"No token found for current mailbox");
                }

                var request = new HttpRequestMessage(HttpMethod.Get, $"https://web2.temp-mail.org/messages/{messageId}");

                request.Headers.Add("accept", "application/json");
                request.Headers.Add("accept-language", "ru-RU,ru;q=0.9,en-US;q=0.8,en;q=0.7");
                request.Headers.Add("authorization", $"Bearer {token}");
                request.Headers.Add("sec-ch-ua", "\"Chromium\";v=\"92\", \" Not A;Brand\";v=\"99\", \"Google Chrome\";v=\"92\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "same-site");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_14_6) AppleWebKit/537.36");

                var response = await _client.SendAsync(request);
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Message details response: {content}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to get message details. Status: {response.StatusCode}, Content: {content}");
                }

                var messageDetails = JsonSerializer.Deserialize<MessageDetails>(content);
                return new EmailMessageContent 
                { 
                    Html = messageDetails.bodyHtml,
                    Text = messageDetails.bodyPreview
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting message details: {ex.Message}");
                throw;
            }
        }

        private string _currentMailbox;
    }

    public class EmailMessage
    {
        [JsonPropertyName("_id")]
        public string Id { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Text { get; set; }
        public string BodyHtml { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class EmailMessageContent
    {
        [JsonPropertyName("bodyHtml")]
        public string Html { get; set; }

        [JsonPropertyName("bodyPreview")]
        public string Text { get; set; }
    }

    public class MailboxTokenResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }
    }

    public class MailboxResponse
    {
        [JsonPropertyName("mailbox")]
        public string Mailbox { get; set; }

        [JsonPropertyName("messages")]
        public List<MessageInfo> Messages { get; set; }
    }

    public class MessageInfo
    {
        [JsonPropertyName("_id")]
        public string _id { get; set; }

        [JsonPropertyName("receivedAt")]
        public long ReceivedAt { get; set; }

        [JsonPropertyName("from")]
        public string From { get; set; }

        [JsonPropertyName("subject")]
        public string Subject { get; set; }

        [JsonPropertyName("bodyPreview")]
        public string BodyPreview { get; set; }
    }

    public class MailboxCreationResponse
    {
        [JsonPropertyName("token")]
        public string Token { get; set; }

        [JsonPropertyName("mailbox")]
        public string Mailbox { get; set; }
    }

    public class MessageDetails
    {
        [JsonPropertyName("_id")]
        public string _id { get; set; }

        [JsonPropertyName("bodyHtml")]
        public string bodyHtml { get; set; }

        [JsonPropertyName("bodyPreview")]
        public string bodyPreview { get; set; }

        [JsonPropertyName("createdAt")]
        public string createdAt { get; set; }

        [JsonPropertyName("from")]
        public string from { get; set; }

        [JsonPropertyName("mailbox")]
        public string mailbox { get; set; }

        [JsonPropertyName("receivedAt")]
        public long receivedAt { get; set; }

        [JsonPropertyName("subject")]
        public string subject { get; set; }

        [JsonPropertyName("user")]
        public string user { get; set; }
    }
}
