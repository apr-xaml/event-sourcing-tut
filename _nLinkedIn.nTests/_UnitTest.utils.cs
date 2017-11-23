using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nLinkedIn.nAccount;
using nLinkedIn.nEventStore;
using System.Linq;
using nLinkedIn.nEndorsedOther;

namespace _nLinkedIn.nTests
{
    [TestClass]
    public partial class UnitTest
    {

        public IEventStore DefaultEmptyOnStartEventStore;

        public LinkedInAccount UserJanKowaski;
        public LinkedInAccount UserBillGates;

        static public class WellKnowSkillIds
        {
            static public long Windows => 100;
            static public long Unix => 200;

        }

        [TestInitialize]
        public void _Init()
        {
            this.DefaultEmptyOnStartEventStore = new InMemoryEventStore();

            this.UserJanKowaski = new LinkedInAccount(id: 10, firstName: "Jan", lastName: "Kowalski", eventStore: DefaultEmptyOnStartEventStore);
            this.UserBillGates = new LinkedInAccount(id: 20, firstName: "Bill", lastName: "Gates", eventStore: DefaultEmptyOnStartEventStore);
        }


        private IEventStore _GetEvStoreWithSomeEndorsements(int endorsementsCount, long skillId, long targetId)
        {
            var now = DateTime.Now;

            var events = Enumerable.Range(0, endorsementsCount).Select(xi => EndorsementAddedOrRemovedEvent.Added(30 + xi, targetId, WellKnowSkillIds.Unix, now)).ToList();

            return new InMemoryEventStore(events);
        }
    }
}
