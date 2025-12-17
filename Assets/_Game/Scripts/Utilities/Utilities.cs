using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    public static string ConvertSecondToString(int second)
    {
        if (second <= 0) return "0s";

        int d = second / 86400; second %= 86400;
        int h = second / 3600; second %= 3600;
        int m = second / 60; second %= 60;

        if (d > 0) return $"{d}d {h}h";
        if (h > 0) return $"{h}h {m}m";
        if (m > 0) return $"{m}m {second}s";
        return $"{second}s";
    }
}
