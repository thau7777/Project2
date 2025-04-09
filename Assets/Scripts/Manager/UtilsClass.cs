using System;
using UnityEngine;

public static class UtilsClass
{
    private static System.Random random = new System.Random();
    public static Vector3 GetRandomDir()
    {
        float angle = UnityEngine.Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f);
    }

    

    public static T GetRandomValue<T>(T[] levels, float[] probabilities)
    {
        if (levels.Length != probabilities.Length)
            throw new ArgumentException("level count and ratio count must be equals");

        float total = 0;
        foreach (var prob in probabilities)
            total += prob;

        float rand = (float)random.NextDouble() * total;
        float cumulative = 0;

        for (int i = 0; i < levels.Length; i++)
        {
            cumulative += probabilities[i];
            if (rand <= cumulative)
                return levels[i];
        }

        return levels[levels.Length - 1]; // return last level if error
    }
}
