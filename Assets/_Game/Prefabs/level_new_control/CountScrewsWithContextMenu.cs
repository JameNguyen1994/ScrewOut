
using UnityEngine;

public class CountScrewsWithContextMenu : MonoBehaviour
{
    [ContextMenu("Count Screws in Children")]
    void CountScrews()
    {
        int screwCount = CountChildObjectsWithName(gameObject, "screw");
        Debug.Log($"Number of child objects with 'screw' in their name: {screwCount}");
    }

    int CountChildObjectsWithName(GameObject parent, string keyword)
    {
        int count = 0;

        foreach (Transform child in parent.GetComponentsInChildren<Transform>(true))
        {
            if (child.name.ToLower().Contains(keyword.ToLower()))
            {
                count++;
            }
        }

        return count;
    }
}
