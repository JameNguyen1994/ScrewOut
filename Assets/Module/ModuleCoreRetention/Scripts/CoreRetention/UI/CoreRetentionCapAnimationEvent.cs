using UnityEngine;

public class CoreRetentionCapAnimationEvent : MonoBehaviour
{
    public CoreRetentionCap coreRetentionCap;
    //public Animator animator;

    public void PlayEffectDestroyGlass()
    {
        coreRetentionCap.PlayEffectDestroyGlass();
    }

    public void EndAnim()
    {
        //animator.GetComponent<Animator>().enabled = false;
    }
}