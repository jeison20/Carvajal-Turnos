using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Carvajal.Turns.Utils.I18n
{
    public class ResponseCodes
    {
        public List<Codes> Codes { get; set; }
    }

    public class Codes
    {
        public int Code { get; set; }
        public List<CountryResponse> Countries { get; set; }
    }

    public class CountryResponse
    {
        public string CountryCode { get; set; }
        public string Message { get; set; }
    }
}
