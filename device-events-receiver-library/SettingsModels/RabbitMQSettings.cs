namespace device_events_receiver_library;
public sealed class RabbitMQSettings
{
    public required string UserName { get; set; }
    public required string Password { get; set; }
    public required string HostName { get; set; }
    public required string VirtualHost { get; set; }
    public required string Queue { get; set; }
}