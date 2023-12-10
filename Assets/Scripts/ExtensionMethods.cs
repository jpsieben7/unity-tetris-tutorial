using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.Linq;

public static class ExtensionMethods {
    public static void ForEach<T>(this T[] arr, Action<T> func) {
        Array.ForEach(arr, func);
    }

    public static void ForEachIndexed<T>(this T[] arr, Action<Tuple<int, T>> func) {
        for (int i = 0; i < arr.Length; i++) {
            func(new Tuple<int, T>(i, arr[i]));
        }
    }

    //TODO: Check
    public static T MinBy<T>(this IEnumerable<T> inp, Func<T, int> func) {
        T minItem = inp.FirstOrDefault();
        int minVal = func(minItem);
        foreach(T item in inp) {
            int newVal = func(item);
            if (newVal < minVal) {
                minItem = item;
                minVal = newVal;
            }
        }
        return minItem;
    }
}