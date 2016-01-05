using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PixelArtGen.Extensions
{
    static class Extensions
    {
        public static IEnumerable<int> Range(this int count)
        {
            return Enumerable.Range(0, count);
        }

        public static Vector2 Rotate(this Vector2 v, float angle)
        {
            float sin = Mathf.Sin(angle * Mathf.Deg2Rad);
            float cos = Mathf.Cos(angle * Mathf.Deg2Rad);
            float tx = v.x;
            v.x = cos * tx - sin * v.y;
            v.y = sin * tx + cos * v.y;
            return v;
        }

        public static int Wrap(this int value, int min = 0, int max = 1)
        {
            var w = max - min;
            var om = value - min;
            return om >= 0 ? om % w + min : w + om % w + min;
        }
    }
}
