using device_events_receiver;
using device_events_receiver_library;
using device_event_router;

using Microsoft.Extensions.Configuration;

var builder = Host.CreateApplicationBuilder(args);
builder.Logging.AddSimpleConsole(configure =>
                {
                    configure.SingleLine = true;
                    configure.TimestampFormat = "s";
                });
builder.Services.AddHostedService<Worker>();
builder.Services.AddSingleton<IDeviceEventsReceiver, DeviceEventsReceiver>(x => {
    var config = x.GetRequiredService<IConfiguration>();
    var logging = x.GetRequiredService<ILogger<DeviceEventsReceiver>>();
    var settings = config.GetRequiredSection("Server").Get<Settings>();
    if (settings is null)
        throw new ArgumentException("Missing Settings");
    return new DeviceEventsReceiver(settings.ListeningPort, logging);
});
builder.Services.AddSingleton<IDeviceEventsRouter, DeviceEventsRouter>();
var host = builder.Build();

ILogger log = host.Services.GetRequiredService<ILogger<Program>>();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
Settings? settings = config.GetRequiredSection("Server").Get<Settings>();
if (settings is null)
{
    log.LogError("Failed loading settings");
    Environment.Exit(-1);
}
host.Run();
