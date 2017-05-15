using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers.Components
{
    public class CAdvices : Model
    {
        private static CAdvices _Instance = new CAdvices();

        public CAdvices()
      : base()
        {
        }

        public static CAdvices Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveAdvices(Advices Advice)
        {
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                Advices.Add(Advice);
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

        public Advices SearchAdvices(string OrderNumber)
        {
            try
            {
                return Instance.Advices.FirstOrDefault(c => c.Orders_OrderNumber == OrderNumber);
            }
            catch
            {
                return null;
            }
        }
        public Advices SearchAdvicesForId(int Identifier)
        {
            try
            {
                return Instance.Advices.FirstOrDefault(c => c.PkIdentifier == Identifier);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteAdvices(Advices Advice)
        {
            try
            {
                if (Advices.FirstOrDefault(c => c.PkIdentifier.Equals(Advice.PkIdentifier)) != null)
                {
                    Advices.Remove(Advice);
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
