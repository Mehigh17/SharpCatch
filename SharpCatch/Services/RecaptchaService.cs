using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using SharpCatch.Model;

namespace SharpCatch.Services
{
    public class RecaptchaService : IRecaptchaService
    {

        private const string VerificationEndpoint = "https://www.google.com/recaptcha/api/siteverify";

        private readonly string _secretKey;
        private readonly HttpClient _httpClient;

        /// <summary>
        /// Create an instance of recaptcha service.
        /// </summary>
        /// <param name="secretKey">The secret key for the application.</param>
        /// <param name="httpClient">The http client to be used. (call the other constructor if you don't have a specific implementation of the http client)</param>
        /// <returns>An instance of RecaptchaService.</returns>
        public RecaptchaService(string secretKey, HttpClient httpClient)
        {
            _secretKey = secretKey ?? throw new System.ArgumentNullException(nameof(secretKey));
            _httpClient = httpClient ?? throw new System.ArgumentNullException(nameof(httpClient));
        }

        /// <summary>
        /// Create an instance of recaptcha service with a default instance of http client.
        /// </summary>
        /// <param name="secretKey">The secret key for the application.</param>
        /// <returns>An instance of RecaptchaService.</returns>
        public RecaptchaService(string secretKey) : this(secretKey, new HttpClient())
        { }

        /// <inheritdoc/>
        public async Task<RecaptchaResponse> GetResponse(string userToken, string userAddress = null)
        {
            var values = new Dictionary<string, string>
            {
                {"secret", _secretKey},
                {"response", userToken}
            };
            
            if (!string.IsNullOrEmpty(userAddress))
            {
                values.Add("remoteip", userAddress);
            }

            var formUrlContent = new FormUrlEncodedContent(values);

            var request = await _httpClient.PostAsync(VerificationEndpoint, formUrlContent);
            var response = await JsonSerializer.DeserializeAsync<RecaptchaResponse>(await request.Content.ReadAsStreamAsync());

            return response;
        }

        /// <inheritdoc/>
        /// <summary>
        /// If threshold score is met, the property "success" will be ignored.
        /// </summary>
        public async Task<bool> IsValid(string userToken, string action = null, double? minimumScoreThreshold = null, string userAddress = null)
        {
            var response = await GetResponse(userToken, userAddress);

            if (response == null)
                return false;

            if (!string.IsNullOrEmpty(action))
            {
                if (response.Action != action)
                {
                    return false;
                }
            }

            if (minimumScoreThreshold.HasValue && response.Score >= minimumScoreThreshold.Value)
                return true;

            return response.Succeeded;
        }
    }
}