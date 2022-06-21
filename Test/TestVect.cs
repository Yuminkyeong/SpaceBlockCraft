using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TestVect : MonoBehaviour
{
    public Transform m_tTarget;
    public float m_fRaidus;// 반지름 

    public GameObject m_gActor;
    public DOTweenPath m_gPath;


    void Start()
    {
        m_gPath.DOPlay();
    }

    bool IsDetected()
    {

        Vector3 vStart = m_tTarget.position;
        Vector3 vDir = m_tTarget.forward;
        RaycastHit hit;
        Ray ray = new Ray(vStart, vDir);
        //int nMask = (1 << 11);
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            if (hit.collider.gameObject.name == "Me")
            {
                // 충돌했음
                return true;
            }
        }
        return false;

    }
    void StopMove()
    {

    }
    void AvoieMove()
    {

    }
    void SetLinkMove()
    {

        Vector3 vPos = m_gPath.transform.position;
        
        m_gActor.transform.localPosition = vPos;

    }
    // Update is called once per frame
    void Update()
    {
        // 현재 위치에서 타겟하고 충돌하는가?
        if (IsDetected())
        {

        }
        else
        {
            StopMove();
        }
        // 아무곳이나 피한다.
        // 현재 위치에서는 충돌하지 않는다. 움직이지 않는다. 


        SetLinkMove();

    }
}
