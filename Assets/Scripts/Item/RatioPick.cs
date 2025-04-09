using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatioPick
{
    private static System.Random random = new System.Random();

    public static T GetRandomLevel<T>(T[] levels, float[] probabilities)
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
