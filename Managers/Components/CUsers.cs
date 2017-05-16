using Carvajal.Shifts.Data;
using System;
using System.Linq;

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
            try
            {
                Users.Add(User);
                Instance.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog("Error en el metodo SaveUser " + ex.Message);
                return false;
            }
        }

        public Users SearchUser(string IdentificationNumber)
        {
            try
            {
                return Instance.Users.FirstOrDefault(c => c.PkIdentifier == IdentificationNumber);
            }
            catch (Exception ex)
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