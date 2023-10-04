using System.Globalization;
using Serilog;

namespace DopplerBeplic.Logging
{
    public static class SerilogSetup
    {
        public static LoggerConfiguration SetupSeriLog(
            this LoggerConfiguration loggerConfiguration,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            configuration.ConfigureLoggly(hostEnvironment);

            loggerConfiguration
                .WriteTo.Console(formatProvider: CultureInfo.InvariantCulture)
                .Enrich.WithProperty("Application", hostEnvironment.ApplicationName)
                .Enrich.WithProperty("Environment", hostEnvironment.EnvironmentName)
                .Enrich.WithProperty("Platform", Environment.OSVersion.Platform)
                .Enrich.WithProperty("OSVersion", Environment.OSVersion)
                .Enrich.FromLogContext();

            if (!hostEnvironment.IsDevelopment())
            {
                loggerConfiguration
                    .WriteTo.Loggly(formatProvider: CultureInfo.InvariantCulture);
            }

            loggerConfiguration.ReadFrom.Configuration(configuration);

            return loggerConfiguration;
        }
    }
}
