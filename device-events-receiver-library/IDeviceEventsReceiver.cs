using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace device_events_receiver_library
{
    public interface IDeviceEventsReceiver
    {
        int ListeningPort { get; }
        public Task<string> GetNextMessageAsync(CancellationToken cancellationToken);
        public void Process(string received);
    }
}