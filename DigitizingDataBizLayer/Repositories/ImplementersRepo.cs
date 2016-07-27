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
    public class ImplementersRepo : RepositoryBase<Implementers>
    {
        // List of all project implementers
        public List<Implementers> findAllImplementers()
        {
            var implemeters = (from i in SessionProxy.Query<Implementers>()
                               select i).ToList();
            return implemeters;
        }

        // Find an implemeter by Id
        public Implementers findImplementerById(int ImplementerId)
        {
            var implementer = (from i in SessionProxy.Query<Implementers>()
                               where i.ImplementerId == ImplementerId
                               select i).FirstOrDefault();
            return implementer;
        }
    }
}
