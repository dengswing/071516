using System.Collections.Generic;
using UnityEngine;

public static class CatCommonTools
{
    static public List<T> RandomContent<T>(List<T> data, int min, int max)
    {
        if (data == null) return null;
        var randomList = new List<T>();
        var tmpList = new List<T>(data.ToArray());
        var length = Random.Range(min, max + 1);
        if (length >= data.Count) length = data.Count;
        for (int i = 0; i < length; i++)
        {
            var index = Random.Range(0, tmpList.Count);
            var value = tmpList[index];
            tmpList.RemoveAt(index);
            randomList.Add(value);
        }

        return randomList;
    }
}