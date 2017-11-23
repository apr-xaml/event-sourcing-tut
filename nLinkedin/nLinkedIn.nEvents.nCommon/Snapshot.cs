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

        //Do I need 'version'? If events are linear and append-only, then EventsCount uniquely identifies Snapshot?

        public int EventsCount { get; }

        public TEntity Entity { get; }

        public Snapshot<TEntity> WithNewEvents(IReadOnlyList<IEvent> notIncluded)
        {
            throw new NotImplementedException();
        }
    }
}
