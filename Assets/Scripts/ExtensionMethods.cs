using UnityEngine;
using System.Collections;
using System;

public static class ExtensionMethods {
    public static void ForEach<T>(this T[] arr, Action<T> func) {
        Array.ForEach(arr, func);
    }

    public static void ForEachIndexed<T>(this T[] arr, Action<Tuple<int, T>> func) {
        for (int i = 0; i < arr.Length; i++) {
            func(new Tuple<int, T>(i, arr[i]));
        }
    }
}