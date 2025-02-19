using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class GetConversationsByExternalIdAndDatesTests
    {
        [Fact]
        public async Task GET_conversations_should_not_authorize_without_token()
        {
            var application = new PlaygroundApplication();
            var client = application.CreateClient();

            var response = await client.GetAsync("/customer/1/conversations");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_conversations_should_authorize_super_user()
        {
            var expectedConversations = 0;
            var dateFrom = "20240201";
            var dateTo = "20240228";

            var application = new PlaygroundApplication();
            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.GetConversationsByCustomerAndDates(1, dateFrom, dateTo))
                .ReturnsAsync(expectedConversations);
            application.ConfigureServices(s => s.AddSingleton(beplicServiceMock.Object));

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            var response = await client.GetAsync($"/customer/1/conversations?dateFrom={dateFrom}&dateTo={dateTo}");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            beplicServiceMock.Verify(x => x.GetConversationsByCustomerAndDates(1, dateFrom, dateTo), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            var conversations = JsonConvert.DeserializeObject<int>(content);

            Assert.Equal(conversations, expectedConversations);
        }
    }
}
