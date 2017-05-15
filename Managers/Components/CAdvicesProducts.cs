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
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                AdvicesProducts.Add(AdvicesProduct);
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

        public List<AdvicesProducts> SearchAdvicesProducts(int FkAdvices_Identifier)
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
