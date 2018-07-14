using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;

namespace FateGO.Alter.Managed
{
    static class Extensions
    {

        public static IEnumerable<T> OrderByAlphaNumeric<T>(this IEnumerable<T> source, Func<T, string> selector)
        {
            int max = source
                .SelectMany(i => Regex.Matches(selector(i), @"\d+").Cast<Match>().Select(m => (int?)m.Value.Length))
                .Max() ?? 0;
            return source.OrderBy(i => Regex.Replace(selector(i), @"\d+", m => m.Value.PadLeft(max, '0')));
        }

        internal static void ForEach<T>(this IEnumerable<T> ie, Action<T> action)
        {
            foreach (var i in ie)
            {
                action(i);
            }
        }

        internal static IEnumerable<T> Distinct<T, TKey>(this IEnumerable<T> source, Func<T, TKey> selector)
        {
            return source.Distinct(new CompareSelector<T, TKey>(selector));
        }

        internal static string Join(this string[] self, string separater)
        {
            return string.Join(separater, self);
        }

#if DATAMINE
        internal static bool Equals(this DropInfo source, DropInfo dinfo)
        {
            return (source.objectId == dinfo.objectId
                && source.type == dinfo.type
                && source.num == dinfo.num
                && source.lv == dinfo.lv
                && source.rarity == dinfo.rarity);
        }
#endif
    }

    public class CompareSelector<T, TKey> : IEqualityComparer<T>
    {
        private Func<T, TKey> selector;

        public CompareSelector(Func<T, TKey> selector)
        {
            this.selector = selector;
        }

        public bool Equals(T x, T y)
        {
            return selector(x).Equals(selector(y));
        }

        public int GetHashCode(T obj)
        {
            return selector(obj).GetHashCode();
        }
    }
}
