
using UnityEngine;

public class MoveChildObjectsWithContextMenu : MonoBehaviour
{
    public GameObject parent1; // Object cha 1
    public GameObject parent2; // Object cha 2

    [ContextMenu("Move Children to Parent2")]
    void MoveChildObjectsByParentName()
    {
        if (parent1 == null || parent2 == null)
        {
            Debug.LogError("Please assign both Parent1 and Parent2.");
            return;
        }

        foreach (Transform child1 in parent1.transform)
        {
            foreach (Transform child2 in parent2.transform)
            {
                if (child1.name == child2.name) // Kiểm tra tên giống nhau
                {
                    Debug.Log($"Moving children of {child1.name} from {parent1.name} to {parent2.name}");

                    // Duyệt các object cháu của child1
                    foreach (Transform grandChild in child1)
                    {
                        grandChild.SetParent(child2); // Đổi cha của object cháu thành child2
                        Debug.Log($"Moved {grandChild.name} to {child2.name}");
                    }
                }
            }
        }
    }
}
