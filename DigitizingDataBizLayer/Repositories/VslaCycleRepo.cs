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
    public class VslaCycleRepo:RepositoryBase<VslaCycle>
    {
        public VslaCycle FindVslaCycleByIdEx(string vslaCode, int cycleIdEx)
        {
            var cycle = (from c in SessionProxy.Query<VslaCycle>()
                         where c.CycleIdEx == cycleIdEx && c.Vsla.VslaCode.ToUpper() == vslaCode.ToUpper()
                         select c).FirstOrDefault();
            return cycle;
        }

        public VslaCycle FindVslaCycleByIdEx(int vslaId, int cycleIdEx)
        {
            var cycle = (from c in SessionProxy.Query<VslaCycle>()
                         where c.CycleIdEx == cycleIdEx && c.Vsla.VslaId == vslaId
                         select c).FirstOrDefault();
            return cycle;
        }

        public bool VslaCycleExists(VslaCycle vslaCycle)
        {
            if(vslaCycle == null || vslaCycle.Vsla == null)
            {
                return false;
            }

            var cycle = (from c in SessionProxy.Query<VslaCycle>()
                         where c.CycleIdEx == vslaCycle.CycleIdEx && c.Vsla.VslaId == vslaCycle.Vsla.VslaId
                         select c).FirstOrDefault();

            return (cycle != null);
        }
    }
}
