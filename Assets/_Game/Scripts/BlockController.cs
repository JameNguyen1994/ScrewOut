using UnityEngine;

public class BlockController : Singleton<BlockController>
{
    [SerializeField] private int countBlock;
    [SerializeField] private GameObject gobjBlock;

    public void AddBlockLayer()
    {
        countBlock++;
        gobjBlock.SetActive(true);
        EditorLogger.Log(">>>> AddBlockLayer " + countBlock);
    }

    public void RemoveBlockLayer()
    {
        EditorLogger.Log(">>>> RemoveBlockLayer " + countBlock);
        if (countBlock > 0)
        {
            countBlock--;
            if (countBlock == 0)
            {
                gobjBlock.SetActive(false);
            }
        }
    }

    public bool IsLock()
    {
        EditorLogger.Log(">>>> IsLock " + countBlock);
        return countBlock > 0;
    }

    public void RemoveAllBlockLayer()
    {
        EditorLogger.Log(">>>> RemoveAllBlockLayer " + countBlock);
        countBlock = 0;
        gobjBlock.SetActive(false);
    }
}
