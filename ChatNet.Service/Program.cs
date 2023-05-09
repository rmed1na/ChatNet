using ChatNet.Data.Models.Settings;
using ChatNet.Service;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostContext, services) =>
    {
        var appSettingsSection = hostContext.Configuration.GetSection("Application");

        services.Configure<ServiceSettings>(appSettingsSection);
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
