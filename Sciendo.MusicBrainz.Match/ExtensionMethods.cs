using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sciendo.FilesAnalyser
{
    public static class ExtensionMethods
    {

        public static string CleanString(this string inString)
        {
            if(string.IsNullOrEmpty(inString))
                return inString;
            return inString.Replace('\v', ' ');
        }

    }
}
