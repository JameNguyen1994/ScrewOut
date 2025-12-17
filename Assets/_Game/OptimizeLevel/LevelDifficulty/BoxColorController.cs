using System.Collections.Generic;
using UnityEngine;

public enum BoxDifficultyType
{
    Ease = 1,
    Medium = 2,
    Hard = 3
}

public class BoxColorController : Singleton<BoxColorController>
{
    public ScrewColor CheckDifficultyAndGetBox(List<ScrewColor> lstLowerPriority)
    {
        Debug.Log("[BoxColorController] ===== Start CheckDifficultyAndGetBox =====");

        var lstScrew = LevelController.Instance.GetListScrewOnTray();
        Debug.Log($"[BoxColorController] Screws on tray: {lstScrew.Count}");

        var lstColor = lstScrew.SelectProperty(x => x.ScrewColor, x => x != null);
        Debug.Log($"[BoxColorController] Colors from tray: {string.Join(", ", lstColor)}");

        var lstColorCount = lstColor.CountAndSortDescending();
        foreach (var item in lstColorCount)
            Debug.Log($"[BoxColorController] Color: {item.item}, Count: {item.count}");

        var lstCountColor3 = lstColorCount.SelectProperty(x => x.item, x => x.count >= 3);
        Debug.Log($"[BoxColorController] Colors with >=3 screws: {string.Join(", ", lstCountColor3)}");

        if (lstCountColor3.Count > 0)
        {
            var chosenColor = lstCountColor3.GetRandom();
            Debug.Log($"[BoxColorController] Chosen Color (>=3 screws): {chosenColor}");
            return chosenColor;
        }

        List<int> lstDifficulty = new List<int>() { 1, 0, 0 };

        int process = (int)(ScreenGamePlayUI.Instance.LevelProgressBar.GetProgress() * 100);
        lstDifficulty = LevelDifficultyManager.Instance.GetPercents(process);
        Debug.Log($"[BoxColorController] Process: {process}, Difficulty Percents: {string.Join(", ", lstDifficulty)}");

        int index = lstDifficulty.GetIndexByWeight();
        var diff = (BoxDifficultyType)(index + 1); // enum bắt đầu từ 1
        if (diff <= 0) diff = BoxDifficultyType.Ease;

        Debug.Log($"[BoxColorController] Randomed Difficulty: {diff}");

        var screwColor = GetScrewColorByDifficulty(diff, lstLowerPriority);
        var lstColorString = string.Join(", ", lstLowerPriority);
        Debug.Log($"[BoxColorController] Final Pick -> Difficulty: {diff}, Color: {screwColor}, LowerPriority: {lstColorString}");

        Debug.Log("[BoxColorController] ===== End CheckDifficultyAndGetBox =====");
        return screwColor;
    }

    private ScrewColor GetScrewColorByDifficulty(BoxDifficultyType boxDifficultyType, List<ScrewColor> lstLowerPriority)
    {
        Debug.Log($"[BoxColorController] ---- GetScrewColorByDifficulty: {boxDifficultyType} ----");

        var dic = ScrewBlockedRealTimeController.Instance.DicCurrBlockedScrew;

        var lst3Color = new List<ScrewColor>();
        var lst21Color = new List<ScrewColor>();
        var lst0Color = new List<ScrewColor>();

        foreach (var pair in dic)
        {
            Debug.Log($"[BoxColorController] Color: {pair.Key}, Blocked: {pair.Value}");
            if (pair.Value >= 3)
                lst3Color.Add(pair.Key);
            else if (pair.Value >= 1 && pair.Value <= 2)
                lst21Color.Add(pair.Key);
            else
                lst0Color.Add(pair.Key);
        }

        Debug.Log($"[BoxColorController] Before RemoveLowerPriority - 3+: {string.Join(", ", lst3Color)}, 1-2: {string.Join(", ", lst21Color)}, 0: {string.Join(", ", lst0Color)}");

        RemoveLowerPriority(lst3Color, lstLowerPriority);
        RemoveLowerPriority(lst21Color, lstLowerPriority);
        RemoveLowerPriority(lst0Color, lstLowerPriority);

        Debug.Log($"[BoxColorController] After RemoveLowerPriority - 3+: {string.Join(", ", lst3Color)}, 1-2: {string.Join(", ", lst21Color)}, 0: {string.Join(", ", lst0Color)}");

        ScrewColor result;
        switch (boxDifficultyType)
        {
            case BoxDifficultyType.Ease:
                result = PickColor(lst3Color, lst21Color, lst0Color, lstLowerPriority);
                break;
            case BoxDifficultyType.Medium:
                result = PickColor(lst21Color, lst3Color, lst0Color, lstLowerPriority);
                break;
            case BoxDifficultyType.Hard:
                result = PickColor(lst0Color, lst21Color, lst3Color, lstLowerPriority);
                break;
            default:
                result = ScrewColor.None;
                break;
        }

        Debug.Log($"[BoxColorController] Picked Color by Difficulty {boxDifficultyType}: {result}");
        return result;
    }

    private ScrewColor PickColor(
        List<ScrewColor> primary,
        List<ScrewColor> secondary,
        List<ScrewColor> tertiary,
        List<ScrewColor> lowerPriority)
    {
        Debug.Log($"[BoxColorController] PickColor -> Primary: {string.Join(", ", primary)}, Secondary: {string.Join(", ", secondary)}, Tertiary: {string.Join(", ", tertiary)}, LowerPriority: {string.Join(", ", lowerPriority)}");

        if (primary.Count > 0)
        {
            var pick = primary[Random.Range(0, primary.Count)];
            Debug.Log($"[BoxColorController] Picked from Primary: {pick}");
            return pick;
        }
        if (secondary.Count > 0)
        {
            var pick = secondary[Random.Range(0, secondary.Count)];
            Debug.Log($"[BoxColorController] Picked from Secondary: {pick}");
            return pick;
        }
        if (tertiary.Count > 0)
        {
            var pick = tertiary[Random.Range(0, tertiary.Count)];
            Debug.Log($"[BoxColorController] Picked from Tertiary: {pick}");
            return pick;
        }
        Debug.Log("[BoxColorController] No color found, returning None");
        return ScrewColor.None;
    }

    private void RemoveLowerPriority(List<ScrewColor> list, List<ScrewColor> lowerPriority)
    {
        if (list == null || lowerPriority == null) return;
        if (list.Count == 0) return;

        int overlapCount = list.FindAll(c => lowerPriority.Contains(c)).Count;
        Debug.Log($"[BoxColorController] RemoveLowerPriority -> List: {string.Join(", ", list)}, Overlap: {overlapCount}, LowerPriority: {string.Join(", ", lowerPriority)}");

        if (overlapCount < list.Count)
        {
            list.RemoveAll(c => lowerPriority.Contains(c));
            Debug.Log($"[BoxColorController] After Remove: {string.Join(", ", list)}");
        }
        else
        {
            Debug.Log("[BoxColorController] Kept list (all elements in lowerPriority)");
        }
    }
}
