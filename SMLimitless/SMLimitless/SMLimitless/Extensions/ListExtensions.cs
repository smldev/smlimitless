using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SMLimitless.Extensions
{
    public static class ListExtensions
    {
        public static void AddUnlessDuplicate<T>(this List<T> list, T item)
        {
            if (!list.Contains(item))
            {
                list.Add(item);
            }
        }
    }
}
