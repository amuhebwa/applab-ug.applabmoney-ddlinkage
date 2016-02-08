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
    public class MemberRepo : RepositoryBase<Member>
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

        // count the number of male members
        public long countMaleMembers()
        {
            var maleMembers = (from m in SessionProxy.Query<Member>()
                               where m.Gender == "Male"
                               select m).Count();
            return maleMembers;
        }

        // Count the number of female members
        public long countFemaleMembers()
        {
            var femaleMembers = (from m in SessionProxy.Query<Member>()
                                 where m.Gender == "Female"
                                 select m).Count();
            return femaleMembers;
        }

        // Find all members attached to a given VSLA
        public List<Member> findAllMembersByVslaId(int vslaId)
        {
            var members = (from m in SessionProxy.Query<Member>()
                           where m.Vsla.VslaId == vslaId
                           select m).ToList();
            return members;
        }

        // /find the total number of members attached to a particular group
        public int totalGroupMembers(int vslaId)
        {
            int total = (from m in SessionProxy.Query<Member>()
                         where m.Vsla.VslaId == vslaId
                         select m).Count();
            return total;
        }

        // Number of women in a group
        public int numbeOfWomenInGroup(int vslaId)
        {
            int females = (from m in SessionProxy.Query<Member>()
                           where m.Vsla.VslaId == vslaId
                           && m.Gender == "Female"
                           select m).Count();
            return females;
        }

        // Percentage of women in the group
        //public string percentageOfWomenPerGroup(int vslaId)
        //{
        //    int females = (from m in SessionProxy.Query<Member>()
        //                   where m.Vsla.VslaId == vslaId && m.Gender == "Female"
        //                   select m).Count();

        //    int total = (from m in SessionProxy.Query<Member>()
        //                 where m.Vsla.VslaId == vslaId
        //                 select m).Count();

        //    double result = ((double)females / (double)total) * 100;
        //    return Convert.ToString(Math.Round(result, 2));
        //}
    }
}
