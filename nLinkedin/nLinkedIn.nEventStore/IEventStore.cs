using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEventStore
{

    public interface IEventStoreWrite
    {
        Task Add(IEvent ev);

    }

    public interface IEventStoreRead
    {
        Task<IReadOnlyList<IEvent>> GetEvents(IReadOnlyList<Type> types, int skip, Expression<Func<IEvent, bool>> exPredicate);
    }


    public interface IEventStore : IEventStoreRead, IEventStoreWrite
    {

    }
}
