using Microsoft.Extensions.Hosting;

namespace DopplerBeplic.Tests;

internal sealed class PlaygroundApplication : WebApplicationFactory<Program>
{
    private readonly string _environment;
    private readonly List<Action<IServiceCollection>> _configureDelegates = new List<Action<IServiceCollection>>();

    public PlaygroundApplication(string environment = "Development")
    {
        _environment = environment;
    }

    public void ConfigureServices(Action<IServiceCollection> configureDelegate)
        => _configureDelegates.Add(configureDelegate);

    protected override IHost CreateHost(IHostBuilder builder)
    {
        builder.UseEnvironment(_environment);

        builder.ConfigureServices(services =>
        {
            foreach (var configure in _configureDelegates)
            {
                configure(services);
            }
        });

        return base.CreateHost(builder);
    }
}
