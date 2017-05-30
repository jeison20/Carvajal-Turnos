using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Carvajal.Turns.Utils;

namespace Carvajal.Turns.Utils.I18n
{
    public class I18N : II18N
    {
        public Response GetMessage(string country, int code, object data, string parameter)
        {
            try
            {
                ResponseCodes responseCodes;
                Response response = null;
                using (var jsonCodes = new StreamReader(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "I18n\\I18n.json")))
                {
                    var json = jsonCodes.ReadToEnd();
                    jsonCodes.Close();
                    responseCodes = JsonConvert.DeserializeObject<ResponseCodes>(json);
                }

                foreach (var codes in responseCodes.Codes)
                {

                    if (!(codes.Code.Equals(code))) continue;
                    foreach (var countrCodey in codes.Countries)
                    {
                        if (countrCodey.CountryCode.ToUpper().Equals(country))
                        {
                            response = new Response
                            {
                                Code = code,
                                Message = countrCodey.Message
                               .Replace("[COUNTRY]", country)
                               .Replace("[MESSAGE]", parameter),
                                Data = data,
                                ServerCurrentDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")
                            };
                        }
                    }

                    foreach (var countrCodey in codes.Countries)
                    {
                        if (!countrCodey.CountryCode.Equals("default")) continue;
                        response = new Response
                        {
                            Code = code,
                            Message = countrCodey.Message
                            .Replace("[COUNTRY]", country)
                            .Replace("[MESSAGE]", parameter),
                            Data = data,
                            ServerCurrentDate = DateTime.Now.ToString("yyyy-MM-ddThh:mm:ss")
                        };
                    }
                }
                return response;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.Message);
            }

        }


    }
}
