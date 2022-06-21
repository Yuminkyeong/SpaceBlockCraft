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

public class rolltest : MonoBehaviour
{

    Quaternion m_qDown = new Quaternion();
    Quaternion m_qNow = new Quaternion();

    Vector3 m_vDownPt;           // starting point of rotation arc
    Vector3 m_vCurrentPt;        // current point of rotation arc


    bool m_bDrag;
    float m_fRadius = 1.0f;
    float m_fRadiusTranslation = 1.0f;
    void Start()
    {
     
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void Reset()
    {
        
        //D3DXMatrixIdentity(&m_mRotation);
        //D3DXMatrixIdentity(&m_mTranslation);
        //D3DXMatrixIdentity(&m_mTranslationDelta);
        m_bDrag = false;
       // m_fRadiusTranslation = 1.0f;
        m_fRadius = 1.0f;
    }



}
