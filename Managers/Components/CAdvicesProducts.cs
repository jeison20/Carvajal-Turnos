using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers.Components
{
 public   class CAdvicesProducts : Model
    {
        private static CAdvicesProducts _Instance = new CAdvicesProducts();

        public CAdvicesProducts()
      : base()
        {
        }

        public static CAdvicesProducts Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveAdvicesProduct(AdvicesProducts AdvicesProduct)
        {
            try
            {

                AdvicesProducts.Add(AdvicesProduct);
                Instance.SaveChanges();        
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error en el metodo SaveAdvicesProduct" + ex.Message);
                return false;
            }
        }

        public List<AdvicesProducts> SearchAdvicesProducts(long FkAdvices_Identifier)
        {
            try
            {
                return Instance.AdvicesProducts.Where(c => c.FkAdvices_Identifier == FkAdvices_Identifier).ToList();
            }
            catch
            {
                return null;
            }
        }
        public AdvicesProducts SearchAdvicesProductsForId(int Identifier)
        {
            try
            {
                return Instance.AdvicesProducts.FirstOrDefault(c => c.PkIdentifier == Identifier);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteAdvicesProduct(AdvicesProducts AdvicesProduct)
        {
            try
            {
                if (AdvicesProducts.FirstOrDefault(c => c.PkIdentifier.Equals(AdvicesProduct.PkIdentifier)) != null)
                {
                    AdvicesProducts.Remove(AdvicesProduct);
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
