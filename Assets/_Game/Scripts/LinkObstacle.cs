using Cysharp.Threading.Tasks;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LinkObstacle : Shape
{

    [ContextMenu("SetUpScrew")]

    protected override void SetUpScrew()
    {
        gameObject.layer = 0;

        Transform[] allChildren = GetComponentsInChildren<Transform>();

        foreach (Transform child in allChildren)
        {
            if (child.name.ToLower().Contains("screw"))
            {
                child.gameObject.layer = 6;
                if (child.GetComponent<Screw>() == null)
                {
                    child.gameObject.AddComponent<Screw>();
                }
                rb = GetComponent<Rigidbody>();
                rb.isKinematic = true;

                joint = GetComponent<HingeJoint>();
                var screw = child.GetComponent<Screw>();
                Debug.Log(screw);
                Debug.Log(this);
                screw.LstLinkObstacle.Add(this);
                lstScrew.Add(screw);

                Debug.Log(lstScrew.Count);
            }
        }
        for (int i = 0; i < lstScrew.Count; i++)
        {
            var gobjHold = Instantiate(hold);
            gobjHold.transform.SetParent(transform);
            gobjHold.transform.position = lstScrew[i].transform.position;
            gobjHold.transform.rotation = lstScrew[i].transform.rotation;
            var rb = gobjHold.GetComponent<Rigidbody>();
            lstHoldRigi.Add(rb);
        }

    }

    public void ScrewObstacleToShape()
    {
        for (int i = 0; i < lstScrew.Count; i++)
        {
            lstScrew[i].ScrewObstacleToShape();

        }
    }

    [ContextMenu("CheckRemoveScrew")]
    public override void CheckRemoveScrew()
    {
        for (int i = lstScrew.Count - 1; i >= 0; i--)
        {
            if (lstScrew[i] == null)
                lstScrew.RemoveAt(i);
        }
        if (lstScrew.Count == 1)
        {
         ////   joint.connectedBody = lstHoldRigi[0];
           // joint.anchor = lstHoldRigi[0].transform.localPosition;
            Vector3 screwLocalUp = lstHoldRigi[0].transform.localRotation * Vector3.up;
          //  joint.axis = screwLocalUp;
            lstHoldRigi[0].transform.SetParent(levelMap.transform);
            //EnableKinematic(false);

        }

        if (lstScrew.Count == 0)
        {
            OnThisShapeRemove();
            //col.isTrigger = false;
            //lstHoldRigi[0].isKinematic = false;
            levelMap.LstShape.Remove(this);
            /// 
            DestroyShape();

            var screwPos = lastScrew.transform.position + lastScrew.transform.up * 5;
            //col.isTrigger = true;
            ApplyReleaseForce(0.2f, 20, screwPos);

        }

    }

}
