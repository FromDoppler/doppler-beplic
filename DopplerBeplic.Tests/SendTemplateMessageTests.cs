using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class SendTemplateMessageTests
    {
        [Fact]
        public async Task POST_send_message_should_not_authorize_without_token()
        {
            var application = new PlaygroundApplication();
            var client = application.CreateClient();

            var response = await client.PostAsync("/customer/rooms/1/templates/1/message", JsonContent.Create(new { }));

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task POST_send_message_should_authorize_super_user_and_returns_messageId()
        {
            var bodyParams = new { };

            var application = new PlaygroundApplication();

            var expectedResponse = new TemplateMessageResponse()
            {
                MessageId = Guid.NewGuid().ToString(),
            };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock.Setup(x => x.SendTemplateMessage(1, 1, It.IsAny<TemplateMessageDTO>())).ReturnsAsync(expectedResponse);

            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            var response = await client.PostAsync("/customer/rooms/1/templates/1/message", JsonContent.Create(bodyParams));

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            beplicServiceMock.Verify(x => x.SendTemplateMessage(1, 1, It.IsAny<TemplateMessageDTO>()), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            var sentMessage = JsonConvert.DeserializeObject<TemplateMessageResponse>(content);

            Assert.Equal(sentMessage?.MessageId, expectedResponse.MessageId);
        }

    }
}
