using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    void Start()
    {
        try
        {
            PerformanceService.Initialize();
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }

        SceneManager.LoadSceneAsync("Loading");
    }
}