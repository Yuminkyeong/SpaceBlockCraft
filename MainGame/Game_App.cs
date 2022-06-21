using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class Game_App : MonoBehaviour
{
    public Material[] m_kSkyMat;
    public bool m_bGamePad;// 입력을 게임 패드로 한다

    public Text m_kInfoTest;

    public GameObject m_gHeroDir;
    public GameObject m_gvisible;
    public GameObject m_gUIDir; // UI 호출시 부모 디렉토리 
    public GameObject m_gEUIDir; // UI 호출시 부모 디렉토리 
    public GameObject m_gGameEnvDir;// 게임에서 호출하는 프리펫 부모 디렉토리 
    public GameObject m_gSelect;//선택된 오브젝트

    public GameObject m_gTutoBackgroundUI;

    public GameObject m_gMobDir;

    public Camera m_gEffectCam;

    public Camera m_gCaptureCamera;
    public Canvas Canvas_Window;

    public GameObject m_gSelectBlock;
    public CWCharacter m_gChar_CW;// 해설 캐릭
    public CWCharacter m_gChar_HW;// 해적 캐릭

    public float m_fTuboSpeed=0.5f;

    public float m_fTuboRate = 1.3f;
    public bool m_bTuboFlag = false;

    public float Test1 = 0.01f;
    public float Test2 = 0.1f;
    public float Test3 = 0.1f;

    public float FlagTime = 2f; // 변수 타임

    protected static Game_App instance = null;

    void Awake()
    {
        instance = this;
        
        m_gvisible.SetActive(true);
        if(m_gTutoBackgroundUI)
            m_gTutoBackgroundUI.SetActive(false);

    }
    public static Game_App Instance
    {
        get
        {
            return instance;
        }
    }

    public bool m_bDontFollowCamera;// 카메라 유저 따라가기 금지 

    #region 타이머 함수 시간이 지나면 false로 바뀐다 
    bool _MoveGalaxy;
    public bool g_bDirecting  // 지금은 UI 액션 중이다
    {
        get
        {
            return _MoveGalaxy;
        }
        set
        {
            if(value)
            {
                Debug.Log("");
            }
            _MoveGalaxy = value;
            CWGlobal.g_GameStop = value;
        }
    }
    bool _ADStop;
    public bool g_ADSTOP
    {
        get
        {
            return _ADStop;
        }
        set
        {
            _ADStop = value;
            Invoke("_Function2", 600f);// 7분간 정지  초기 광고 정지
        }
    }
    void _Function2()
    {
        _ADStop = false;
    }

    #endregion

    #region 튜토리얼 

    public Transform m_tTutoDir;
    public TutoHelpDlg m_gTutoHlep;

    CWProductionPage m_kNowPage;
    // 행당 오브젝트를 상위로 올린다
    void ClickEvent(int num)
    {
        ClickHelpDlg.Instance.Close();
        if (m_kNowPage)
            m_kNowPage.CloseEvent();
    }
    public void ShowObject(GameObject gSub,CWProductionPage pg)
    {
        
        m_kNowPage = pg;
        //GameObject gg = Instantiate(gSub);
        GameObject gg = gSub;
        gg.transform.SetParent(m_tTutoDir);
        gg.transform.localPosition = gSub.transform.localPosition;
        //        gg.transform.localScale = Vector3.one;

        CWButton[] bt = gg.GetComponentsInChildren<CWButton>();
        foreach(var v in bt)
        {
            v.CBClickFunction = ClickEvent;
        }

        SlotItemUI[] ss = gg.GetComponentsInChildren<SlotItemUI>();
        foreach (var v in ss)
        {
            v.CBClickFunction = ClickEvent;
        }

        PlanetSlot[] pp = gg.GetComponentsInChildren<PlanetSlot>();
        foreach (var v in pp)
        {
            v.CBClickFunction = ClickEvent;
        }


    }
    public void CloseObject()
    {
        //for(int i=0;i< m_tTutoDir.childCount; i++)
        //{
        //    Transform tt= m_tTutoDir.GetChild(i);
        //    GameObject.Destroy(tt.gameObject, 0.1f);
        //}
    }

    /*
        GameObject m_gPrefab = null;




        public void Show(string szTitle,CallBackFunction cbfuc=null)
        {
            Show(false, szTitle,0, cbfuc);
        }
        public void Show(bool bBkimage, string szTitle, int nSelect, CallBackFunction cbfuc)
        {
            if(m_gPrefab)
                Destroy(m_gPrefab,0.5f);

            TutoHelpDlg gg = Instantiate(m_gTutoHlep);

            m_gPrefab = gg.gameObject;


            gg.transform.SetParent(m_tTutoDir);
            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.Show(bBkimage, szTitle, nSelect, cbfuc);

        }
        public void ShowCursor(bool bBkimage, int ndir, Transform tTarget, bool bClickBtn, Vector3 pos, string szTitle, CallBackFunction cbfuc)
        {
            if (m_gPrefab)
                Destroy(m_gPrefab, 0.5f);

            TutoHelpDlg gg = Instantiate(m_gTutoHlep);
            gg.transform.SetParent(m_tTutoDir);
            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.ShowCursor(bBkimage, ndir, tTarget,bClickBtn,pos,szTitle,cbfuc);

            m_gPrefab = gg.gameObject;

        }
        public void ShowSimpleCursor(bool bRight, Transform tTarget, bool bClickBtn, Vector3 pos, string szTitle, CallBackFunction cbfuc)
        {

            if (m_gPrefab)
                Destroy(m_gPrefab, 0.5f);

            TutoHelpDlg gg = Instantiate(m_gTutoHlep);
            gg.transform.SetParent(m_tTutoDir);
            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.ShowSimpleCursor(bRight, tTarget, bClickBtn, pos, szTitle, cbfuc);

            m_gPrefab = gg.gameObject;

        }
        */
    #endregion


    #region 버튼 조정

    public bool m_bButtonDontClose = false;// 버튼이 클로즈 되지 않았다

    #endregion

    public delegate bool RunFunction(int num);


    public IEnumerator IRun(RunFunction func)
    {
        int number = 0;
        while(!func(number))
        {
            number++;
            yield return null;
        }
    }




}
