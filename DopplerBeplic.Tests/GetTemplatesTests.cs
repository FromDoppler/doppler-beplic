using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class GetTemplatesTests
    {
        [Fact]
        public async Task GET_templates_should_not_authorize_without_token()
        {
            var application = new PlaygroundApplication();
            var client = application.CreateClient();

            var response = await client.GetAsync("/customer/rooms/1/templates");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_templates_should_authorize_super_user()
        {
            var expectedRooms = new List<TemplateResponse>() {
                new TemplateResponse()
                {
                    Id = 1,
                    Name = "expected template 1",
                    BodyText = "template body 1"
                },
                new TemplateResponse()
                {
                    Id = 2,
                    Name = "expected temaplate 2",
                    BodyText = "template body 1"
                }
            };

            var application = new PlaygroundApplication();
            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.GetTemplatesByRoom(1))
                .ReturnsAsync(expectedRooms);
            application.ConfigureServices(s => s.AddSingleton(beplicServiceMock.Object));

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            var response = await client.GetAsync("/customer/rooms/1/templates");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            beplicServiceMock.Verify(x => x.GetTemplatesByRoom(1), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            var templates = JsonConvert.DeserializeObject<IEnumerable<TemplateResponse>>(content);

            Assert.Equal(templates?.Count(), 2);
            Assert.Equal(templates?.ElementAt(1).Id, expectedRooms.ElementAt(1).Id);
        }
    }
}
