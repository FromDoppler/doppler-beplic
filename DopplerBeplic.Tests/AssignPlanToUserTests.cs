using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class AssignPlanToUserTests
    {
        private readonly ITestOutputHelper _output;

        public AssignPlanToUserTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task POST_plan_customer_should_not_authorize_without_token()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();

            // Act
            using var response = await client.PostAsync("/plan/customer", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task POST_plan_customer_should_not_authorize_doppler_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Account_123_test1AtTestDotCom_Expire2033_05_18);

            // Act
            using var response = await client.PostAsync("/plan/customer", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_plan_customer_should_not_authorize_expired_super_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2001_09_08);

            // Act
            using var response = await client.PostAsync("/plan/customer", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer error=\"invalid_token\", error_description=\"The token expired at '09/09/2001 01:46:40'\"", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task POST_plan_customer_should_authorize_super_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();

            var expectedResponse = new PlanAssignResponse
            {
                Success = true,
                StartDate = "2024-07-22",
                EndDate = "2024-08-22",
                ActiveDate = "2024-07-22",
                Active = true,
                TrialPeriod = 10
            };

            var postFormData = new PlanAssignmentDTO { IdExternal = "123", IdPlan = 100 };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.PlanAssign(It.IsAny<PlanAssignmentDTO>()))
                .ReturnsAsync(expectedResponse);
            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            // Act
            using var response = await client.PostAsync(
                "/plan/customer", JsonContent.Create(postFormData));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            beplicServiceMock.Verify(x => x.PlanAssign(It.IsAny<PlanAssignmentDTO>()), Times.Once);

            var plans = JsonConvert.DeserializeObject<PlanAssignResponse>(content);

            Assert.True(plans?.Success);
        }
    }
}
