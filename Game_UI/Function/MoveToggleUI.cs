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
public class MoveToggleUI : MonoBehaviour
{

    public Vector2 m_vStart;
    public Vector2 m_vEnd;
    public RectTransform m_tTarget;
    public float m_fTime = 0.5f;

    bool m_bFlag = false;

    public void OnToggle()
    {
        if (!m_bFlag)
        {
            m_bFlag = true;
         //   Game_App.Instance.m_bButtonDontClose = true;// 종료를 기다려야 한다 
            m_tTarget.DOAnchorPosX(m_vEnd.x, m_fTime).OnComplete(()=> {

                Game_App.Instance.m_bButtonDontClose = false;
            
            });

        }
        else
        {
            m_bFlag = false;
          //  Game_App.Instance.m_bButtonDontClose = true;// 종료를 기다려야 한다 
            m_tTarget.DOAnchorPosX(m_vStart.x, m_fTime).OnComplete(() => {

                Game_App.Instance.m_bButtonDontClose = false;
            });
        }

    }


}
