using System.Threading.Tasks;
using NUnit.Framework;
using OGame.Bot.Infrastructure.API.APIClients;

namespace OGame.Bot.Infrastructure.API.Tests
{
    [TestFixture]
    public class AuthorizationClientTest
    {
        [Test]
        public async Task LogInAsync_ShouldReturnSessionId()
        {
            var httpClientFactory = new HttpClientFactory();
            var authorizationClient = new AuthorizationClient(httpClientFactory);

            var sessionData = await authorizationClient.LogInAsync();

            Assert.NotNull(sessionData);
            Assert.NotNull(sessionData.RequestCookies);
            Assert.GreaterOrEqual(sessionData.RequestCookies.Count, 4);
        }
    }
}