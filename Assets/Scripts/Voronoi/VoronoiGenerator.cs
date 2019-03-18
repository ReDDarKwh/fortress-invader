using System.Collections.Generic;
using UnityEngine;

public static class VoronoiGenerator
{
    public static List<Vector2> GetVector2Points(int seed, int number, int max)
    {
        UnityEngine.Random.InitState(seed);
        var points = new List<Vector2>();
        for (int i = 0; i < number; i++)
        {
            points.Add(new Vector2(UnityEngine.Random.Range(0, max), UnityEngine.Random.Range(0, max)));
        }

        // var sa = Random.value * 2 * Mathf.PI;

        // for (var i = 0; i < number; i++)
        // {
        //     var a = sa + Mathf.Sqrt(i) * 5;
        //     var r = (i == 0 ? 0 : 10 + i * (2 + Random.value));
        //     points.Add(new Vector2(Mathf.Cos(a) * r, Mathf.Sin(a) * r));

        // }

        return points;
    }
}
