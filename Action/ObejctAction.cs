using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using CWUnityLib;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;


public class ObejctAction : MonoBehaviour
{
    public bool m_bStop = false;
    

    public virtual void DetectBegin()
    {
        m_bStop = true;
        Debug.Log("detect "+name);
    }
    public virtual void DetectExit()
    {
        m_bStop = false;
    }
    public virtual void DetectStay()
    {

    }
    public virtual float GetSpeed()
    {
        return 0;
    }
    public virtual bool CheckMove(Vector3 vPos)
    {
        //if (vPos.x < 0) return false;
        //if (vPos.x > 256) return false;
        //if (vPos.z < 0) return false;
        //if (vPos.z > 256) return false;
        //if (vPos.y > 255) return false;
        


        return true;
    }
    public virtual Vector3 GetDir()
    {
        Transform m_tCamera = Camera.main.transform;
        return m_tCamera.transform.forward;
    }

    protected virtual void MovePosition(Vector3 vPos)
    {
        transform.position = vPos;
    }
    protected virtual  void Move()
    {
        
        //if (m_bStop)
        //{
        //    Vector3 vPos2 = transform.position;
        //    vPos2.y += 10f * Time.deltaTime;//+= m_tCamera.transform.forward * fspeed * Time.deltaTime;
        //    //transform.position = vPos2;
        //    MovePosition(vPos2);
        //    return;
        //}

        Vector3 vPos = transform.position;
        vPos += GetDir() * GetSpeed() * Time.deltaTime;

        MovePosition(vPos);


    }
    private void LateUpdate()
    {
        if (!GamePlay.Instance) return;
        if (!GamePlay.Instance.IsGamePlay()) return;

        Move();
    }
}
