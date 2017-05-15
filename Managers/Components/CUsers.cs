using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Managers.Components
{
    public class CUsers : Model
    {
        private static CUsers _Instance = new CUsers();

        public CUsers()
      : base()
        {
        }

        public static CUsers Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveUser(Users User)
        {
            DbContextTransaction dbTx = Instance.Database.BeginTransaction();
            try
            {
                Instance.Database.BeginTransaction();
                Users.Add(User);
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

        public Users SearchUser(long IdentificationNumber)
        {
            try
            {
                return Instance.Users.FirstOrDefault(c => c.PkIdentifier == IdentificationNumber);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteUser(Users User)
        {
            try
            {
                if (Orders.FirstOrDefault(c => c.PkIdentifier.Equals(User.PkIdentifier)) != null)
                {
                    Users.Remove(User);
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
