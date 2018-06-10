using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM4DesignTool.Utilities
{
    public static class StringExtension
    {
        public static void DisplayNow(this string source)
        {
            Console.WriteLine(source);
        }

        public static string SelectString(this string source, int startIndex, int endIndex)
        {
            if (startIndex > -1 && startIndex < endIndex)
            {
                return source.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                return string.Empty;
            }
        }
    }
}
