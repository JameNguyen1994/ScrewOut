using UnityEngine;
using System;
using System.Collections.Generic;

public static class ScrewManager
{
    private static readonly Dictionary<string, Screw> screws = new Dictionary<string, Screw>();

    public static IReadOnlyCollection<Screw> Screws => screws.Values;

    public static void AddScrew(Screw screw)
    {
        if (screw == null || string.IsNullOrEmpty(screw.UniqueId))
            return;

        screws[screw.UniqueId] = screw;
    }

    public static void RemoveScrew(Screw screw)
    {
        if (screw == null || string.IsNullOrEmpty(screw.UniqueId))
            return;

        screws.Remove(screw.UniqueId);
    }

    public static Screw GetScrewById(string id)
    {
        if (string.IsNullOrEmpty(id))
            return null;

        screws.TryGetValue(id, out var screw);

        if (screw == null)
        {
            Debug.LogError("[GetScrewById] Can not find Screw");

            List<Screw> screws = LevelController.Instance.Level.LstScrew;

            if (screws != null && screws.Count > 0)
            {
                for (int i = 0; i < screws.Count; i++)
                {
                    if (screws[i] != null && screws[i].UniqueId == id)
                    {
                        return screws[i];
                    }
                }
            }
        }

        return screw;
    }

    public static void ExecuteOnScrew(string id, Action<Screw> action)
    {
        if (string.IsNullOrEmpty(id) || action == null)
            return;

        if (screws.TryGetValue(id, out var screw))
        {
            action(screw);
        }
    }
}