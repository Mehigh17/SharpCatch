using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using SharpCatch.Services;
using Xunit;

namespace SharpCatch.Test
{
    public class RecaptchaServiceTests
    {

        [Fact]
        public async Task GetResponse_Should_ReturnNonNullResponse_When_AnySecretGiven()
        {
            const string responseBody = @"
            {
                ""success"": true,
                ""score"": 0.23
            }"; // A complete answer isn't required at all.

            var service = new RecaptchaService("somestring", new HttpClient(GetMockedHandlerForBody(responseBody)));
            var response = await service.GetResponse("usertoken");
            Assert.NotNull(response);
        }

        [Fact]
        public async Task IsValid_Should_Fail_When_ActionDoesntMatch()
        {
            const string responseBody = @"
            {
                ""success"": true,
                ""score"": 1.0,
                ""action"": ""testAction""
            }";

            var httpClient = new HttpClient(GetMockedHandlerForBody(responseBody));
            var service = new RecaptchaService("invalid", httpClient);
            var success = await service.IsValid("usertoken", action: "invalidAction");
            Assert.False(success);
        }

        [Fact]
        public async Task IsValid_Should_Succeed_When_ThresholdScoreIsMet()
        {
            const string responseBody = @"
            {
                ""score"": 0.7
            }";

            var httpClient = new HttpClient(GetMockedHandlerForBody(responseBody));
            var service = new RecaptchaService("invalid", httpClient);
            var success = await service.IsValid("usertoken", minimumScoreThreshold: 0.5);
            Assert.True(success);
        }

        [Fact]
        public async Task IsValid_Should_Succeed_When_IsSuccess()
        {
            const string responseBody = @"
            {
                ""success"": true
            }";

            var httpClient = new HttpClient(GetMockedHandlerForBody(responseBody));
            var service = new RecaptchaService("invalid", httpClient);
            var success = await service.IsValid("usertoken");
            Assert.True(success);
        }

        /// <summary>
        /// Return a mocked http message handler that always returns 200 OK with the given body.
        /// </summary>
        /// <param name="body">The body to be returned.</param>
        /// <returns>A 200 OK response with given body.</returns>
        private HttpMessageHandler GetMockedHandlerForBody(string body)
        {
            var mock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            mock.Protected()
                .Setup<Task<HttpResponseMessage>>(
                  "SendAsync",
                  ItExpr.IsAny<HttpRequestMessage>(),
                  ItExpr.IsAny<CancellationToken>()
                )
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent(body),
                })
                .Verifiable();


            return mock.Object;
        }

    }
}