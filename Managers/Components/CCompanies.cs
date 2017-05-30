using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class CCompanies : ModelContainer
    {
        private static CCompanies _Instance = new CCompanies();

        public CCompanies()
      : base()
        {
        }

        public static CCompanies Instance
        {
            get
            {
                return _Instance;
            }
        }

        public bool SaveCompanies(Companies Company)
        {

            try
            {

                Companies.Add(Company);
                Instance.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(Company.Companies_Identifier, "0", Responses.A0+"SaveCompanies: " + ex.Message);
                return false;
            }
        }

   

        public Companies SearchCompaniesForId(string Identifier)
        {
            try
            {
                return Instance.Companies.FirstOrDefault(c => c.PkIdentifier == Identifier);
            }
            catch
            {
                return null;
            }
        }

        public bool DeleteCompanies(Companies Company)
        {
            try
            {
                if (Companies.FirstOrDefault(c => c.PkIdentifier.Equals(Company.PkIdentifier)) != null)
                {
                    Companies.Remove(Company);
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