using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class CWProductionRoot : MonoBehaviour {

    #region 전역관리

    // 1,2,3 
    static List<CWProductionRoot> g_kLists = new List<CWProductionRoot>();


    static void Push(CWProductionRoot kRoot)
    {
        // 첫번째거 빼고 모드 pause
        foreach(var v in g_kLists)
        {
            v.m_bPause = true;
        }
        g_kLists.Add(kRoot);

    }
    static void Pop(CWProductionRoot kRoot)
    {
        g_kLists.Remove(kRoot);
        if(g_kLists.Count>0)
        {
            int last = g_kLists.Count - 1;
            g_kLists[last].m_bPause = false;
        }



    }

    #endregion

    
    public bool m_bDebug = false;


    CWProductionPage m_kSelectPage;
    public Action CloseFunction;
    Action StartFunction;




    bool m_bCoin=false;


    public bool m_bPause = false;// 잠시 중지 

    public bool m_bClosed = false;

    public bool m_bCoinView = false;
    public bool m_bDisableUI = false;// UI를 안보여준다
    public bool m_bDirecting = false;// 카메라 따라다니기 금지
    public bool m_bBGMStop = false;// 배경음 끄기
    public bool m_bAIStop = false;

    bool m_bBGM ;

    // 카메라 관련
    // 



    static CWProductionRoot _Instance;
    public static CWProductionRoot Instance
    {
        get
        {
            return _Instance;
        }
    }

    void CreateData()
    {

    }


    private void Awake()
    {
        _Instance = this;

        CWProductionPage kPage = GetComponentInChildren<CWProductionPage>();
        if(kPage.m_visable!=null)
            kPage.m_visable.SetActive(false);


    }

    public void FadeFunc(int nType, float fTime,string szText)
    {

        if(nType==1)
        {
            FadeInOutDlg.Instance.Show(true, fTime, szText);
        }
        if (nType == 2)
        {
            FadeInOutDlg.Instance.Show(false, fTime, szText);
        }


    }


    public void Begin(Action _Closefunction = null, Action _Startfunction = null)
    {

        
        
        CloseFunction = _Closefunction;
        StartFunction = _Startfunction;

        //Push(this);
        StartCoroutine("IStart");

        m_bCoin= CoininfoDlg.Instance.IsShow();
        //CoininfoDlg.Instance.SetShow(m_bCoinView);

        if(m_bDisableUI)
        {
            Game_App.Instance.m_gUIDir.SetActive(false);
            //GamePlay.Instance.SetShow(false);
        }

        Game_App.Instance.m_bDontFollowCamera = m_bDirecting;
        //if (Game_App.Instance.g_bDirecting) return;// 연출중
        m_bBGM = CWGlobal.g_bBgmOn;
        if (m_bBGMStop)
        {
            CWGlobal.g_bBgmOn = false;
        }

    
    }
    IEnumerator IStart()
    {

        if (StartFunction != null)
        {
            StartFunction();
        }
        CWProductionPage kPage = GetComponentInChildren<CWProductionPage>();
        yield return StartCoroutine(IRunPage(kPage));
        Close();

    }

    IEnumerator IRunPage(CWProductionPage kPage)
    {
        //while(Game_App.Instance.g_bDirecting)
        //{
        //    yield return null;
        //}

        {
            yield return null;
            m_kSelectPage = kPage;
            if (kPage)
            {
                yield return StartCoroutine(kPage.IRun());
                if (kPage.Result)
                {
                    if(kPage.m_visable!=null)
                    {
                        Transform tTrans = kPage.m_visable.transform;
                        for (int i = 0; i < tTrans.childCount; i++)
                        {
                            if (!tTrans.GetChild(i).gameObject.activeSelf)
                            {
                                continue;
                            }
                            CWProductionPage cs = tTrans.GetChild(i).GetComponent<CWProductionPage>();
                            if (cs)
                            {
                                yield return StartCoroutine(IRunPage(cs));
                            }
                        }

                    }

                }
                kPage._Close();
            }




            m_kSelectPage = null;
        }
    }


    public void ChildClose()
    {

        CWProductionPage[] pArray = GetComponentsInChildren<CWProductionPage>();
        foreach (var v in pArray)
        {
            v._Close();
        }
    }
    public void OnClick()
    {
        m_kSelectPage.OnClick();

    }

    public void ShowMessageBox() //튜토리얼 정말 스킵하시겠습니까?
    {
        TutoMessageBoxDlg.Instance.Show(SkipTuto, ResumeTuto, "스킵", "튜토리얼을 스킵하시겠습니까?");
    }

    private void SkipTuto() //튜토리얼 스킵
    {
        Close();
        CWLib.FindChild(this.gameObject, "CloseTuto").SetActive(true);

        CWSocketManager.Instance.UpdateDayLog(DAYLOG.TutoSkip);

    }
    private void ResumeTuto() //튜토리얼 계속 진행하기
    {
        TutoMessageBoxDlg.Instance.Close();
    }
    public void Close()
    {

        
        ChildClose();
        //Pop(this);
        
        Destroy(gameObject);
        if (CloseFunction != null)
            CloseFunction();
        // 모든걸 초기화
        Game_App.Instance.g_bDirecting = false;
        CWGlobal.g_bStopAIAttack = false;
        CWGlobal.g_bDontSave = false;// 저장 가능
        CWMapManager.m_bMapDontChange = false;//

        if (m_bDisableUI)
        {
            Game_App.Instance.m_gUIDir.SetActive(true);
            //GamePlay.Instance.SetShow(true);
        }
        Game_App.Instance.m_gTutoBackgroundUI.SetActive(false);


        //CWGlobal.g_bBgmOn = m_bBGM;
        CWBgmManager.Instance.SetBGM(m_bBGM);
        if(m_bAIStop)
        {
            CWGlobal.g_GameStop = false;
        }
        Game_App.Instance.m_bDontFollowCamera = false;

    }

    #region 이벤트 함수 

    #region 게임 전역 변수 조작



    public void OnDontChangeMap()
    {
        CWMapManager.m_bMapDontChange = true;// 회전금지
    }
    public void OnAllowChangeMap() // 회전 허용
    {
        CWMapManager.m_bMapDontChange = false;//
    }
    public void OnDontSave()
    {
        CWGlobal.g_bDontSave = true;// 저장 금지 

    }
    public void OnAllowSave()
    {
        CWGlobal.g_bDontSave = false;// 저장 가능

    }
    public void OnStopAIAttackON()
    {
        CWGlobal.g_bStopAIAttack = true;
    }
    public void OnStopAIAttackOFF()
    {
        CWGlobal.g_bStopAIAttack = false;
    }


    #endregion


    public void OnShowPlanet()
    {
       
    }
    public void OnShowStory()
    {
        
    }

    public void OnMissionSuccess()
    {

       // MissionResultDlg.Instance.Show(true);
    }
    public void OnMissionFail()
    {
     //   MissionResultDlg.Instance.Show(false);
    }

    public void OnHeroShow()
    {
        //CWHero.Instance.Show(true);
        CWHero.Instance.gameObject.SetActive(true);

    }
    public void OnHeroHide()
    {
        //CWHero.Instance.Show(false);
        CWHero.Instance.gameObject.SetActive(false);
    }
  
    public void OnHeroBusterON()
    {
        Buster[] aa = CWHero.Instance.gameObject.GetComponentsInChildren<Buster>();
        foreach (var v in aa)
        {
            v.Begin(1);
        }

    }
    public void OnHeroBusterOFF()
    {
        Buster[] aa = CWHero.Instance.gameObject.GetComponentsInChildren<Buster>();
        foreach (var v in aa)
        {
            v.Stop();
        }

    }
    public void OnAIStart()
    {
      

    }

    //public void OnGamePlay()
    //{
        
    //    CWMapManager.SelectMap.StartPlay();
    //}
    // 게임 플레이 

    // 맵의 마스크제거 : 밝아진다
    public void OnClearMask()
    {
        CWMapManager.Instance.ClearMask();

    }
    // 마스크를 씌운다 :맵이 어두어짐 
    public void OnReStartMask()
    {
        CWMapManager.Instance.ReStartMask();

    }

    public void OnResetHero()
    {
        CWHero.Instance.ResetHP();
    }
    // 정거장연출을 끝낸다 
    public void OnCloseStageProduction()
    {
       

    }
    public void OnRotatePlanet()
    {
        GameObject gg= CWMapManager.Instance.m_gVisable.gameObject;
        AxisRotate rr = gg.GetComponent<AxisRotate>();
        if(rr==null)
        {
            rr = gg.AddComponent<AxisRotate>();
        }
        rr.m_vAxis = new Vector3(1,1,-1);
        rr.m_fSpeed = 4f;





    }
    public void OnRotateStopPlanet()
    {
        GameObject gg = CWMapManager.Instance.m_gVisable.gameObject;
        AxisRotate rr=gg.GetComponent<AxisRotate>();
        if(rr!=null)
            Destroy(rr);

        gg.transform.rotation = new Quaternion();

    }

    // UI를 보이게 한다
    public void OnGamePlayUI_ON()
    {
        
        GamePlay.Instance.SetShow(true);
    }
    // UI를 안보이게 한다
    public void OnGamePlayUI_OFF()
    {
        GamePlay.Instance.SetShow(false);
    }
    public void OnGameCloud_ON()
    {
        CWMapManager.Instance.SetCloud(true);
    }
    public void OnGameCloud_OFF()
    {
        CWMapManager.Instance.SetCloud(false);
    }

    public void OnClose()
    {
        m_bClosed = true;
    }

    #endregion


}
