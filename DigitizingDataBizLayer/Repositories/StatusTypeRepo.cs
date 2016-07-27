using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;
using NHibernate.Criterion;
namespace DigitizingDataBizLayer.Repositories
{
    public class StatusTypeRepo : RepositoryBase<StatusType>
    {
        public List<StatusType> findAllStatusType()
        {
            var statusTypes = (from s in SessionProxy.Query<StatusType>()
                               select s).ToList();
            return statusTypes;
        }
    }
}
