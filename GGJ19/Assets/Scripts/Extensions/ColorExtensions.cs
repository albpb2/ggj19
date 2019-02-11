using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class ColorExtensions
    {
        public static Color IncreaseAlpha(this Color color, float alphaToIncrease)
        {
            color.a += alphaToIncrease;

            return color;
        }

        public static Color ReduceAlpha(this Color color, float alphaToIncrease)
        {
            color.a -= alphaToIncrease;

            return color;
        }
    }
}
