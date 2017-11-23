using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using nLinkedIn.nAccount;
using System.Linq;
using System.Threading.Tasks;

namespace _nLinkedIn.nTests
{

    public partial class UnitTest
    {

        [TestMethod]
        public void User_starts_with_no_events()
        {
            Assert.IsFalse(UserJanKowaski.EndorsedSomeoneElseEvents.Any());
        }


        [TestMethod]
        public async Task User_can_endorse_other_user()
        {

            var res =await UserJanKowaski.EndorseOther(UserBillGates.Id, skillId: WellKnowSkillIds.Unix);

            Assert.IsTrue(res.IsOk);

            var ev = UserJanKowaski.EndorsedSomeoneElseEvents.Single();

            Assert.IsTrue(ev.IsDescribedBy(UserBillGates.Id, WellKnowSkillIds.Unix, byWhomId: UserJanKowaski.Id));
        }


        [TestMethod]
        public async Task User_cannot_endorse_twice_same_user_with_same_skill()
        {

            var resOk = await UserJanKowaski.EndorseOther(UserBillGates.Id, skillId: WellKnowSkillIds.Unix);
            var resNotOk = await UserJanKowaski.EndorseOther(UserBillGates.Id, skillId: WellKnowSkillIds.Unix);

            Assert.IsTrue(resOk.IsOk, "ass1");
            Assert.IsFalse(resNotOk.IsOk, "ass2");
            Assert.IsTrue(resNotOk.ExplanationIfNotOk.IsDescribedBy(UserBillGates.Id, WellKnowSkillIds.Unix, byWhomId: UserJanKowaski.Id));

            var ev = UserJanKowaski.EndorsedSomeoneElseEvents.Single();

            Assert.IsTrue(ev.IsDescribedBy(UserBillGates.Id, WellKnowSkillIds.Unix, byWhomId: UserJanKowaski.Id), "ass3");

        }


        [TestMethod]
        public async Task User_cannot_endorse_twice_same_user_but_with_different_skill()
        {

            var resOkUnix = await UserJanKowaski.EndorseOther(UserBillGates.Id, skillId: WellKnowSkillIds.Unix);
            var resOkWindows = await UserJanKowaski.EndorseOther(UserBillGates.Id, skillId: WellKnowSkillIds.Windows);

            Assert.IsTrue(resOkUnix.IsOk);
            Assert.IsTrue(resOkWindows.IsOk);

            var evUnix = UserJanKowaski.EndorsedSomeoneElseEvents.Single(x => x.SkillId == WellKnowSkillIds.Unix);
            var evWindows = UserJanKowaski.EndorsedSomeoneElseEvents.Single(x => x.SkillId == WellKnowSkillIds.Windows);

            Assert.IsTrue(evUnix.IsDescribedBy(UserBillGates.Id, WellKnowSkillIds.Unix, byWhomId: UserJanKowaski.Id));
            Assert.IsTrue(evWindows.IsDescribedBy(UserBillGates.Id, WellKnowSkillIds.Windows, byWhomId: UserJanKowaski.Id));
        }


    }
}
