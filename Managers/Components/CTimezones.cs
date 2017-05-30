using Carvajal.Shifts.Data;
using System;
using System.Data.Entity;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class CTimezones : ModelContainer
    {
        private static CTimezones _Instance = new CTimezones();

        public CTimezones()
      : base()
        {
        }

        public static CTimezones Instance
        {
            get
            {
                return _Instance;
            }
        }
        public Timezones SearchTimezonesForCode(string Code)
        {
            try
            {
                return Instance.Timezones.FirstOrDefault(c => c.Code == Code);
            }
            catch
            {
                return null;
            }
        }
    }
}