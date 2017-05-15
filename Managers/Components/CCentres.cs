using Carvajal.Shifts.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Managers.Components
{
    public class CCentres : Model
    {
        private static CCentres _Instance = new CCentres();

        public CCentres()
      : base()
        {
        }

        public static CCentres Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveCentres(Centres Center)
        {
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                Centres.Add(Center);
                Instance.SaveChanges();
                dbTx.Commit();
                return true;
            }
            catch
            {
                dbTx.Rollback();
                return false;
            }
        }

        public List<Centres> SearchCentres()
        {
            try
            {
                return Instance.Centres.Where(c => c.Status == true).ToList();
            }
            catch
            {
                return null;
            }
        }

        public Centres SearchCentresForId(long Identifier)
        {
            try
            {
                return Instance.Centres.FirstOrDefault(c => c.PkIdentifier == Identifier);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteAdvices(Centres Center)
        {
            try
            {
                if (Centres.FirstOrDefault(c => c.PkIdentifier.Equals(Center.PkIdentifier)) != null)
                {
                    Centres.Remove(Center);
                    Instance.SaveChanges();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}