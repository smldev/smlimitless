using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    public static class StringExtensions
    {
        public static string Substring(this string str, int startIndex, int endIndex)
        {
            int length = endIndex - startIndex;
            return str.Substring(startIndex, length);
        }
    }
}
