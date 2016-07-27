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
    public class TechnicalTrainerRepo : RepositoryBase<TechnicalTrainer>
    {
        public TechnicalTrainer checkIfTrainerExists(string Username, String Passkey)
        {
            var trainer = (from t in SessionProxy.Query<TechnicalTrainer>()
                           where t.Username == Username && t.Passkey == Passkey
                           select t).FirstOrDefault();
            return trainer;

        }

        // All Technical Trainers
        public List<TechnicalTrainer> findAllTechnicalTrainers()
        {
            var trainers = (from t in SessionProxy.Query<TechnicalTrainer>()
                            select t).ToList();
            return trainers;
        }

        // Details for a particular technical trainer
        public TechnicalTrainer findParticularTrainer(int id)
        {
            var trainer = (from t in SessionProxy.Query<TechnicalTrainer>()
                           where t.Id == id
                           select t).FirstOrDefault();
            return trainer;
        }
    }
}
