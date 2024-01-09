using device_events_receiver;
using device_events_receiver_library;
using device_event_router;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging.Configuration;
using Microsoft.Extensions.Logging.EventLog;

var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddWindowsService(options => {
    options.ServiceName = "Device Events Receiver";
});
LoggerProviderOptions.RegisterProviderOptions<EventLogSettings, EventLogLoggerProvider>(builder.Services);
builder.Logging.AddSimpleConsole(configure =>
                {
                    configure.SingleLine = true;
                    configure.TimestampFormat = "s";
                });
builder.Services.AddHostedService<WindowsService>();
builder.Services.AddSingleton<ServerSettings>(x => {
    var config = x.GetRequiredService<IConfiguration>();
    var serverSettings = config.GetRequiredSection("Server").Get<ServerSettings>() ?? throw new ArgumentException("Missing Settings");
    return serverSettings;
});
builder.Services.AddSingleton<RabbitMQSettings>(x => {
    var config = x.GetRequiredService<IConfiguration>();
    var rabbitMQSettings = config.GetRequiredSection("RabbitMQ").Get<RabbitMQSettings>() ?? throw new ArgumentException("Missing Settings");
    return rabbitMQSettings;
});
builder.Services.AddSingleton<IDeviceEventsReceiver, DeviceEventsReceiver>();
builder.Services.AddSingleton<IDeviceEventsRouter, DeviceEventsRouter>();
var host = builder.Build();

ILogger log = host.Services.GetRequiredService<ILogger<Program>>();

IConfiguration config = host.Services.GetRequiredService<IConfiguration>();
ServerSettings? settings = config.GetRequiredSection("Server").Get<ServerSettings>();
if (settings is null)
{
    log.LogError("Failed loading settings");
    Environment.Exit(-1);
}
host.Run();
