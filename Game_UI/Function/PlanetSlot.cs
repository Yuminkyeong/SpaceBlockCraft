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

public class PlanetSlot : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{

    public delegate void CBClickEvent(int number);
    public CBClickEvent CBClickFunction;

    public int m_nSlot;
    public int m_nNumber;
    public Image m_kImage;
    public Text m_kText;

    public CWButton m_gLock;
    public Transform m_gReAction;
    public string m_gLockMessage;
    public Transform m_gMover;
    bool _moveflag=false;
    public bool m_bMoveFlag
    {
        get
        {
            return _moveflag;
        }
        set
        {
            _moveflag = value;
            if(_moveflag)
            {
                if(m_gMover)
                {
                    Game_App.Instance.m_bButtonDontClose = true;// 종료를 기다려야 한다 
                    m_gMover.DOLocalMoveX(0, 1f).OnComplete(() => {
                        Game_App.Instance.m_bButtonDontClose = false;
                    });
                }
            }
            else
            {
                if (m_gMover)
                {
                    Game_App.Instance.m_bButtonDontClose = true;// 종료를 기다려야 한다 
                    m_gMover.DOLocalMoveX(700, 0.5f).OnComplete(()=> {
                        Game_App.Instance.m_bButtonDontClose = false;
                    });
                }

            }
            if (m_gReAction)
            {
                m_gReAction.DORotate(new Vector3(0, 0, 30), 0.3f).OnComplete(() => {
                    m_gReAction.DORotate(Vector3.zero, 0.2f);
                });
            }

        }
    }
    
    //0 스토리 행성, 1 : 멀티 행성 2: 1:1 대전  3 : 내행성
    public void UpdateData()
    {
        if(m_nNumber==0)// 스토리 행성
        {
            m_kImage.sprite = Space_Map.Instance.m_kPlanetsprites[0];// 지정함
            m_kText.text = CWLocalization.Instance.GetLanguage(Space_Map.Instance.m_kPlanettext[0]);



        }
        //2개 행성
        if (m_nNumber == 1)// 멀티행성
        {
            if(CWHeroManager.Instance.m_nStageNumber<=12)
            {
                if(m_gLock)
                    m_gLock.gameObject.SetActive(true);

                m_gLockMessage = "행성 2개이상 정복을 해야 합니다.";
            }
            else
            {
                if (m_gLock)
                    m_gLock.gameObject.SetActive(false);
            }

            m_kImage.sprite = Space_Map.Instance.m_kPlanetsprites[1];// 지정함
            m_kText.text = CWLocalization.Instance.GetLanguage(Space_Map.Instance.m_kPlanettext[1]);
        }
        if (m_nNumber == 2)// 1:1
        {

            m_kImage.sprite = Space_Map.Instance.m_kPlanetsprites[2];// 지정함
            m_kText.text = CWLocalization.Instance.GetLanguage(Space_Map.Instance.m_kPlanettext[2]);
            if (CWHeroManager.Instance.m_nStageNumber <= 6)
            {
                if (m_gLock)
                    m_gLock.gameObject.SetActive(true);

                m_gLockMessage = "행성을 정복 필요합니다.";
            }
            else
            {
                if (m_gLock)
                    m_gLock.gameObject.SetActive(false);
            }


        }
        if (m_nNumber == 3)// 내 행성
        {
            m_kImage.sprite = Space_Map.Instance.m_kPlanetsprites[3];// 지정함
            m_kText.text = CWLocalization.Instance.GetLanguage(Space_Map.Instance.m_kPlanettext[3]);

            if (CWHeroManager.Instance.m_nStageNumber <= 18)
            {
                if(m_gLock)
                    m_gLock.gameObject.SetActive(true);

                m_gLockMessage = "행성 3개이상 정복해야 진입할 수 있습니다";

            }
            else
            {
                if (m_gLock)
                    m_gLock.gameObject.SetActive(false);
            }


        }

        m_bMoveFlag = false;
    }

    public void OnPointerDown(PointerEventData data)
    {

    }
    public void OnPointerUp(PointerEventData data)
    {
        if(m_gLock!=null)
        {
            if (m_gLock.gameObject.activeSelf)
            {

                NoticeMessage.Instance.Show(m_gLockMessage);

                return;
            }

        }
        CBClickFunction?.Invoke(m_nNumber);
        CBClickFunction = null;

        if (m_nSlot == 0)
        {
            m_bMoveFlag = !m_bMoveFlag;
            return;
        }

        Space_Map.Instance.ChangePlanet(m_nSlot,m_nNumber);
    }

}
