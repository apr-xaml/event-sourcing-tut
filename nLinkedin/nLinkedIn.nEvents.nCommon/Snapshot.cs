using nLinkedin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEvents.nCommon
{
    public class Snapshot<TEntity> where TEntity : IWithId
    {
        public static readonly long ZERO_VERSION;

        public Snapshot(TEntity entity, int eventsCount, long version)
        {
            Entity = entity;
            EventsCount = eventsCount;
            Version = version;
        }

  
        public int EventsCount { get; }
        public long Version { get; }
        public TEntity Entity { get; }

        public Snapshot<TEntity> WithNewEvents(IReadOnlyList<IEvent> notIncluded)
        {
            throw new NotImplementedException();
        }
    }
}
