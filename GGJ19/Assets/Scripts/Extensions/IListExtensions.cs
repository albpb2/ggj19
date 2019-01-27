using System;
using System.Collections.Generic;

namespace Assets.Scripts.Extensions
{
    public static class IListExtensions
    {
        private static Random _random = new Random();

        public static T GetRandomElement<T>(this IList<T> collection)
        {
            var index = _random.Next(0, collection.Count);

            return collection[index];
        }
    }
}
