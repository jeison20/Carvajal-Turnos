using Carvajal.Shifts.Data;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace Managers.Components
{
    public class COrdersProducts : Model
    {
        private static COrdersProducts _Instance = new COrdersProducts();

        public COrdersProducts()
      : base()
        {
        }

        public static COrdersProducts Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveOrdersProducts(OrdersProducts OrderProduct)
        {
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                OrdersProducts.Add(OrderProduct);
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

        public List<OrdersProducts> SearchOrdersProducts(int OrderNumber)
        {
            try
            {
                return Instance.OrdersProducts.Where(c => c.FkOrders_Identifier == OrderNumber).ToList();
            }
            catch
            {
                return null;
            }
        }

        public OrdersProducts SearchOrderProduct(int OrderProductId)
        {
            try
            {
                return Instance.OrdersProducts.FirstOrDefault(c => c.PkIdentifier == OrderProductId);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteOrdersProducts(OrdersProducts OrderProduct)
        {
            try
            {
                if (Orders.FirstOrDefault(c => c.PkIdentifier.Equals(OrderProduct.PkIdentifier)) != null)
                {
                    OrdersProducts.Remove(OrderProduct);
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