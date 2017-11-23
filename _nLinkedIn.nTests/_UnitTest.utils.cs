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

        static public IEventStore DefaultEmptyOnStartEventStore = new InMemoryEventStore();

        static public LinkedInAccount UserJanKowaski = new LinkedInAccount(id: 10, firstName: "Jan", lastName: "Kowalski", eventStore: DefaultEmptyOnStartEventStore);
        static public LinkedInAccount UserBillGates = new LinkedInAccount(id: 20, firstName: "Bill", lastName: "Gates", eventStore: DefaultEmptyOnStartEventStore);

        static public class WellKnowSkillIds
        {
            static public long Windows => 100;
            static public long Unix => 200;

        }



        private IEventStore _GetEvStoreWithSomeEndorsements(int endorsementsCount, long skillId, long targetId)
        {
            var now = DateTime.Now;

            var events =  Enumerable.Range(0, endorsementsCount).Select(xi => new EndorsementAddedOrRemovedEvent(30 + xi, targetId, WellKnowSkillIds.Unix, now)).ToList();

            return new InMemoryEventStore(events);
        }
    }
}
