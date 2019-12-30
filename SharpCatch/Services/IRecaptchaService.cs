using System.Net;
using System.Threading.Tasks;
using SharpCatch.Model;

namespace SharpCatch.Services
{
    public interface IRecaptchaService
    {

        /// <summary>
        /// Fetch the reCAPTCHA response from the validation source.
        /// </summary>
        /// <param name="userToken">The token obtained from the user.</param>
        /// <param name="userAddress">(optional) The IP address of the user under dotted quad format. (ex: 1.2.3.4)</param>
        /// <returns>A recaptcha response.</returns>
        Task<RecaptchaResponse> GetResponse(string userToken, string userAddress = null);

        /// <summary>
        /// Check if the user token passes the validity tests.
        /// </summary>
        /// <param name="userToken">The token obtained from the user.</param>
        /// <param name="action">The action that the token has been gathered from. Check omitted if null. (Recommended to use for reCAPTCHA v3)</param>
        /// <param name="minimumScoreThreshold">The minimum score threshold that is required to pass the test, omitted if null. (inclusive)</param>
        /// <param name="userAddress">(optional) The IP address of the user under dotted quad format. (ex: 1.2.3.4)</param>
        /// <returns>Returns true if the token is valid.</returns>
        Task<bool> IsValid(string userToken, string action = null, double? minimumScoreThreshold = null, string userAddress = null);

    }
}