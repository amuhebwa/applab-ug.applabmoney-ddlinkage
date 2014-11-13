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
    public class DataSubmissionRepo : RepositoryBase<DataSubmission>
    {
        public List<DataSubmission> RetrieveSubmissions()
        {
            var submissions = (from s in SessionProxy.Query<DataSubmission>()
                               orderby s.SubmissionId ascending
                               select s).ToList();
            return submissions;
        }

        public List<DataSubmission> RetrieveUnProcessedSubmissions()
        {
            var submissions = (from s in SessionProxy.Query<DataSubmission>()
                               where s.ProcessedFlag != true
                               orderby s.SubmissionId ascending
                               select s).ToList();
            return submissions;
        }

        public DataSubmission GetMostRecentDataSubmission()
        {
            var dataSubmission = (from s in SessionProxy.Query<DataSubmission>()
                              orderby s.SubmissionId descending
                              select s).First();            
            return dataSubmission;
        }        
    }
}
