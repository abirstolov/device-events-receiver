namespace device_event_router;
using Microsoft.Extensions.Logging;
public class DeviceEventsRouter(ILogger<DeviceEventsRouter> logger) : IDeviceEventsRouter
{
    private readonly ILogger<DeviceEventsRouter> _log = logger;

    public void RouteDeviceEvent(string received)
    {
        _log.LogDebug("RouteDeviceEvent was called with {received}", received);
    }
}
