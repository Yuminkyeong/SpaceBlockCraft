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

public class PlanetCtrl : MonoBehaviour
{

    
    int m_nFace = 0;
    int m_nFaceDown = 0;
    int RaycastPlane()
    {
        RaycastHit hit;
        int layer = LayerMask.NameToLayer("mapcheck");
        int nMask = (1 << layer);//
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
        {
            CWPlanet cs = hit.collider.gameObject.GetComponentInParent<CWPlanet>();
            if(cs)
            {
                if(cs.m_nNumber==Space_Map.Instance.m_nPlanetNumber)
                {

                    int nFace = CWLib.ConvertInt(hit.collider.name);
                    return nFace;

                }
            }
            else
            {
                MultiPlanet ms = hit.collider.gameObject.GetComponentInParent<MultiPlanet>();
                if(ms)
                {
                    int nFace = CWLib.ConvertInt(hit.collider.name);
                    return nFace;
                }

            }
        }
        return -1;
    }
    
    bool m_bDrag=false;
    public void OnMUp()
    {
        m_bDrag = false;
        int nFace = RaycastPlane();
        if (nFace == -1) return;
        if (nFace == m_nFace)
        {
            nFace = m_nFaceDown;
        }
        Space_Map.Instance.SetFace(nFace);
        m_nFace = nFace;



    }
    public void OnMDown()
    {
        m_bDrag = true;
        int nFace = RaycastPlane();
        if (nFace == -1) return;
        m_nFaceDown = nFace;

    }
    public void OnDrag()
    {
        int nFace = RaycastPlane();
        if (nFace == -1) return;
        if (nFace != m_nFace)
        {
            m_nFaceDown = nFace;
        }


    }
    private void Update()
    {
        if (!Space_Map.Instance) return;
        if (Space_Map.Instance.m_nType != 1) return; // 행성 선택에서만 실행

        if (Input.GetMouseButtonDown(0))
        {
            OnMDown();
        }
        if (Input.GetMouseButtonUp(0))
        {
            
            OnMUp();
        }
        if(m_bDrag)
        {
            OnDrag();
        }


    }

}
