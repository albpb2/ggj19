using System;

namespace Assets.Scripts.Player
{
    public class Player
    {
        private const int MinKarma = 0;
        private const int MaxKarma = 100;

        public int Karma { get; private set; }

        public void IncreaseKarma(int quantity)
        {
            Karma = Math.Min(Karma + quantity, MaxKarma);
        }

        public void ReduceKarma(int quantity)
        {
            Karma = Math.Max(Karma - quantity, MinKarma);
        }
    }
}
