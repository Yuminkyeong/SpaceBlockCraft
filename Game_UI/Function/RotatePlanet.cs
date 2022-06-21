using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class RotatePlanet : MonoBehaviour
{
    Vector3[] VRotate =
   {
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 90),
            new Vector3(90, 0, 0),
            new Vector3(0, 0, 270),
            new Vector3(270, 0, 0),
            new Vector3(180, 0, 0),
    };


    public void Rotate(int nFace)
    {

        if (nFace < 0)
        {
            Debug.Log("");
            return;
        }
        if (nFace >= 6)
        {
            Debug.Log("");
            return;
        }
        Quaternion qq = Quaternion.Euler(VRotate[nFace]);
        transform.DOLocalRotateQuaternion(qq, 1);
    }

    private void OnEnable()
    {
        int nStage = (Space_Map.Instance.GetStageID() - 1) % 6;

        Rotate(nStage);

    }
}
