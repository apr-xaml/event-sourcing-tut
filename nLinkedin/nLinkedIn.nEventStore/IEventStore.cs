using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEventStore
{

    public interface IEventStoreWritable
    {
        Task AddRange(IReadOnlyList<IEvent> events);

    }

    public interface IEventStoreReadable
    {
        Task<IReadOnlyList<IEvent>> GetEvents(IReadOnlyList<Type> types, int skip, Expression<Func<IEvent, bool>> exPredicate);
        Task<IReadOnlyList<IEvent>> GetAllEvents(IReadOnlyList<Type> observedEventTypes);
    }


    public interface IEventStore : IEventStoreReadable, IEventStoreWritable
    {
   
    }
}
