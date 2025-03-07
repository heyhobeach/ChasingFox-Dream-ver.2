using System.Collections.Generic;
using Unity.Collections;

public static class NativeListExtensions
{
    public static bool Contains<T>(this NativeList<T> list, T value) where T : unmanaged
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].Equals(value)) return true;
        }
        return false;
    }

    public static void Reverse<T>(this NativeList<T> list) where T : unmanaged
    {
        int start = 0;
        int end = list.Length - 1;

        while (start < end)
        {
            T temp = list[start];
            list[start] = list[end];
            list[end] = temp;

            start++;
            end--;
        }
    }
    public static bool Remove<T>(this NativeList<T> list, T value) where T : unmanaged
    {
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].Equals(value))
            {
                list.RemoveAtSwapBack(i);
                return true;
            }
        }
        return false;
    }
    public static List<T> ToList<T>(this NativeList<T> nativeList) where T : unmanaged
    {
        List<T> list = new List<T>(nativeList.Length);
        for (int i = 0; i < nativeList.Length; i++)
        {
            list.Add(nativeList[i]);
        }
        return list;
    }
}