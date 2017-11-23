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
            await _evStore.Add(ev);
            var snapshot = await _CreateSnapshotIfEnough();
            _snaphotsByIdMap[snapshot.Entity.Id] = snapshot;
        }



        public async Task AddRange(IReadOnlyList<IEvent> events)
        {
            await _evStore.AddRange(events);
            var snapshot = await _CreateSnapshotIfEnough();
            _snaphotsByIdMap[snapshot.Entity.Id] = snapshot;
        }

        public async Task<LinkedInAccount> GetEntity(long id)
        {
            var snapshot = await _GetLatestSnapshot(accountId: id);
            var notAppliedEvents = await _evStore.GetEvents(types: _observedEventTypes);

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

        private async Task<Snapshot<LinkedInAccount>> _GetLatestSnapshot(long accountId)
        {
            var snaphot = _snaphotsByIdMap.GetOrAdd(accountId, (xId) => _CreateSnapshotIfEnough(xId).Result);
            return snaphot;
        }

        private async Task<Snapshot<LinkedInAccount>> _CreateSnapshotIfEnough(long accountId)
        {

            Expression<Func<IEvent, bool>> exPredicate = (x) => (((EndorsementAddedOrRemovedEvent)x).TargetId == accountId);

            var latestSnapshot = await _GetLatestSnapshot(accountId);

            var importantEventsNotIncluded = await _evStore.GetEvents(_observedEventTypes, skip: latestSnapshot.EventsCount, exPredicate: exPredicate);



            var notIncludedCount = (importantEventsNotIncluded.Count - latestSnapshot.EventsCount);

            if (notIncludedCount >= _snapshotSize)
            {
                var newSnap = latestSnapshot.WithNewEvents(importantEventsNotIncluded);

                return newSnap;
            }
        }

    }
}
