using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEndorsedOther
{
    public class EndorsementAddedOrRemovedEvent : IEvent
    {
        public long ByWhomId { get; }
        public long TargetId { get; }
        public long SkillId { get; }

        public EndorsementAddeOrRemovedKindEnum Kind { get; }

        public DateTime EnteredSystemAt { get; }


        static public EndorsementAddedOrRemovedEvent Removed(long byWhomId, long targetId, long skillId, DateTime enteredSystemAt)
            => new EndorsementAddedOrRemovedEvent(byWhomId, targetId, skillId, enteredSystemAt, kind: EndorsementAddeOrRemovedKindEnum.Removed);

        static public EndorsementAddedOrRemovedEvent Added(long byWhomId, long targetId, long skillId, DateTime enteredSystemAt)
           => new EndorsementAddedOrRemovedEvent(byWhomId, targetId, skillId, enteredSystemAt, kind: EndorsementAddeOrRemovedKindEnum.Added);

        private EndorsementAddedOrRemovedEvent(long byWhomId, long targetId, long skillId, DateTime enteredSystemAt, EndorsementAddeOrRemovedKindEnum kind)
        {
            this.ByWhomId = byWhomId;
            this.TargetId = targetId;
            this.SkillId = skillId;

            this.EnteredSystemAt = enteredSystemAt;

        }

        public bool IsDescribedBy(long targetId, long skillId, long byWhomId)
         => (this.ByWhomId == byWhomId) && (this.SkillId == skillId) && (this.TargetId == targetId);

        
    }


    public enum EndorsementAddeOrRemovedKindEnum
    {
        Added,
        Removed
    }
}
