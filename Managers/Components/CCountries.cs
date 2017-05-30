using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Managers.Components
{
    class CCountries : ModelContainer
    {
        private static CCountries _Instance = new CCountries();

        public CCountries()
      : base()
        {
        }

        public static CCountries Instance
        {
            get
            {
                return _Instance;
            }
        }
        public Countries SearchCountriesForCode(string Code)
        {
            try
            {
                return Instance.Countries.FirstOrDefault(c => c.Code == Code);
            }
            catch
            {
                return null;
            }
        }

    }
}
