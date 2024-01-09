
namespace device_events_receiver_library;
using System.Net;
using System.Net.Sockets;
using Microsoft.Extensions.Logging;

public class DeviceEventsReceiver : IDeviceEventsReceiver, IDisposable
{
    public int ListeningPort { get; }
    public readonly ServerSettings _serverSettings;
    private readonly TcpListener _listener;
    private ILogger _log;
    public DeviceEventsReceiver(ServerSettings serverSettings, ILogger<DeviceEventsReceiver> logger)
    {
        _serverSettings = serverSettings;
        ListeningPort = serverSettings.ListeningPort;
        _listener = StartListening();
        _log = logger;
    }

    private TcpListener StartListening()
    {
        IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
        IPAddress ipAddr = ipHost.AddressList[0];
        var listener = new TcpListener(ipAddr, _serverSettings.ListeningPort);
        listener.Start();
        return listener;
    }

    const int BufferSize = 512;
    public async Task<string> GetNextMessageAsync(CancellationToken cancellationToken)
    {
        _log.LogDebug("Waiting for a connection {localEndPoint}", _listener.LocalEndpoint.ToString());
        TcpClient tcpClient = await _listener.AcceptTcpClientAsync(cancellationToken);
        NetworkStream? stream = null;
        try
        {
            stream = tcpClient.GetStream();
            var bytes = new byte[BufferSize];
            int bytesRead = stream.Read(bytes, 0, bytes.Length);
            if (bytesRead == bytes.Length)
                _log.LogWarning("Read the maximal buffer size {bufferSize}", bytes.Length);
            stream.Close();
            return System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
        }
        catch (System.Exception e)
        {
            _log.LogError(e, e.Message);
            throw;
        }
        finally
        {
            stream?.Close();
            tcpClient?.Close();
            _log.LogDebug("Closed client socket");
        }
    }

    public void Dispose()
    {
        _log.LogDebug("Dispose() called");
        _listener?.Stop();
    }
}
