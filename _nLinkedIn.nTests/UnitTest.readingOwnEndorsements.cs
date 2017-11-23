using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nLinkedIn.nAccount;
using System.Linq;

namespace _nLinkedIn.nTests
{

    public partial class UnitTest
    {

        [TestMethod]
        public void User_can_read_their_own_endorsements()
        {

            var evStore = _GetEvStoreWithSomeEndorsements(endorsementsCount: 100, skillId: WellKnowSkillIds.Unix, targetId: UserBillGates.Id);


           
        }

   




    }
}
