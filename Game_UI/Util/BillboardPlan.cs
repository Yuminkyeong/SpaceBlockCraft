using UnityEngine;
using System.Collections;
using CWUnityLib;
public class BillboardPlan : MonoBehaviour {


    public bool bFixUpDown;// 위아래 고정
    void LateUpdate()
    {
        if (bFixUpDown)
        {
            FixUpdate();
            return;
        }
        transform.LookAt(Camera.main.transform.position);
    }
    void FixUpdate()
    {
        if (Camera.main == null) return;
        Vector3 v1 = transform.position;
        Vector3 v2 = Camera.main.transform.position;
        float fYaw = CWMath.GetLookYaw(v2, v1);
        Vector3 vPos = transform.eulerAngles;
        vPos.y = fYaw;
        transform.eulerAngles = vPos;


    }
}
