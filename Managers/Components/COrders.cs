using Carvajal.Shifts.Data;
using System.Data.Entity;
using System.Linq;

namespace Managers.Components
{
    public class COrders : Model
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
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                Orders.Add(Order);
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