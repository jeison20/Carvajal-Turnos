using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class CTurns : ModelContainer
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
                LogManager.WriteLog(Turn.FkUsers_Merchant_Identifier, "0", Responses.A0 + "SaveCentres: " + ex.Message);
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
                return Instance.Turns.FirstOrDefault(c => c.Orders_OrderNumber == OrderNumber && c.FkTurnsStatus_Identifier.Equals("1"));
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