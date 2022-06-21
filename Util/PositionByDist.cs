using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
// 해당 타겟을 거리를 두고 위치를 맞춘다
public class PositionByDist : MonoBehaviour
{

    public GameObject m_gTarget;
    public float m_fDist;
    public float m_fYaw;
    public float m_fFitch;
    
    void Start()
    {
    
    }


    
    // 매우 중요
    // 카메라 떨림이 안일어나게 하려면, LateUpdate()에서 구현해야 한다!!!!!!!!!!!!!!!
    private void LateUpdate()
    {
        if (!m_gTarget) return;

    
        Vector3 vdir = CWMath.CalAngle(m_fYaw, m_fFitch, Vector3.forward);

        Vector3 vStart = m_gTarget.transform.position;
        Vector3 vPos = vStart + vdir * m_fDist;
        transform.position = vPos;
        
    }
}
