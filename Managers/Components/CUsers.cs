
using Carvajal.Shifts.Data;
using System;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class CUsers : ModelContainer
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
                LogManager.WriteLog(User.FkCompanies_Identifier, "0", Responses.A0+"SaveUser. " + ex.Message);
                return false;
            }
        }

        public Users SearchUser(string IdentificationNumber)
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