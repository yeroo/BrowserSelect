using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace browser_select.Helpers
{
    public class RegestryHelper
    {

        public static string GetSiteName()
        {
            var retVal = string.Empty;
            var args = Environment.GetCommandLineArgs();
            if (args.Length > 1)
            {
                var url = args[1];
                // get host name from url
                var uriAddress = new Uri(url);
                retVal = uriAddress.Host;

            }
            return retVal;
        }
    }
}
