using UnityEngine;

namespace Assets.Scripts.Extensions
{
    public static class Vector3Extensions
    {
        public static Vector3 AddX(this Vector3 vector, float deltaX)
        {
            return vector + new Vector3(deltaX, 0, 0);
        }

        public static Vector3 AddY(this Vector3 vector, float deltaY)
        {
            return vector + new Vector3(0, deltaY, 0);
        }

        public static Vector3 AddZ(this Vector3 vector, float deltaZ)
        {
            return vector + new Vector3(0, deltaZ, 0);
        }
    }
}
