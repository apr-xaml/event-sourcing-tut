using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEventStore
{
    public class InMemoryEventStore : IEventStore
    {
        private List<IEvent> _events;

        public InMemoryEventStore(IReadOnlyList<IEvent> events)
        {
            this._events = events.ToList();
            _CreateSnapshotsIfEnough().Wait();
        }

        public InMemoryEventStore()
        {
            _events = new List<IEvent>();
        }

        private async Task _CreateSnapshotsIfEnough()
        {
            throw new NotImplementedException();
        }

        public async Task Add(IEvent ev)
        {
            _events.Add(ev);
            await _CreateSnapshotsIfEnough();
        }

        public async Task AddRange(IReadOnlyList<IEvent> events)
        {
            _events.AddRange(events);
            await _CreateSnapshotsIfEnough();
        }

        public Task<IReadOnlyList<IEvent>> GetEvents(IReadOnlyList<Type> types)
        {
            throw new NotImplementedException();
        }
    }
}
