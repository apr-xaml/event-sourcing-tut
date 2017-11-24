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
using nLinkedin;

namespace nLinkedIn.nUserAccountRetriever
{
    public class UserAccountRetriever : IEntityRetriever<LinkedInAccount>, IEventStoreWritable
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

        public async Task<IReadOnlyList<Result<OptimisticConcurrencyErrorDesc>>> AddRange(IReadOnlyList<IEvent> events)
        {

            await _evStore.AddRange(events);

            var grouped = events.Cast<EndorsementAddedOrRemovedEvent>()
                .GroupBy(x => x.TargetId)
                .ToList();

            var results = grouped.Select
                (
                    async iG =>
                       {
                           var snapshot = await _CreateSnapshot(iG.Key, iG.ToList());
                           var leftSnapshot = _snaphotsByIdMap.AddOrUpdate(iG.Key, snapshot, (xId, xExisting) => _ChooseSnapshot(snapshot, xExisting));
                           if (leftSnapshot != snapshot)
                           {
                               return Result<OptimisticConcurrencyErrorDesc>.Error(new OptimisticConcurrencyErrorDesc());
                           }
                           else
                           {
                               return Result<OptimisticConcurrencyErrorDesc>.Ok;
                           }
                       }
                ).ToList();

            return await Task.WhenAll(results);
        }

        private Task<Snapshot<LinkedInAccount>> _CreateSnapshot(long key, List<EndorsementAddedOrRemovedEvent> list)
        {
            throw new NotImplementedException();
        }

        private Snapshot<LinkedInAccount> _ChooseSnapshot(object snapshot, Snapshot<LinkedInAccount> xExisting)
        {
            throw new NotImplementedException();
        }


        private Snapshot<LinkedInAccount> _ChooseSnapshot(Snapshot<LinkedInAccount> snapshot, Snapshot<LinkedInAccount> xExisting)
        {
            throw new NotImplementedException();
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


        private async Task<Snapshot<LinkedInAccount>> _CreateSnapshot(long targetId, IReadOnlyList<EndorsementAddedOrRemovedEvent> newEvents)
        {



            var latestSnapshot = await Task.Run(() => _snaphotsByIdMap.GetOrAdd(targetId, x => _CreateSnapshotFromStart(x, LinkedInAccount.ZeroState()).Result));

            var importantEventsNotIncluded = await _evStore.GetEvents(_observedEventTypes, skip: latestSnapshot.EventsCount, exPredicate: _GetPredicate(targetId));



            var notIncludedCount = (importantEventsNotIncluded.Count - latestSnapshot.EventsCount);

            var newSnap = latestSnapshot.WithNewEvents(importantEventsNotIncluded);

            return newSnap;
        }

        private async Task<Snapshot<LinkedInAccount>> _CreateSnapshotFromStart(long targetId, LinkedInAccount zeroStateAccount)
            
        {
            var allEvents = await _evStore.GetAllEvents(_observedEventTypes);
            var filledAccount = zeroStateAccount.WithAppliedEvents(allEvents);


            return new Snapshot<LinkedInAccount>(filledAccount, allEvents.Count, Snapshot<LinkedInAccount>.ZERO_VERSION);
        }
    }
}

