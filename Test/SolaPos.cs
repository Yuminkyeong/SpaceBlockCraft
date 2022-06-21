using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolaPos : MonoBehaviour
{

    public Vector3 m_vStart;
    public float m_fRadius;
    public GameObject m_gTarget;
    
    public void Create()
    {

        
        for(int i=0;i<transform.childCount;i++)
        {

            Vector3 rr = new Vector3(0,0,1);// Random.onUnitSphere;
            rr.y = 0;
            Transform tChild = transform.GetChild(i);
            
            tChild.localPosition = m_vStart+ rr*(i*m_fRadius) ;

        }

    }
    // 현재 카메라 적용하기
    public void ApplayCam()
    {

        BasePlanet[] pp = gameObject.GetComponentsInChildren<BasePlanet>();

        for (int i=0;i<pp.Length;i++)
        {
            pp[i].m_gMapCameraPos.transform.localPosition = m_gTarget.transform.localPosition;
            pp[i].m_gMapCameraPos.transform.localRotation = m_gTarget.transform.localRotation;
        }

    }

}
