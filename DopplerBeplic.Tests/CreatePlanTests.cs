using DopplerBeplic.Models.DTO;
using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class CreatePlanTests
    {
        private readonly ITestOutputHelper _output;

        public CreatePlanTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task POST_plan_should_not_authorize_without_token()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();

            // Act
            using var response = await client.PostAsync("/plan", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task POST_plan_should_not_authorize_doppler_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Account_123_test1AtTestDotCom_Expire2033_05_18);

            // Act
            using var response = await client.PostAsync("/plan", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
        }

        [Fact]
        public async Task POST_plan_should_not_authorize_expired_super_user()
        {
            // Arrange
            await using var application = new PlaygroundApplication();
            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2001_09_08);

            // Act
            using var response = await client.PostAsync("/plan", JsonContent.Create(new { }));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer error=\"invalid_token\", error_description=\"The token expired at '09/09/2001 01:46:40'\"", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task POST_plan_should_authorize_super_user_and_return_created_statuscode()
        {
            var postData = """
            {
                "planType": "STANDARD",
                "name": "test 5",
                "status": "PENDING_APPROVAL",
                "price": 4.5,
                "planContractDate": "2024-07-24",
                "startDate": "2024-07-24",
                "endDate": "2024-08-24",
                "trialPeriod": 10,
                "planConfigurations": [
                    {
                        "name": "wsp_conversation_free",
                        "value": "500",
                        "id": 33
                    },
                    {
                        "name": "wsp_enable",
                        "value": "true",
                        "id": 34
                    }
                ],
                "publish": false
            }
            """;

            // Arrange
            await using var application = new PlaygroundApplication();

            var expectedResponse = new PlanCreationResponse()
            {
                Success = true,
                PlanId = 1,
            };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.CreatePlan(It.IsAny<PlanCreationDTO>()))
                .ReturnsAsync(expectedResponse);
            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            // Act
            using var response = await client.PostAsync(
                "/plan", JsonContent.Create(JsonConvert.DeserializeObject<PlanCreationDTO>(postData)));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);

            beplicServiceMock.Verify(x => x.CreatePlan(It.IsAny<PlanCreationDTO>()), Times.Once);

            var planCreated = JsonConvert.DeserializeObject<PlanCreationResponse>(content)!;

            Assert.Equal(planCreated.PlanId, expectedResponse.PlanId);
            Assert.True(planCreated.Success);
        }

        [Fact]
        public async Task POST_plan_should_authorize_super_user_and_return_badrequest_statuscode()
        {
            var postData = """
            {
                "planType": "STANDARD",
                "name": "test 5",
                "status": "PENDING_APPROVAL",
                "price": 4.5,
                "planContractDate": "2024-07-24",
                "startDate": "2024-07-24",
                "endDate": "2024-08-24",
                "trialPeriod": 10,
                "planConfigurations": [
                    {
                        "name": "wsp_conversation_free",
                        "value": "500",
                        "id": 33
                    },
                    {
                        "name": "wsp_enable",
                        "value": "true",
                        "id": 34
                    }
                ],
                "publish": false
            }
            """;

            // Arrange
            await using var application = new PlaygroundApplication();

            var expectedResponse = new PlanCreationResponse()
            {
                Success = false,
                Error = "Error creating the plan",
                ErrorStatus = "400"
            };

            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.CreatePlan(It.IsAny<PlanCreationDTO>()))
                .ReturnsAsync(expectedResponse);
            application.ConfigureServices(services => services.AddSingleton(beplicServiceMock.Object));

            using var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            // Act
            using var response = await client.PostAsync(
                "/plan", JsonContent.Create(JsonConvert.DeserializeObject<PlanCreationDTO>(postData)));

            _output.WriteLine(response.GetHeadersAsString());
            var content = await response.Content.ReadAsStringAsync();
            _output.WriteLine(content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);

            beplicServiceMock.Verify(x => x.CreatePlan(It.IsAny<PlanCreationDTO>()), Times.Once);

            var planCreated = JsonConvert.DeserializeObject<PlanCreationResponse>(content)!;

            Assert.Equal(expectedResponse.Error, planCreated.Error);
            Assert.Equal(expectedResponse.ErrorStatus, planCreated.ErrorStatus);
            Assert.Null(planCreated.PlanId);
            Assert.False(planCreated.Success);
        }
    }
}
