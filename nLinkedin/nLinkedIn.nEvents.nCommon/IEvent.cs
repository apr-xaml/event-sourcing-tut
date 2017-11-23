using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEvents.nCommon
{
    public interface IEvent
    {
        DateTime EnteredSystemAt { get; }
    }
}
