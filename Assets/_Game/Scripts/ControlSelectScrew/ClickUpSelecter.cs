using UnityEngine;

public class ClickUpSelecter : AbsControlSelectScrew
{
    private Screw screw;
    private Vector3 pointDown = new Vector3(0, 0, 0);
    private Vector3 pointUp = new Vector3(0, 0, 0);
    private Collider[] colliders;
    private float minDist;
    private int screwLayer = 1 << LayerMask.NameToLayer(Define.SCREW_LAYER);

    public override void OnClickUp(RaycastHit hit)
    {
        if (BoosterController.Instance.CurrentAnimationBooster != BoosterType.None)
            return;

        Wrench wrench = hit.collider.GetComponent<Wrench>();

        if (wrench != null)
        {
            wrench.OnSelect();
            return;
        }

        Screw selectScrew = hit.collider.GetComponent<Screw>();

        if (selectScrew != null)
        {
            EditorLogger.Log(">>> OnClickUp Screw!!!");
            SelectScrew(hit, selectScrew);
        }
        else if (hit.collider.GetComponent<Shape>() != null)
        {
            colliders = Physics.OverlapSphere(hit.point, 0.5f, screwLayer);
            minDist = float.MaxValue;

            foreach (Collider col in colliders)
            {
                float dist = Vector3.Distance(hit.point, col.transform.position);

                if (dist < minDist)
                {
                    Screw screwTemp = col.GetComponent<Screw>();

                    if (screwTemp != null)
                    {
                        minDist = dist;
                        selectScrew = screwTemp;
                    }
                }
            }

            if (selectScrew != null && !selectScrew.IsDetectShape())    
            {
                EditorLogger.Log(">>> OnClickUp nearestScrew!!!");
                SelectScrew(hit, selectScrew, true);
            }
        }
    }

    private void SelectScrew(RaycastHit hit, Screw screwNeedToProcess, bool isForce = false)
    {
        if (!isForce)
        {
            pointUp = hit.point;
            var distance = Vector3.Distance(pointDown, pointUp);
            EditorLogger.Log($">>>Distance: {distance}");

            if (screw != screwNeedToProcess || distance >= 0.05f)
            {
                return;
            }
        }
        else if (screw != null && screw != screwNeedToProcess)
        {
            return;
        }

        AudioController.Instance.PlaySound(SoundName.ScrewClick, true);
        screwNeedToProcess.OnScrewSelected();
        GameManager.Instance.AwaitInputClick.OnScrewClicked(hit);
        screw = null;
    }

    public override void OnClickDown(RaycastHit hit)
    {
        if (Input.touchCount == 2) return;

        if (hit.collider.GetComponent<Screw>() != null)
        {
            //AudioController.Instance.PlaySound(SoundName.Click);

            screw = hit.collider.GetComponent<Screw>();
            pointDown = hit.point;
        }
    }
}