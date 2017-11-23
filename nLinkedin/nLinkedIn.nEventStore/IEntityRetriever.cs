using nLinkedin;
using nLinkedIn.nEvents.nCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nLinkedIn.nEventStore
{
    public interface IEntityRetriever<TEntity> where TEntity: IWithId
    {
        Task<TEntity> GetEntity(long id);
    }
}
