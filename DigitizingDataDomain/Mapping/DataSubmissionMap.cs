using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Mapping;
using FluentNHibernate.Mapping;
using DigitizingDataDomain.Model;

namespace DigitizingDataDomain.Mapping
{
    public class DataSubmissionMap : ClassMap<DataSubmission>
    {
        public DataSubmissionMap()
        {
            Id(d => d.SubmissionId);
            Map(d => d.SourceVslaCode).Length(20);
            Map(d => d.SourcePhoneImei).Length(20);
            Map(d => d.SourceNetworkOperator).Length(50);
            Map(d => d.SourceNetworkType).Length(20);
            Map(d => d.SubmissionTimestamp);
            Map(d => d.Data).Length(2147483647).CustomSqlType("nvarchar(max)");
            Map(d => d.ProcessedFlag);
        }
    }
}
