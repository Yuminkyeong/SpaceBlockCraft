using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWSpinWithMouse : MonoBehaviour {

    
    Vector3 m_vPivot;
    public float m_fSpeed = 10f;
    public Vector3 m_vRotate =Vector3.up;
    public GameObject m_gCube;
    Transform mTrans;
    void Start()
    {
        mTrans = CWHero.Instance.transform;
        //m_vPivot =  //CWHero.Instance.GetPosition() - CWHero.Instance.GetCenter();
        m_gCube.transform.parent = mTrans;
        m_gCube.transform.localPosition= CWHero.Instance.GetCenter();
        //m_vPivot = m_gCube.transform.position;

        m_vPivot = CWHero.Instance.m_gCenterObject.transform.position;
    }
    void OnDrag(Vector2 delta)
    {


        
        //UICamera.currentTouch.clickNotification = UICamera.ClickNotification.None;

        
        m_vRotate.y = -delta.x;
        m_vRotate.z = -delta.y;

        m_vRotate.Normalize();

        mTrans.RotateAround(m_vPivot, m_vRotate, m_fSpeed*Time.deltaTime);
    }
}
