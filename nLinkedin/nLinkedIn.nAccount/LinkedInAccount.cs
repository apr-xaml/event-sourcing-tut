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
        private readonly List<EndorsementAddedOrRemovedEvent> _endorsedOtherEvents = new List<EndorsementAddedOrRemovedEvent>();

        public long Id { get; }
        public string FirstName { get; }
        public string LastName { get; }

        private IEventStore _eventStore;

        public IReadOnlyList<EndorsementAddedOrRemovedEvent> EndorsedSomeoneElseEvents => _endorsedOtherEvents;

        public Result<EndorsementAddedOrRemovedEvent> EndorseOther(long targetId, long skillId)
        {

            var now = DateTime.Now;

            var (canEndorse, endorsedEv) = _CanEndorseOther(targetId, skillId);

            if (!canEndorse)
            {
                return Result<EndorsementAddedOrRemovedEvent>.Error(endorsedEv);
            }

            var ev = EndorsementAddedOrRemovedEvent.Added(this.Id, targetId, skillId, enteredSystemAt: now);

            _endorsedOtherEvents.Add(ev);
            _eventStore.Add(ev);

            return Result<EndorsementAddedOrRemovedEvent>.Ok;
        }

        private (bool, EndorsementAddedOrRemovedEvent) _CanEndorseOther(long targetId, long skillId)
        {
            var already = _endorsedOtherEvents.SingleOrDefault(x => x.IsDescribedBy(targetId, skillId, byWhomId: this.Id));

            if (already == null)
            {
                return (true, null);
            }
            else
            {
                return (false, already);
            }
        }

        public LinkedInAccount WithAppliedEvents(IReadOnlyList<IEvent> events)
        {
            throw new NotImplementedException();
        }

        public LinkedInAccount(int id, string firstName, string lastName, IEventStore eventStore)
        {
            this.Id = id;
            this.FirstName = firstName;
            this.LastName = lastName;
            _eventStore = eventStore;
        }


    }
}
