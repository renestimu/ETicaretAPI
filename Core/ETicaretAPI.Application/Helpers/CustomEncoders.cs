using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETicaretAPI.Application.Helpers
{
    static public  class CustomEncoders
    {
        public static string UrlEncode(this string value)
        {
            byte[] Bytes = Encoding.UTF8.GetBytes(value);
            return WebEncoders.Base64UrlEncode(Bytes);
        }
        public static string UrlDecode(this string value)
        {
            byte[] Bytes = WebEncoders.Base64UrlDecode(value);
            return Encoding.UTF8.GetString(Bytes);
        }
    }
}
