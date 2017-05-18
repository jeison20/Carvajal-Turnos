using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Managers.Components
{
    public class CTurns : Model
    {
        private static CTurns _Instance = new CTurns();

        public CTurns()
      : base()
        {
        }

        public static CTurns Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveTurn(Turns Turn)
        {
            try
            {
                Turns.Add(Turn);
                Instance.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error en el metodo SaveCentres" + ex.Message);
                return false;
            }
        }

        public List<Turns> SearchStartDateTime(DateTime Date)
        {
            try
            {
                Date = new DateTime(Date.Year, Date.Month, Date.Day, 1, 0, 0);
                return Instance.Turns.Where(c => c.StartDateTime > Date).ToList();
            }
            catch
            {
                return null;
            }
        }

        public Turns SearchTurnsForOrderActive(string OrderNumber)
        {
            try
            {
                return Instance.Turns.FirstOrDefault(c => c.Orders_OrderNumber == OrderNumber && c.Status.Equals("A"));
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteTurn(Turns Turn)
        {
            try
            {
                if (Turns.FirstOrDefault(c => c.PkIdentifier.Equals(Turn.PkIdentifier)) != null)
                {
                    Turns.Remove(Turn);
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