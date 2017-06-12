using System;
using System.Net.Http;
using System.Threading.Tasks;
using NLog;
using Polly;

namespace OGame.Bot.Infrastructure.API.Helpers
{
    public class HttpHelper : IHttpHelper
    {
        private readonly ILogger _logger = LogManager.GetLogger(nameof(HttpHelper));

        public async Task<HttpResponseMessage> GetAsync(HttpClient httpClient, string requestUri)
        {
            var sleepDurationsBetweenAttempts = new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)
            };

            var httpResponseMessage = await Policy
                .Handle<TimeoutException>()
                .WaitAndRetryAsync(sleepDurationsBetweenAttempts, (exception, timeSpan, attempt) =>
                {
                    _logger.Warn("Exception while sending message. " +
                                $"Attempt {attempt} of {sleepDurationsBetweenAttempts.Length}. " +
                                $"Exception details: {exception}");
                }).ExecuteAsync(async () => await httpClient.GetAsync(requestUri));

            return httpResponseMessage;
        }

        public async Task<string> GetStringAsync(HttpClient httpClient, string requestUri)
        {
            var sleepDurationsBetweenAttempts = new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)
            };

            var responseBody = await Policy
                .Handle<TimeoutException>()
                .WaitAndRetryAsync(sleepDurationsBetweenAttempts, (exception, timeSpan, attempt) =>
                {
                    _logger.Warn("Exception while sending message. " +
                                $"Attempt {attempt} of {sleepDurationsBetweenAttempts.Length}. " +
                                $"Exception details: {exception}");
                }).ExecuteAsync(async () => await httpClient.GetStringAsync(requestUri));

            return responseBody;
        }

        public async Task<HttpResponseMessage> PostAsync(HttpClient httpClient, string requestUri, HttpContent content)
        {
            var sleepDurationsBetweenAttempts = new[]
            {
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(15),
                TimeSpan.FromSeconds(20),
                TimeSpan.FromSeconds(30)
            };

            var responseBody = await Policy
                .Handle<TimeoutException>()
                .WaitAndRetryAsync(sleepDurationsBetweenAttempts, (exception, timeSpan, attempt) =>
                {
                    _logger.Warn("Exception while sending message. " +
                                $"Attempt {attempt} of {sleepDurationsBetweenAttempts.Length}. " +
                                $"Exception details: {exception}");
                }).ExecuteAsync(async () => await httpClient.PostAsync(requestUri, content));

            return responseBody;
        }
    }
}
