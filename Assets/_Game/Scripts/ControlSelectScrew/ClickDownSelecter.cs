using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDownSelecter : AbsControlSelectScrew
{
    public override void OnClickUp(RaycastHit hit)
    {
        if (hit.collider.GetComponent<Screw>() != null)
        {
            AudioController.Instance.PlaySound(SoundName.ScrewClick,true);

            var screw = hit.collider.GetComponent<Screw>();
            screw.OnScrewSelected();

        }


    }
}
