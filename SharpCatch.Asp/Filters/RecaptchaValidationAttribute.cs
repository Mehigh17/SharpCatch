using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpCatch.Services;

namespace SharpCatch.Asp.Filters
{
    /// <summary>
    /// <para>Attribute allowing reCAPTCHA validation before continuing the execution of the action.</para>
    /// <para>If you're using reCAPTCHA v2 don't precise any parameter, otherwise your validation will fail in most cases.</para>
    /// <para>If you're using reCAPTCHA v3 you can precise the action to make sure the origin of the token is valid or the minimum threshold score required to pass. ('success' property is ignored if score passes)</para>
    /// </summary>
    public class RecaptchaValidationAttribute : ActionFilterAttribute
    {
        private readonly double _minimumScoreThreshold;
        private readonly string _action;
        private readonly string _errorMessage;

        /// <summary>
        /// Create an instance of the RecaptchaValidation attribute.
        /// </summary>
        /// <param name="minimumScoreThreshold">(reCAPTCHA v3 only) Minimum score threshold required to pass the test. If score is met, "success" property will be ignored.</param>
        /// <param name="action">(reCAPTCHA v3 only) The action that the token has been issued from. This is an extra safety check to verify the origin of the token.</param>
        /// <param name="errorMessage">The error message to be shown if the token is invalid or has not been provided.</param>
        /// <exception cref="ArgumentOutOfRangeException">Throws if the threshold score is negative.</exception>
        public RecaptchaValidationAttribute(double minimumScoreThreshold = 0.0,
                                            string action = null,
                                            string errorMessage = "Your recaptcha input was invalid, please try again.")
        {
            if (minimumScoreThreshold < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(_minimumScoreThreshold), "The threshold score cannot be negative");
            }
            
            _minimumScoreThreshold = minimumScoreThreshold;
            _action = action;
            _errorMessage = errorMessage;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            context.HttpContext.Request.Query.TryGetValue("g-recaptcha-response", out var userToken);

            var recaptchaService = context.HttpContext.RequestServices.GetService(typeof(IRecaptchaService)) as RecaptchaService;
            if (recaptchaService == null) throw new NullReferenceException($"The service of type {nameof(IRecaptchaService)} could not be resolved.");

            var valid = await recaptchaService.IsValid(userToken.FirstOrDefault() ?? null,
                                                       _action,
                                                       _minimumScoreThreshold > 0.0 ? (double?)_minimumScoreThreshold : null, // Don't allow negative or zero scores since that would always pass the reCAPTCHA check.
                                                       null);

            if (valid)
            {
                await next.Invoke();
                return;
            }

            context.ModelState.TryAddModelError("InvalidRecaptcha", _errorMessage);
            context.Result = new BadRequestObjectResult(new ValidationProblemDetails(context.ModelState));
        }

    }
}