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
            try
            {
                Advices.Add(Advice);
                Instance.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error en el metodo SaveAdvices " + ex.Message);

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
