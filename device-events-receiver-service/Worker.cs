namespace device_events_receiver;
using device_events_receiver_library;
using device_event_router;
using static String;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IDeviceEventsReceiver _deviceEventsReceiver;
    private readonly IDeviceEventsRouter _deviceEventsRouter;
    public Worker(ILogger<Worker> logger, IDeviceEventsReceiver deviceEventsReceiver, IDeviceEventsRouter deviceEventsRouter)
    {
        _logger = logger;
        _deviceEventsRouter = deviceEventsRouter;
        _deviceEventsReceiver = deviceEventsReceiver;

    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Listening on port {ListeningPort}", _deviceEventsReceiver.ListeningPort);
        while (!stoppingToken.IsCancellationRequested)
        {
            string received = await _deviceEventsReceiver.GetNextMessageAsync(stoppingToken);
            if (IsNullOrEmpty(received))
                break;
            _logger.LogInformation($"Received {received}");
            _logger.LogInformation("Routing to event broker at: {time}", DateTimeOffset.Now);
            _deviceEventsRouter.RouteDeviceEvent(received);
            await Task.Delay(1000, stoppingToken);
        }
    }
}
