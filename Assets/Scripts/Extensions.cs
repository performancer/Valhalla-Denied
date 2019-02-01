using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static void DestroyChildren(this Transform transform)
    {
        int children = transform.childCount;

        for (int i = children - 1; i >= 0; i--)
            GameObject.Destroy(transform.GetChild(i).gameObject);
    }
}

