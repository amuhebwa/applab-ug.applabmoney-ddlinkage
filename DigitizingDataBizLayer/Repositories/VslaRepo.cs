using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NHibernate;
using NHibernate.Linq;
using DigitizingDataDomain.Model;

namespace DigitizingDataBizLayer.Repositories
{
    public class VslaRepo : RepositoryBase<Vsla>
    {
        public Vsla FindVslaByCode(string vslaCode)
        {
            var vsla = (from v in SessionProxy.Query<Vsla>()
                           where v.VslaCode.ToUpper() == vslaCode.ToUpper()
                           select v).FirstOrDefault();            
            return vsla;
        }
    }
}
