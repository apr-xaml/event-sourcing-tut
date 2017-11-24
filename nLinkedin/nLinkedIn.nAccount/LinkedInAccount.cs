using nLinkedin;
using nLinkedIn.nEndorsedOther;
using nLinkedIn.nEvents.nCommon;
using nLinkedIn.nEventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nAccount
{
    public class LinkedInAccount : IWithId, IRestorableFromEvents<LinkedInAccount>
    {
        private readonly List<EndorsementAddedOrRemovedEvent> _localEvents = new List<EndorsementAddedOrRemovedEvent>();

        public long Id { get; }
        public string FirstName { get; }
        public string LastName { get; }


        int _restoredFromEventsCount;


        public IReadOnlyList<EndorsementAddedOrRemovedEvent> LocalEvents => _localEvents;

        public static LinkedInAccount ZeroState() => new LinkedInAccount();

        public Result<EndorsementAddedOrRemovedEvent> EndorseOther(long targetId, long skillId)
        {

            var now = DateTime.Now;

            var (canEndorse, previousEndorsedEv) = _CanEndorseOther(targetId, skillId);

            if (!canEndorse)
            {
                return Result<EndorsementAddedOrRemovedEvent>.Error(previousEndorsedEv);
            }

            var ev = EndorsementAddedOrRemovedEvent.Added(this.Id, targetId, skillId, enteredSystemAt: now);

 

            _localEvents.Add(ev);

            return Result<EndorsementAddedOrRemovedEvent>.Ok;
        }

        private (bool, EndorsementAddedOrRemovedEvent) _CanEndorseOther(long targetId, long skillId)
        {
            var already = _localEvents.SingleOrDefault(x => x.IsDescribedBy(targetId, skillId, byWhomId: this.Id));

            if (already == null)
            {
                return (true, null);
            }
            else
            {
                return (false, already);
            }
        }


        public Task SaveChanges(IEventStoreWritable writer) => writer.AddRange(_localEvents);
   

        public LinkedInAccount WithAppliedEvents(IReadOnlyList<IEvent> events)
        {
            throw new NotImplementedException();
        }

        private LinkedInAccount()
        {
            
            _restoredFromEventsCount = 0;
        }


    }
}
