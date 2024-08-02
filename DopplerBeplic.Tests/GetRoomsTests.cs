using DopplerBeplic.Models.Responses;
using DopplerBeplic.Services.Interfaces;
using Newtonsoft.Json;

namespace DopplerBeplic.Tests
{
    public class GetRoomsTests
    {
        [Fact]
        public async Task GET_rooms_should_not_authorize_without_token()
        {
            var application = new PlaygroundApplication();
            var client = application.CreateClient();

            var response = await client.GetAsync("/customer/1/rooms");

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
        }

        [Fact]
        public async Task GET_Rooms_should_authorize_super_user()
        {
            var expectedRooms = new List<RoomResponse>() {
                new RoomResponse()
                {
                    Id = 1,
                    Name = "expected room",
                    PhoneNumber = "111111111"
                }
            };

            var application = new PlaygroundApplication();
            var beplicServiceMock = new Mock<IBeplicService>();
            beplicServiceMock
                .Setup(x => x.GetRoomsByCustomer(1, 1))
                .ReturnsAsync(expectedRooms);
            application.ConfigureServices(s => s.AddSingleton(beplicServiceMock.Object));

            var client = application.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2033_05_18);

            var response = await client.GetAsync("/customer/1/rooms");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            beplicServiceMock.Verify(x => x.GetRoomsByCustomer(1, 1), Times.Once);

            var content = await response.Content.ReadAsStringAsync();

            var rooms = JsonConvert.DeserializeObject<IEnumerable<RoomResponse>>(content);

            Assert.Equal(rooms?.ElementAt(0).Id, expectedRooms.ElementAt(0).Id);
        }
    }
}
