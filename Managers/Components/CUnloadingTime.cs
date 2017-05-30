using Carvajal.Shifts.Data;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Carvajal.Turns.CodeResponses;

namespace Managers.Components
{
    public class CUnloadingTime : ModelContainer
    {
        private static CUnloadingTime _Instance = new CUnloadingTime();

        public CUnloadingTime()
      : base()
        {
        }

        public static CUnloadingTime Instance
        {
            get
            {
                return _Instance;
            }
        }
        public bool SaveUnloadingTime(UnloadingTime UnloadingTimeToSave)
        {

            try
            {

                UnloadingTime.Add(UnloadingTimeToSave);
                Instance.SaveChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogManager.WriteLog(UnloadingTimeToSave.FkCentres_Identifier,"0", Responses.A0 + "SaveUnloadingTime: " + ex.Message);
                return false;
            }
        }

    }
}