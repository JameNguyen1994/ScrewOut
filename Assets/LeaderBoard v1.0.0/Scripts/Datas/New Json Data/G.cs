using UnityEngine;
using System.Collections.Generic;

public static class G   // Generator
{
    public static void Gen(
        M month,
        int c, int minP, int maxP,
        List<string> names,
        int ac, int bc,
        bool desc)
    {
        month.u.Clear();

        for (int i = 0; i < c; i++)
        {
            bool zero = Random.value < 0.1f;
            bool rndName = Random.value < 0.3f;

            string name = (!rndName && names.Count > 0)
                ? names[Random.Range(0, names.Count)]
                : $"P{i + 1}";

            month.u.Add(new U
            {
                n = name,
                p = zero ? 0 : Random.Range(minP, maxP + 1),
                a = ac > 0 ? Random.Range(0, ac) : -1,
                b = bc > 0 ? Random.Range(0, bc) : -1
            });
        }

        if (desc)
            month.u.Sort((x, y) => y.p.CompareTo(x.p));
        else
            month.u.Sort((x, y) => x.p.CompareTo(y.p));
    }
}
