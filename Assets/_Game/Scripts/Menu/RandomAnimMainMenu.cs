using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimMainMenu : MonoBehaviour
{
    [SerializeField] private List<Animator> animators;

    [SerializeField] private float minDelay = 5;
    [SerializeField] private float maxDelay = 10;

    public void StartAnim()
    {
        RandomAnimIcon().Forget();
    }
    private async UniTask RandomAnimIcon()
    {
        List<int> lstIndexAvailable = new List<int>();
        for (int i = 0; i < animators.Count; i++)
        {
            lstIndexAvailable.Add(i);
        }
        while (true)
        {
            if (lstIndexAvailable.Count == 0)
            {
                // Reset the list if all indices have been used
                for (int i = 0; i < animators.Count; i++)
                {
                    lstIndexAvailable.Add(i);
                }
            }
            int randomIndex = Random.Range(0, lstIndexAvailable.Count);
            int selectedIndex = lstIndexAvailable[randomIndex];
            lstIndexAvailable.RemoveAt(randomIndex); // Remove the selected id to avoid repetition

            Debug.Log($"Playing animation for animator at id: {selectedIndex}");

            if (animators != null && animators[selectedIndex] != null)
            {
                animators[selectedIndex].Play("Run");
            }

            float delay = Random.Range(minDelay, maxDelay);
            await UniTask.Delay((int)(delay * 1000));
        }
    }
}
