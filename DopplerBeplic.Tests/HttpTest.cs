namespace DopplerBeplic.Tests;

public class HttpTest
{
    private readonly ITestOutputHelper _output;

    public HttpTest(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public async Task GET_inexistent_endpoint_should_return_404()
    {
        // Arrange
        await using var application = new PlaygroundApplication();
        using var client = application.CreateClient();

        // Act
        using var response = await client.GetAsync("/not-found");

        _output.WriteLine(response.GetHeadersAsString());
        var content = await response.Content.ReadAsStringAsync();
        _output.WriteLine(content);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
