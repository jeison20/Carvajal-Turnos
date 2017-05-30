using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carvajal.Turns.Utils.I18n
{
    interface II18N
    {
        Response GetMessage(string country, int code, object data, string parameter);
    }
}
