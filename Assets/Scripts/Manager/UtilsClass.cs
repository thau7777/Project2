using UnityEngine;

public static class UtilsClass
{
    public static Vector3 GetRandomDir()
    {
        float angle = Random.Range(0f, 360f);
        float radian = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f);
    }
}
