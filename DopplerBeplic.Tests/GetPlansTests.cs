using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class GetPlansTests
    {
        private readonly ITestOutputHelper _output;

        public GetPlansTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task GET_plan_should_not_authorize_without_token()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();

            // Act
            using var response = await client.GetAsync("/plan");

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_plan_should_not_authorize_doppler_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Account_123_test1AtTestDotCom_Expire2033_05_18);

            // Act
            using var response = await client.GetAsync("/plan");

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task GET_plan_should_not_authorize_expired_super_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2001_09_08);

            // Act
            using var response = await client.GetAsync("/plan");

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer error=\"invalid_token\", error_description=\"The token expired at '09/09/2001 01:46:40'\"", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_plan_should_authorize_super_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();

            var expectedPlans = new List<PlanResponse>()
            {
                new()
                {
                    Id = 1,
                    PlanType = "STANDARD",
                    Name = "test",
                    Status = "PENDING_APPROVAL",
                    Price = 100,
                    PlanContractDate = "2024-07-10",
                    Publish = false,
                    StartDate = "2024-07-10",
                    EndDate = "2024-08-10",
                    TrialPeriod = 10
                }
            };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.GetPlans())
                .ReturnsAsync(expectedPlans);
            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            // Act
            using var response = await client.GetAsync(
                "/plan");

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            beplicServiceMock.Verify(x => x.GetPlans(), Times.Once);

            var plans = JsonConvert.DeserializeObject<List<PlanResponse>>(content);

            Assert.Equal(plans!.Count, expectedPlans.Count);
            Assert.Equal(plans[0].Id, expectedPlans[0].Id);
        }
    }
}
