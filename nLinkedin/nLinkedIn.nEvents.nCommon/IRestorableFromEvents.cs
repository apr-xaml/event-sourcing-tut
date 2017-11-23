using nLinkedin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEvents.nCommon
{
    public interface IRestorableFromEvents<TEntity> where TEntity : IWithId
    {
        TEntity WithAppliedEvents(IReadOnlyList<IEvent> events);
    }
}
