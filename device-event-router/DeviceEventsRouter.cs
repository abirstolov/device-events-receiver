namespace device_event_router;

using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

public class DeviceEventsRouter(ILogger<DeviceEventsRouter> logger) : IDeviceEventsRouter
{
    private readonly ILogger<DeviceEventsRouter> _log = logger;

    public void RouteDeviceEvent(string received)
    {
        _log.LogDebug("RouteDeviceEvent was called with {received}", received);
        ConnectionFactory factory = new()
        {
            UserName = "guest",
            Password = "guest",
            HostName = "localhost",
            VirtualHost = "/"
        };
        using IConnection conn = factory.CreateConnection();
        using IModel channel = conn.CreateModel();
        channel.QueueDeclare(queue: "letterbox", durable: false, exclusive: false, autoDelete: false);
        var message = "This is my first message";
        var encodedMessage = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("", "letterbox", null, encodedMessage);
        _log.LogInformation($"Published message {message}");
    }
}
