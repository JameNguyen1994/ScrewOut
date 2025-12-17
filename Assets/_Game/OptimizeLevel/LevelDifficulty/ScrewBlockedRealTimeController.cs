using Storage;
using System.Collections.Generic;
using System.Xml;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class ScrewBlockedRealTimeController : Singleton<ScrewBlockedRealTimeController>
{
    [SerializeField] private Dictionary<ScrewColor, int> dicTotalScrew = new Dictionary<ScrewColor, int>();
    [SerializeField] private Dictionary<ScrewColor, int> dicCurrScrew = new Dictionary<ScrewColor, int>();
    [SerializeField] private Dictionary<ScrewColor, int> dicCurrScrewResolved = new Dictionary<ScrewColor, int>();
    [SerializeField] private Dictionary<ScrewColor, int> dicTotalBlockedScrew = new Dictionary<ScrewColor, int>();
    [SerializeField] private Dictionary<ScrewColor, int> dicCurrBlockedScrew = new Dictionary<ScrewColor, int>();

    [SerializeField] private List<Screw> lstAdded = new List<Screw>();

    public Dictionary<ScrewColor, int> DicBlockTotalBlockedScrew { get => dicTotalBlockedScrew; }
    public Dictionary<ScrewColor, int> DicCurrBlockedScrew { get => dicCurrBlockedScrew; }
    public Dictionary<ScrewColor, int> DicCurrScrewResolved { get => dicCurrScrewResolved; }
    public Dictionary<ScrewColor, int> DicTotalScrew { get => dicTotalScrew; }
    public Dictionary<ScrewColor, int> DicCurrScrew { get => dicCurrScrew; }
    public string UniqueId => Define.LEVEL_CONTROLLER_ID;
    public void Init()
    {
        dicTotalBlockedScrew = new Dictionary<ScrewColor, int>();
        dicCurrBlockedScrew = new Dictionary<ScrewColor, int>();
        dicTotalScrew = new Dictionary<ScrewColor, int>();
        dicCurrScrew = new Dictionary<ScrewColor, int>();
    }
    public void AddTotalScrew(ScrewColor color)
    {
        if (dicTotalScrew.ContainsKey(color))
        {
            dicTotalScrew[color]++;
        }
        else
        {
            dicTotalScrew[color] = 1;
            dicCurrScrew[color] = 0;
        }
    }
    public void AddCurrScrewResolved(ScrewColor color)
    {
        if (dicCurrScrewResolved.ContainsKey(color))
        {
            dicCurrScrewResolved[color]++;
        }
        else
        {
            dicCurrScrewResolved[color] = 1;
        }
    }

    public void AddBlockedScrew(Screw screw)
    {
        if (screw == null || lstAdded.Contains(screw))
        {
            return;
        }
        lstAdded.Add(screw);
        var screwColor = screw.ScrewColor;
        Debug.Log($"[ScrewBlockedRealTimeController] AddBlockedScrew: {screwColor}");
        if (dicTotalBlockedScrew.ContainsKey(screwColor))
        {
            dicTotalBlockedScrew[screwColor]++;
        }
        else
        {
            dicTotalBlockedScrew[screwColor] = 1;
        }
        if (IsFullAtColor(screwColor))
        {
            return;
        }
        if (dicCurrBlockedScrew.ContainsKey(screwColor))
        {
            dicCurrBlockedScrew[screwColor]++;
        }
        else
        {
            dicCurrBlockedScrew[screwColor] = 1;
        }
    }
    public void RemoveBlockedScrew(ScrewColor color)
    {

        if (dicCurrBlockedScrew.ContainsKey(color))
        {
            Debug.Log($"[ScrewBlockedRealTimeController] RemoveBlockedScrew: {color}, Before: {dicCurrBlockedScrew[color]}");
            dicCurrBlockedScrew[color] -= 3;
            if (dicCurrScrew.ContainsKey(color))
            {
                dicCurrScrew[color] += 3;
            }
            else
            {
                dicCurrScrew[color] = 3;
            }
            if (dicTotalScrew.ContainsKey(color) && dicTotalScrew[color] == dicCurrScrew[color])
            {
                dicCurrBlockedScrew.Remove(color);
            }
            // Debug.Log($"[ScrewBlockedRealTimeController] RemoveBlockedScrew: {color}, After: {dicCurrBlockedScrew[color]}");
            /*if (dicCurrBlockedScrew[color] <= 0)
            {
                dicCurrBlockedScrew[color] = 0;
            }*/
        }
    }
    private bool IsFullAtColor(ScrewColor color)
    {
        return dicTotalScrew.ContainsKey(color) && dicTotalScrew[color] == dicCurrScrew[color];
    }
    public bool IsFullAll()
    {
        foreach (var kvp in dicTotalScrew)
        {
            var color = kvp.Key;
            var total = kvp.Value;
            var curr = dicCurrScrew.ContainsKey(color) ? dicCurrScrew[color] : 0;
            if (total != curr)
            {
                return false;
            }
        }
        return true;
    }


    [EasyButtons.Button]
    public void Serialize()
    {
        var data = new ScrewBlockedRealTimeData();

        data.UniqueId = UniqueId;
        data.DicBlockTotalBlockedScrew = dicTotalBlockedScrew;
        data.DicCurrBlockedScrew = dicCurrBlockedScrew;
        data.DicCurrScrewResolved = dicCurrScrewResolved;
        data.DicTotalScrew = dicTotalScrew;
        data.DicCurrScrew = dicCurrScrew;

        Db.storage.ScrewBlockedRealTimeData = data;
    }

    public void InitFormSave()
    {
        var data = Db.storage.ScrewBlockedRealTimeData;
        dicCurrBlockedScrew = data.DicCurrBlockedScrew ?? new Dictionary<ScrewColor, int>();
        dicTotalBlockedScrew = data.DicBlockTotalBlockedScrew ?? new Dictionary<ScrewColor, int>();
        dicCurrScrewResolved = data.DicCurrScrewResolved ?? new Dictionary<ScrewColor, int>();
        dicTotalScrew = data.DicTotalScrew ?? new Dictionary<ScrewColor, int>();
        dicCurrScrew = data.DicCurrScrew ?? new Dictionary<ScrewColor, int>();
    }
}
public class ScrewBlockedRealTimeData
{
    public string UniqueId;

    public Dictionary<ScrewColor, int> DicBlockTotalBlockedScrew;
    public Dictionary<ScrewColor, int> DicCurrBlockedScrew;
    public Dictionary<ScrewColor, int> DicCurrScrewResolved;
    public Dictionary<ScrewColor, int> DicTotalScrew;
    public Dictionary<ScrewColor, int> DicCurrScrew;

    public ScrewBlockedRealTimeData()
    {

    }
}