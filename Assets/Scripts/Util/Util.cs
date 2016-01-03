using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Util
{
    public static float Wrap(float value, float min = 0, float max = 1)
    {
        var w = max - min;
        var om = value - min;
        return om >= 0 ? om % w + min : w + om % w + min;
    }

    public static float RandomF(float to = 1, float from = 0)
    {
        return Random.value * (to - from) + from;
    }

    public static float RandomFPM(float to = 1)
    {
        return Random.value * to * 2 - to;
    }

    public static float RandomI(int to = 2, int from = 0)
    {
        return Mathf.FloorToInt(Random.value * (to - from)) + from;
    }

    public static int RandomPM()
    {
        return Mathf.FloorToInt(Random.value * 2) * 2 - 1;
    }
}

static class UtilExtension
{
    // e.g. 42.Range().ForEach...
    public static IEnumerable<int> Range(this int count)
    {
        return Enumerable.Range(0, count);
    }

    public static void ForEach<T>(this IEnumerable<T> enumeration, System.Action<T> action)
    {
        foreach (T item in enumeration)
        {
            action(item);
        }
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
}
