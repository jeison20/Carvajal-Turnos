using Carvajal.Shifts.Data;
using System;
using System.Data.Entity;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class COrders : ModelContainer
    {
        private static COrders _Instance = new COrders();

        public COrders()
      : base()
        {
        }

        public static COrders Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveOrders(Orders Order)
        {
            try
            {
                Orders.Add(Order);
                Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(Order.FkUsers_Merchant_Identifier,"0", Responses.A0 + "SaveOrders: " + ex.Message);
                return false;
            }
        }

        public Orders SearchOrder(string OrderNumber)
        {
            try
            {
                return Instance.Orders.FirstOrDefault(c => c.OrderNumber == OrderNumber);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteOrder(Orders Order)
        {
            try
            {
                if (Orders.FirstOrDefault(c => c.PkIdentifier.Equals(Order.PkIdentifier)) != null)
                {
                    Orders.Remove(Order);
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