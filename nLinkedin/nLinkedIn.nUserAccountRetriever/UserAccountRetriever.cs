using nLinkedIn.nAccount;
using nLinkedIn.nEventStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using nLinkedIn.nEvents.nCommon;
using nLinkedIn.nEndorsedOther;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace nLinkedIn.nUserAccountRetriever
{
    public class UserAccountRetriever : IEntityRetriever<LinkedInAccount>, IEventStoreWrite
    {

        private readonly ConcurrentDictionary<long, Snapshot<LinkedInAccount>> _snaphotsByIdMap = new ConcurrentDictionary<long, Snapshot<LinkedInAccount>>();

        private readonly int _snapshotSize;
        private readonly IEventStore _evStore;

        IReadOnlyList<Type> _observedEventTypes = new[] { typeof(EndorsementAddedOrRemovedEvent) };

        public UserAccountRetriever(IEventStore evStore, int snapshotSize = 10)
        {
            _snapshotSize = snapshotSize;
            _evStore = evStore;
        }

        public async Task Add(IEvent ev)
        {
            switch (ev)
            {
                case EndorsementAddedOrRemovedEvent endEv:
                    {
                        await _evStore.Add(endEv);
                        var snapshot = await _CreateSnapshotIfEnough(endEv.TargetId);
                        _snaphotsByIdMap[snapshot.Entity.Id] = snapshot;
                        break;
                    }
                default:
                    throw new InvalidOperationException();
            }

            await _evStore.Add(ev);

        }


        Expression<Func<IEvent, bool>> _GetPredicate(long targetId)
            => (x) => (((EndorsementAddedOrRemovedEvent)x).TargetId == targetId);




        public async Task<LinkedInAccount> GetEntity(long id)
        {
            var snapshot = await _GetLatestSnapshot(accountId: id);

            var exPred = _GetPredicate(id);

            var notAppliedEvents = await _evStore.GetEvents(_observedEventTypes, skip: snapshot.EventsCount, exPredicate: exPred);

            if (!notAppliedEvents.Any())
            {
                return snapshot.Entity;
            }
            else
            {
                var withThoseMissing = snapshot.Entity.WithAppliedEvents(notAppliedEvents);
                return withThoseMissing;
            }

        }

        private Task<Snapshot<LinkedInAccount>> _GetLatestSnapshot(long accountId)
        {
            //The line below could be wrapped in Task::Start
            var snaphot = _snaphotsByIdMap.GetOrAdd(accountId, (xId) => _CreateSnapshotIfEnough(xId).Result);
            return Task.FromResult(snaphot);
        }

        private async Task<Snapshot<LinkedInAccount>> _CreateSnapshotIfEnough(long accountId)
        {



            var latestSnapshot = await _GetLatestSnapshot(accountId);

            var importantEventsNotIncluded = await _evStore.GetEvents(_observedEventTypes, skip: latestSnapshot.EventsCount, exPredicate: _GetPredicate(accountId));



            var notIncludedCount = (importantEventsNotIncluded.Count - latestSnapshot.EventsCount);

            var newSnap = latestSnapshot.WithNewEvents(importantEventsNotIncluded);

            return newSnap;
        }

    }
}
