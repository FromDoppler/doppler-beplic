using Microsoft.Extensions.Hosting;

namespace DopplerBeplic.Tests;

internal sealed class PlaygroundApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;

    public PlaygroundApplication(string environment = "Development")
    {
        _environment = environment;
    }

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        builder.ConfigureServices(services =>
        {
            // TODO: Add mock/test services to the builder here
        });

        return base.CreateHost(builder);
    }
}
