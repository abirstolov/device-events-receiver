namespace device_event_router;

public interface IDeviceEventsRouter
{
    void RouteDeviceEvent(string received);
}
