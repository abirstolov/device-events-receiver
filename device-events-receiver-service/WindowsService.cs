namespace device_events_receiver;
using device_events_receiver_library;
using device_event_router;
using static String;

public class WindowsService : BackgroundService
{
    private readonly ILogger<WindowsService> _logger;
    private readonly IDeviceEventsReceiver _deviceEventsReceiver;
    private readonly IDeviceEventsRouter _deviceEventsRouter;
    public WindowsService(ILogger<WindowsService> logger, IDeviceEventsReceiver deviceEventsReceiver, IDeviceEventsRouter deviceEventsRouter)
    {
        _logger = logger;
        _deviceEventsRouter = deviceEventsRouter;
        _deviceEventsReceiver = deviceEventsReceiver;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Listening port: {ListeningPort}", _deviceEventsReceiver.ListeningPort);
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                string received = await _deviceEventsReceiver.GetNextMessageAsync(stoppingToken);
                if (IsNullOrEmpty(received))
                    break;
                _logger.LogInformation("Received {received}", received);
                _logger.LogInformation("Routing to event broker at: {time}", DateTimeOffset.Now);
                _deviceEventsRouter.RouteDeviceEvent(received);
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Operation Canceled");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "{Message}", ex.Message);
            Environment.Exit(-1);
        }
    }
}
