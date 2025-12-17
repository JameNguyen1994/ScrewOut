using System.Collections.Generic;
using UnityEngine;

public class ModelRocket : MonoBehaviour
{
    [SerializeField] private ParticleSystem parTrail;
    [SerializeField] private ParticleSystem parExplode;

    [SerializeField] private List<GameObject> lstGobjFire;
    [SerializeField] private Transform tfmRocket;

    public void PlayParTrail()
    {
        if (parTrail != null)
        {
            parTrail.Play();
        }
    }
    public void PlayParDestroy()
    {
        PoolManager.Instance.Spawn(PoolKey.SOF_SHAPE_DESTROY, transform.position, transform.rotation);
        tfmRocket.gameObject.SetActive(false);
        for (int i = 0; i < lstGobjFire.Count; i++)
        {
            lstGobjFire[i].SetActive(false);
        }
        parExplode.Play();
         Destroy(gameObject, 0.3f);
    }
    public void DestoyRocket()
    {
        parExplode.Play();
        GameObject.Destroy(parTrail.gameObject);//, 0.1f);
        GameObject.Destroy(tfmRocket.gameObject);// 0.1f);
    }
}
