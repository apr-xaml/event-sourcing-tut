using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace nLinkedIn.nEventStore
{
    public class InMemoryEventStore : IEventStore
    {
        private List<IEvent> _events;

        public InMemoryEventStore(IReadOnlyList<IEvent> events)
        {
            this._events = events.ToList();

        }

        public InMemoryEventStore()
        {
            _events = new List<IEvent>();
        }



        public async Task Add(IEvent ev) => _events.Add(ev);





        public Task<IReadOnlyList<IEvent>> GetEvents(IReadOnlyList<Type> types, int skip, Expression<Func<IEvent, bool>> exPredicate)
        {
            var oxPred = exPredicate.Compile();

            IReadOnlyList<IEvent> filtered = _events
                .Where(x => types.Contains(x.GetType()))
                .Where(x => oxPred(x))
                .Skip(skip)
                .ToList();

            return Task.FromResult(filtered);
        }
    }
}
