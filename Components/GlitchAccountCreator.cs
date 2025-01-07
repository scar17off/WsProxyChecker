using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace WsProxyChecker.Components
{
    internal class GlitchAccountCreator
    {
        private readonly HttpClient _client;
        private readonly TempMailApi _mailApi;
        private string _recaptchaToken;
        private string _currentEmail;

        public GlitchAccountCreator()
        {
            _client = new HttpClient();
            _mailApi = new TempMailApi();
        }

        public void SetRecaptchaToken(string token)
        {
            _recaptchaToken = token;
        }

        public async Task<string> PrepareEmail()
        {
            return await _mailApi.CreateMailbox();
        }

        public async Task<GlitchAccount> CreateAccountWithEmail(string email)
        {
            _currentEmail = email;
            if (string.IsNullOrEmpty(_recaptchaToken))
            {
                throw new InvalidOperationException("Recaptcha token not set");
            }

            try
            {
                Console.WriteLine($"Creating account with email: {email}");
                await RequestEmailVerification(email, _recaptchaToken);
                
                Console.WriteLine("Waiting for verification email...");
                var message = await _mailApi.WaitForGlitchEmail(email);
                
                // Get message details with token
                var messageDetails = await _mailApi.GetMessageDetails(message.Id);
                
                Console.WriteLine($"Got verification code: {messageDetails.Text}");
                var account = await VerifyEmail(messageDetails);
                account.Email = email;
                
                return account;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAccountWithEmail: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task RequestEmailVerification(string email, string recaptchaToken)
        {
            try
            {
                Console.WriteLine($"Requesting verification for email: {email}");
                Console.WriteLine($"Using reCAPTCHA token: {recaptchaToken.Substring(0, 20)}...");

                var request = new Dictionary<string, string>
                {
                    { "emailAddress", email },
                    { "recaptcha", recaptchaToken }
                };

                var content = new StringContent(
                    JsonSerializer.Serialize(request),
                    Encoding.UTF8,
                    "application/json"
                );
                content.Headers.ContentType.CharSet = "UTF-8";

                // Clear headers before setting new ones
                _client.DefaultRequestHeaders.Clear();
                
                // Set headers exactly as needed
                _client.DefaultRequestHeaders.Add("accept", "application/json, text/plain, */*");
                _client.DefaultRequestHeaders.Add("sec-fetch-dest", "empty");
                _client.DefaultRequestHeaders.Add("sec-fetch-mode", "cors");
                _client.DefaultRequestHeaders.Add("sec-fetch-site", "same-site");
                _client.DefaultRequestHeaders.Add("origin", "https://glitch.com");
                _client.DefaultRequestHeaders.Add("referer", "https://glitch.com/");

                var response = await _client.PostAsync(
                    "https://api.glitch.com/v1/auth/email/",
                    content
                );

                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Response status: {response.StatusCode}");
                Console.WriteLine($"Response content: {responseContent}");

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception($"Failed to request email verification: {responseContent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in RequestEmailVerification: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private async Task<GlitchAccount> VerifyEmail(EmailMessageContent messageContent)
        {
            try
            {
                // Extract verification code
                var verificationCode = ExtractVerificationCode(messageContent);
                Console.WriteLine($"Extracted verification code: {verificationCode}");

                // First, get the user instance
                var request = new HttpRequestMessage(HttpMethod.Post, $"https://api.glitch.com/v1/auth/email/{verificationCode}");
                
                // Set headers for Glitch API
                request.Headers.Add("accept", "application/json, text/plain, */*");
                request.Headers.Add("accept-language", "uk,en-US;q=0.9,en;q=0.8,ru;q=0.7");
                request.Headers.Add("origin", "https://glitch.com");
                request.Headers.Add("priority", "u=1, i");
                request.Headers.Add("referer", "https://glitch.com/");
                request.Headers.Add("sec-ch-ua", "\"Google Chrome\";v=\"131\", \"Chromium\";v=\"131\", \"Not_A Brand\";v=\"24\"");
                request.Headers.Add("sec-ch-ua-mobile", "?0");
                request.Headers.Add("sec-ch-ua-platform", "\"Windows\"");
                request.Headers.Add("sec-fetch-dest", "empty");
                request.Headers.Add("sec-fetch-mode", "cors");
                request.Headers.Add("sec-fetch-site", "same-site");
                request.Headers.Add("user-agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/131.0.0.0 Safari/537.36");

                var response = await _client.SendAsync(request);
                
                if (!response.IsSuccessStatusCode)
                {
                    var content2 = await response.Content.ReadAsStringAsync();
                    throw new Exception($"Failed to verify email: {content2}");
                }

                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Verification response: {content}");

                // Deserialize the response with user wrapper
                var userResponse = JsonSerializer.Deserialize<GlitchUserResponse>(content);
                var user = userResponse.User;

                // Return simplified account info
                return new GlitchAccount
                {
                    Email = _currentEmail,
                    Token = user.PersistentToken,
                    CreatedAt = DateTime.Parse(user.CreatedAt ?? DateTime.UtcNow.ToString("o"))
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in VerifyEmail: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                throw;
            }
        }

        private string ExtractVerificationCode(EmailMessageContent messageContent)
        {
            try
            {
                var htmlContent = messageContent.Html ?? "";
                var textContent = messageContent.Text ?? "";

                Console.WriteLine($"Full HTML content: {htmlContent}");
                Console.WriteLine($"Full text content: {textContent}");

                // First try to find the code in the URL
                var urlMatch = Regex.Match(
                    htmlContent,
                    @"href=""[^""]*?/login/email\?token=([^""&]+)""",
                    RegexOptions.IgnoreCase
                );

                if (urlMatch.Success)
                {
                    var code = urlMatch.Groups[1].Value.Trim();
                    Console.WriteLine($"Found verification code in URL: {code}");
                    return code;
                }

                // Then try to find it in the code tags
                var codeMatch = Regex.Match(
                    htmlContent,
                    @"<code[^>]*>([a-z0-9-]+)</code>",
                    RegexOptions.IgnoreCase
                );

                if (codeMatch.Success)
                {
                    var code = codeMatch.Groups[1].Value.Trim();
                    Console.WriteLine($"Found verification code in code tag: {code}");
                    return code;
                }

                throw new Exception("Could not find verification code in email");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error extracting verification code: {ex.Message}");
                throw;
            }
        }
    }

    public class GlitchAccount
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public int ProjectCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class GlitchError
    {
        public string Message { get; set; }
        public string Code { get; set; }
        public Dictionary<string, object> Details { get; set; }
    }

    public class GlitchUserInstance
    {
        [JsonPropertyName("isSupport")]
        public bool IsSupport { get; set; }

        [JsonPropertyName("isInfrastructureUser")]
        public bool IsInfrastructureUser { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("githubId")]
        public object GithubId { get; set; }

        [JsonPropertyName("githubToken")]
        public object GithubToken { get; set; }

        [JsonPropertyName("facebookId")]
        public object FacebookId { get; set; }

        [JsonPropertyName("facebookToken")]
        public object FacebookToken { get; set; }

        [JsonPropertyName("googleId")]
        public object GoogleId { get; set; }

        [JsonPropertyName("googleToken")]
        public object GoogleToken { get; set; }

        [JsonPropertyName("slackId")]
        public object SlackId { get; set; }

        [JsonPropertyName("slackToken")]
        public object SlackToken { get; set; }

        [JsonPropertyName("slackTeamId")]
        public object SlackTeamId { get; set; }

        [JsonPropertyName("persistentToken")]
        public string PersistentToken { get; set; }

        [JsonPropertyName("avatarUrl")]
        public object AvatarUrl { get; set; }

        [JsonPropertyName("avatarThumbnailUrl")]
        public object AvatarThumbnailUrl { get; set; }

        [JsonPropertyName("login")]
        public string Login { get; set; }

        [JsonPropertyName("name")]
        public object Name { get; set; }

        [JsonPropertyName("location")]
        public object Location { get; set; }

        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("hasCoverImage")]
        public bool HasCoverImage { get; set; }

        [JsonPropertyName("coverColor")]
        public object CoverColor { get; set; }

        [JsonPropertyName("thanksCount")]
        public int ThanksCount { get; set; }

        [JsonPropertyName("utcOffset")]
        public object UtcOffset { get; set; }

        [JsonPropertyName("featuredProjectId")]
        public object FeaturedProjectId { get; set; }

        [JsonPropertyName("createdAt")]
        public string CreatedAt { get; set; }

        [JsonPropertyName("updatedAt")]
        public string UpdatedAt { get; set; }

        [JsonPropertyName("twoFactorEnabled")]
        public bool TwoFactorEnabled { get; set; }

        [JsonPropertyName("accountLocked")]
        public bool AccountLocked { get; set; }

        [JsonPropertyName("loginAttempts")]
        public int LoginAttempts { get; set; }

        [JsonPropertyName("passwordEnabled")]
        public bool PasswordEnabled { get; set; }

        [JsonPropertyName("suspendedAt")]
        public object SuspendedAt { get; set; }

        [JsonPropertyName("suspendedReason")]
        public string SuspendedReason { get; set; }

        [JsonPropertyName("persistentTokenGeneratedAt")]
        public string PersistentTokenGeneratedAt { get; set; }

        [JsonPropertyName("features")]
        public object[] Features { get; set; }
    }

    public class GlitchUserResponse
    {
        [JsonPropertyName("user")]
        public GlitchUserInstance User { get; set; }
    }
}
