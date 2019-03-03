using System;

namespace Assets.Scripts.Maths
{
    public static class Probabilities
    {
        private static Random random = new Random();

        public static bool CalculateSuccessBase1(float probability)
        {
            return random.NextDouble() < probability;
        }

        public static bool CalculateSuccessBase100(int probability)
        {
            return random.Next(0, 100) < probability;
        }
    }
}
