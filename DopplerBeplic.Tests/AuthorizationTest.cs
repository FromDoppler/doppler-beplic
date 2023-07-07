
namespace DopplerBeplic.Tests;

public class AuthorizationTest
{
    private readonly ITestOutputHelper _output;

    public AuthorizationTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task POST_account_should_not_authorize_without_token()
    {
        // Arrange
        await using var application = new PlaygroundApplication();
        using var client = application.CreateClient();

        // Act
        using var response = await client.PostAsync("/account", JsonContent.Create(new { }));

        _output.WriteLine(response.GetHeadersAsString());
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Bearer", response.Headers.WwwAuthenticate.ToString());
    }

    [Fact]
    public async Task POST_account_should_not_authorize_doppler_user()
    {
        // Arrange
        await using var application = new PlaygroundApplication();
        using var client = application.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Account_123_test1AtTestDotCom_Expire2033_05_18);

        // Act
        using var response = await client.PostAsync("/account", JsonContent.Create(new { }));

        _output.WriteLine(response.GetHeadersAsString());
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task POST_account_should_not_authorize_expired_super_user()
    {
        // Arrange
        await using var application = new PlaygroundApplication();
        using var client = application.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", TestUsersData.Token_Superuser_Expire2001_09_08);

        // Act
        using var response = await client.PostAsync("/account", JsonContent.Create(new { }));

        _output.WriteLine(response.GetHeadersAsString());
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        Assert.Equal("Bearer error=\"invalid_token\", error_description=\"The token expired at '09/09/2001 01:46:40'\"", response.Headers.WwwAuthenticate.ToString());
    }
}
