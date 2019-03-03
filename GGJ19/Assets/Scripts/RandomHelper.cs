using System;

namespace Assets.Scripts.Extensions
{
    public static class RandomHelper
    {
        public static Random _random = new Random();

        public static bool IsProbabilityReached100(int probability)
        {
            return _random.Next(0, 100) < probability;
        }

        public static bool IsProbabilityReached1(float probabiliy)
        {
            return _random.NextDouble() < probabiliy;
        }
    }
}