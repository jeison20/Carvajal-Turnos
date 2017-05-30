using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class COrdersProducts : ModelContainer
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

            try
            {

                OrdersProducts.Add(OrderProduct);
                Instance.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("0","0",Responses.A0+"SaveOrdersProducts: " + ex.Message);
                return false;
            }
        }

        public List<OrdersProducts> SearchOrdersProducts(long OrderNumber)
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