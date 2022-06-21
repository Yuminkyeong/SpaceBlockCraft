using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;

public class BasePlanet : MonoBehaviour
{
    public GameObject m_visible;
    public AxisRotate m_kAxisRotate;
    public MeshFilter[] m_kMeshs;
    // 카메라 좌표

    public GameObject m_gMapCameraPos;


    private void Start()
    {
        if (m_gMapCameraPos == null)
        {
            m_gMapCameraPos = CWLib.FindChild(gameObject, "CameraDummy_1");
        }
        if (m_visible == null)
        {
            m_visible = CWLib.FindChild(gameObject, "visible");
        }
    }

    // 현재 맵을 LOD2로 바꾼다
    public virtual void Rotate(Vector3 vRot)
    {
    }
    
    public virtual void ResetRotate()
    {
        m_visible.transform.rotation = new Quaternion();
    }

    public virtual void SetMap()
    {

    }
    void _LookPlanet()
    {

        MoveObject mm = Camera.main.gameObject.AddComponent<MoveObject>();
        mm.m_bHeroStop = true;
        mm.m_gEndObject = m_gMapCameraPos;
        mm.m_fLifetime = 2f;
        mm.m_bLookTarget = false;

        mm.CBCloseFuc = CloseFuc;
        
        TrailRenderer[] tt = gameObject.GetComponentsInChildren<TrailRenderer>();
        foreach (var v in tt)
        {
            v.Clear();
        }
        Game_App.Instance.m_gSelect = gameObject;

    }
    Action m_CloseFuc = null;
    void CloseFuc()
    {
        Game_App.Instance.g_bDirecting = false;
        if (m_CloseFuc!=null)
        {
            m_CloseFuc();
            m_CloseFuc = null;
        }
            
    }
    // 멈추고 카메라를 바라 본다
    public void LookPlanet(Action cbClose)
    {
        m_CloseFuc = cbClose;
        Game_App.Instance.g_bDirecting = true;
        RotateAround rr = GetComponentInChildren<RotateAround>();
        rr.ResetAngle(120, _LookPlanet);

    }
    public virtual void UpdateSelectPlanet()
    {

    }


}
