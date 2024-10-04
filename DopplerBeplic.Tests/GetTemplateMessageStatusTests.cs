using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class GetTemplateMessageStatusTests
    {
        [Fact]
        public async Task GET_get_message_status_should_not_authorize_without_token()
        {
            var application = new PlaygroundApplication();
            var client = application.CreateClient();

            var response = await client.GetAsync("/customer/messages/xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_get_message_status_should_authorize_super_user_and_returns_status()
        {
            var bodyParams = new { };

            var application = new PlaygroundApplication();

            var messageId = Guid.NewGuid().ToString();

            var expectedResponse = new TemplateMessageResponse()
            {
                MessageId = messageId,
                Status = new List<TemplateMessageStatusResponse>()
                {
                    new TemplateMessageStatusResponse()
                    {
                        Status = "READ",
                        StatusDate = DateTime.UtcNow
                    }
                }
            };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock.Setup(x => x.GetMessageStatus(messageId)).ReturnsAsync(expectedResponse);

            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            var response = await client.GetAsync($"/customer/messages/{messageId}");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            beplicServiceMock.Verify(x => x.GetMessageStatus(messageId), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            var messageResponse = JsonConvert.DeserializeObject<TemplateMessageResponse>(content);

            Assert.Equal(messageResponse?.MessageId, expectedResponse.MessageId);
            Assert.Equal("READ", messageResponse?.Status?[0].Status);
        }

    }
}
