namespace device_event_router;

using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using device_events_receiver_library;

public class DeviceEventsRouter(ILogger<DeviceEventsRouter> logger, RabbitMQSettings rabbitMQSettings) : IDeviceEventsRouter
{
    private readonly ILogger<DeviceEventsRouter> _log = logger;
    private readonly RabbitMQSettings _rabbitMQSettings = rabbitMQSettings;
    private IConnection? rabbitMQConnection = null;
    public void RouteDeviceEvent(string received)
    {
        _log.LogDebug("RouteDeviceEvent was called with {received}", received);
        ConnectionFactory factory = new()
        {
            UserName = _rabbitMQSettings.UserName,
            Password = _rabbitMQSettings.Password,
            HostName = _rabbitMQSettings.HostName,
            VirtualHost = _rabbitMQSettings.VirtualHost
        };
        if (rabbitMQConnection is null || !rabbitMQConnection.IsOpen)
            rabbitMQConnection = factory.CreateConnection();
        using IModel channel = rabbitMQConnection.CreateModel();
        channel.QueueDeclare(queue: _rabbitMQSettings.Queue, durable: false, exclusive: false, autoDelete: false);
        var message = received;
        var encodedMessage = Encoding.UTF8.GetBytes(message);
        channel.BasicPublish("", _rabbitMQSettings.Queue, null, encodedMessage);
        _log.LogInformation($"Published message {message} to queue {_rabbitMQSettings.Queue}");
    }

}
