using System;
using System.Text.Json.Serialization;

namespace SharpCatch.Model
{
    public class RecaptchaResponse
    {

        /// <summary>
        ///  Whether this request was a valid reCAPTCHA token for your site.
        /// </summary>
        [JsonPropertyName("success")]
        public bool Succeeded { get; set; }

        /// <summary>
        /// The score for this request (0.0 - 1.0)
        /// </summary>
        [JsonPropertyName("score")]
        public double Score { get; set; }

        /// <summary>
        /// The action name for this request.
        /// </summary>
        [JsonPropertyName("action")]
        public string Action { get; set; }
        
        /// <summary>
        /// Timestamp of the challenge load.
        /// </summary>
        [JsonPropertyName("challenge_ts")]
        public DateTime ChallengedOn { get; set; }

        /// <summary>
        /// The hostname of the site where the reCAPTCHA was solved.
        /// </summary>
        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        /// <summary>
        /// An array of error codes returned by the recaptcha validator.
        /// </summary>
        [JsonPropertyName("error-codes")]
        public string[] ErrorCodes { get; set; }

    }
}