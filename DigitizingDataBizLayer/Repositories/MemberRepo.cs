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
    public class MemberRepo: RepositoryBase<Member>
    {
        public Member FindMemberById(int memberId)
        {
            var member = (from m in SessionProxy.Query<Member>()
                          where m.MemberId == memberId
                          select m).FirstOrDefault();
            return member;
        }

        //Find a Member by the Id as is on the Phone
        public Member FindMemberByIdEx(string vslaCode, int memberIdEx)
        {
            var member = (from m in SessionProxy.Query<Member>()
                          where m.MemberIdEx == memberIdEx && m.Vsla.VslaCode == vslaCode
                          select m).FirstOrDefault();
            return member;
        }

        //Find a Member by the Id as is on the Phone
        public Member FindMemberByIdEx(int vslaId, int memberIdEx)
        {
            var member = (from m in SessionProxy.Query<Member>()
                          where m.MemberIdEx == memberIdEx && m.Vsla.VslaId == vslaId
                          select m).FirstOrDefault();
            return member;
        }
    }
}
