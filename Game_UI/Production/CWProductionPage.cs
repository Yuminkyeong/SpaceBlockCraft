using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWStruct;
using CWEnum;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using DG.Tweening;
using System.Runtime.InteropServices;
using System.Timers;


public class CWProductionPage : MonoBehaviour {


    [Header("이벤트함수")]
    public UnityEvent EventFunction;
    public UnityEvent ClickEventFunction;



    #region 외부조정변수




    
    public enum TEXTTYPE {NOTICE,TEXTBOX, BOSSTEXTBOX};
    public enum FADETYPE {NODE,FADEOUT,FADEIN }; //Fadeout 점점 어두어짐 , FADEIN 점점 밝아짐


    [Header("페이지 특징")]
    public FADETYPE m_FadeType;
    

    bool _result;
    public bool Result
    {
        get
        {
            return _result;
        }
        set
        {
            _result = value;
        }
    }
    //변수
    public float m_fLifetime; // 시간이 있다면 바로 종료 
    
    float Lifttime
    {
        get
        {
            if (m_kRoot.m_bClosed) return 0;
            return m_fLifetime;
        }
    }

    [Header("오브젝트 움직임")]
    public float m_fMoveSpeed;// 움직이는 속도가 존재한다 

    [Header("타겟오브젝트")]
    public GameObject m_gTarget;
    public string m_szStartObj;// 시작 오브젝트
    public string m_szEndObj;
    public string m_szTargetObject="MainCamera";// 디폴트
    public string m_szLookAtObject;


    
    public bool m_bHeroLookAt;
    public bool m_bRotateFlag=true;// 회전을 한다
    public bool m_bMoveFlag=true; // 움직인다

    // 카메라의 위아래 각도만 움직인다 
    public float m_fCameraPitch = 0;
        

    public GameObject m_visable;


    [Header("Show/Hide")]
    public string[] m_szOpenList;
    public string[] m_szCloseList;

    public GameObject[] m_gPrefab;

    List<GameObject> m_gPrefablist= new List<GameObject>();

    
    CWProductionRoot m_kRoot;

    [Header("메시지")]

    public Texture2D m_kImage;
    public string m_szTalk; // 대사말 
    public TEXTTYPE m_TextType = TEXTTYPE.NOTICE;



    #endregion
    #region 내부변수
    //GameObject m_gStObjPos;  // 시작  좌표
    Vector3 m_vStartPos;
    Quaternion m_qStart;
    GameObject m_gEndObjPos; //끝 좌표 
    GameObject m_gLookAt;

    

    

    bool m_bExit = false;
    bool m_bPlay = false;



    #endregion
    #region 클릭 헬프 

    

    [Header("활성 버튼")]

    public string[] m_szButtonList;

    [Header("블라인드처리")]
    public bool m_bTutoBackGroundUI;
    public float Alpa=188f/255f;

    [Header("게임정지")]
    public bool m_bGameStop;

    [Header("게임무적")]
    public bool m_bMusuk;

    bool _Musuk;

    #endregion
    #region 함수
    private void Awake()
    {
        if(m_visable)
            m_visable.SetActive(false);
        _result = true;
        m_kRoot = GetComponentInParent<CWProductionRoot>();
    }
    public bool IsPlay()
    {
        return m_bPlay;
    }
    class TempUI
    {
        public GameObject gUI;
        public Transform tParent;
        public Vector3 vPos;
    }
    List<TempUI> m_kTempUI = new List<TempUI>();
    void ActiveButtonList()
    {
        bool bflfag = false;
        foreach(var v in m_szButtonList)
        {
            bflfag = true;
            GameObject gg = CWGlobal.FindObject(v);
            if(gg==null)
            {
                Debug.LogError("파일에라! " +v);
            }
            m_gTarget = gg;// 타겟은 1개만 설정
            TempUI kTemp = new TempUI();

            kTemp.gUI = gg;
            kTemp.tParent = gg.transform.parent;
            kTemp.vPos = gg.transform.localPosition;

            m_kTempUI.Add(kTemp);

            Game_App.Instance.ShowObject(gg,this);


        }


        if (m_gTarget&& bflfag)
        {

            ClickHelpDlg.Instance.Show(m_gTarget, 0, OnClick);

        }



    }
    void RestoreButtonList()
    {
        foreach(var v in m_kTempUI)
        {
            //Vector3 vPos = v.gUI.transform.localPosition;
            v.gUI.transform.SetParent(v.tParent);
            v.gUI.transform.localPosition = v.vPos; ;
        }
    }

    public IEnumerator IRun()
    {
        if(EventFunction!=null)
        {
            EventFunction.Invoke();
        }


        if (m_bTutoBackGroundUI)
        {
            Game_App.Instance.m_gTutoBackgroundUI.SetActive(true);
            Image kk=Game_App.Instance.m_gTutoBackgroundUI.GetComponent<Image>();
            kk.color = new Color(0, 0, 0, Alpa);

        }
        else
        {
            Game_App.Instance.m_gTutoBackgroundUI.SetActive(false);
        }

        if(m_kRoot.m_bAIStop)
        {
            CWGlobal.g_GameStop = true;
        }
        else
        {
            CWGlobal.g_GameStop = m_bGameStop;
        }
        


        if (m_bMusuk)
        {
            _Musuk = CWDebugManager.Instance.m_bMusuk;
            CWDebugManager.Instance.m_bMusuk = true;
        }

        // 초기화



        m_gEndObjPos = null; //끝 좌표 
        m_gLookAt = null;
        
        m_bExit = false;

        m_bPlay = true;
        Result = true;// 디폴트가 성공, 성공이 아니면 값을 넣는다
        if (m_visable)
        {
            if (m_visable.activeSelf == true)
            {
                m_visable.SetActive(false);
                yield return null;
            }
            m_visable.SetActive(true);
        }
         m_kRoot.FadeFunc((int)m_FadeType, Lifttime, m_szTalk);

        foreach(var v in m_gPrefab)
        {
            GameObject gg = Instantiate(v);
            gg.transform.parent = Game_App.Instance.m_gGameEnvDir.transform;
            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.transform.rotation = new Quaternion();
            m_gPrefablist.Add(gg);
        }


        ShowHideUI();
        // 오브젝트 검색 
        if(m_gTarget==null)// 타겟이 정해지지 않았다면 
            m_gTarget = CWGlobal.FindObject(m_szTargetObject);

        GameObject gstart = CWGlobal.FindObject(m_szStartObj);
        if(gstart)
        {
            m_vStartPos = gstart.transform.position;
            m_qStart = gstart.transform.rotation;

        }

        m_gEndObjPos = CWGlobal.FindObject(m_szEndObj);
        m_gLookAt = CWGlobal.FindObject(m_szLookAtObject);

        if(gstart == null&& m_gEndObjPos!=null)
        {
            m_vStartPos = m_gTarget.transform.position;
               
        }

        if(m_bHeroLookAt)
        {
            
            m_gLookAt = CWHero.Instance.gameObject;
            if(m_gTarget!=null)
            {
                m_gTarget.transform.LookAt(CWHero.Instance.gameObject.transform);
            }
            else
            {

                Camera.main.transform.LookAt(CWHero.Instance.gameObject.transform);
            }

        }


        if (m_FadeType== FADETYPE.NODE && CWLib.IsString(m_szTalk))
        {
            if(m_TextType==TEXTTYPE.NOTICE)
            {
                NoticeMessage.Instance.Show(m_szTalk, Lifttime);
            }
            if(m_TextType==TEXTTYPE.TEXTBOX)
            {
                bool bclose = true;
                if(m_szButtonList.Length>0)
                {
                    bclose = false;
                }
                TalkBoxDg.Instance.Show(m_kImage,m_szTalk,Lifttime,()=> {

                    m_bExit = true;

                    
                },bclose);
            }
            if (m_TextType == TEXTTYPE.BOSSTEXTBOX)
            {
                bool bclose = true;
                if (m_szButtonList.Length > 0)
                {
                    bclose = false;
                }
                BossTalkBoxDlg.Instance.Show(m_kRoot, m_kImage, m_szTalk, Lifttime, () => {

                    m_bExit = true;


                }, bclose);
            }

            //

        }




        if (m_gTarget != null &&  m_gEndObjPos != null)
        {
            if(Lifttime == 0)
            {
                // 시간이 정해지지 않았다면 그대로 입력
                m_gTarget.transform.position = m_gEndObjPos.transform.position;
                m_gTarget.transform.rotation = m_gEndObjPos.transform.rotation;
            }
        }
        if(m_fCameraPitch!=0)
        {
            if(m_gTarget)
            {
                Vector3 vEuler = m_gTarget.transform.localEulerAngles;
                vEuler.x = m_fCameraPitch;
                m_gTarget.transform.DOLocalRotate(vEuler, Lifttime);
            }
        }

        m_bExit = false;
        float fstartitme = Time.time;

        if (m_kRoot.m_bDebug)
        {
            Debug.Log(string.Format("Start {0} 실행 - {1}", name, m_kRoot.name));
        }
        if(Lifttime == 0)
        {
            m_bExit = true;// 바로 종료 
        }
        float ftime = 0;
       

        if(m_szButtonList.Length>0)
        {
            ActiveButtonList();
        }

        while (!m_bExit)
        {

            if (Lifttime == 0)
            {
                if (!CWLib.IsString(m_szTalk))
                {
                    break;
                }
                    
            }
            if (m_kRoot.m_bPause)
            {
                yield return null;
                continue;
            }
            float ff = Time.time - fstartitme;
            CWGlobal.g_TimeSec = Lifttime - ff;
            if (ff >= Lifttime)
            {
                if(!CWLib.IsString(m_szTalk))
                {
                    m_bExit = true;
                }
                
            }
            if (m_gTarget != null&& gstart!=null &&  m_gEndObjPos != null)
            {
                ftime += Time.deltaTime;

                //0~1사이
                MoveRun( ftime / Lifttime);

            }


            yield return null;

        }
        if(m_gTarget)
        {
            if (m_gEndObjPos)
            {
                if (m_bRotateFlag) m_gTarget.transform.rotation = m_gEndObjPos.transform.rotation;
                m_gTarget.transform.position = m_gEndObjPos.transform.position;
            }

        }


        m_bPlay = false;
        yield return null;
    }
    
    public void _Close()
    {

        RestoreButtonList();
        if (m_visable)
            m_visable.SetActive(false);
        StopAllCoroutines();
        RestoreWindow();

        foreach(var v in m_gPrefablist)
        {
            GameObject.Destroy(v);
        }
        m_gPrefablist.Clear();

        ClickHelpDlg.Instance.Close();
        TalkBoxDg.Instance.Close();
        BossTalkBoxDlg.Instance.Close();


        //  Debug.Log(string.Format(" {0} 종료  -{1}", name, m_kRoot.name));

    }
    // 자식들 까지 모두 닫는다
    public void CloseAll()
    {
        CWProductionPage[] pArray = GetComponentsInChildren<CWProductionPage>();
        foreach(var v in pArray)
        {
            v.OnClose();
        }
    }
    // 종료
    IEnumerator CloseCoroutine()
    {
        RestoreButtonList();
        yield return null;
        while (Game_App.Instance.m_bButtonDontClose)
        {
            // 종료를 기다림
            yield return null;

        }
        
        OnClose();
    }
    public void CloseEvent()
    {
        // 종료를 해도 되는 상황인가?
        if (!gameObject.activeSelf)
        {
            RestoreButtonList();
            OnClose();
            return;
        }
            

        StartCoroutine("CloseCoroutine");
    }


    public void OnClose()
    {
        Result = true;
        m_bExit = true;


        if (m_bMusuk)
        {
            CWDebugManager.Instance.m_bMusuk = _Musuk;
        }


    }
    public void OnFail()
    {
        Result = false;
        m_bExit = true;
    }

    #endregion
    #region 구현함수

    void MoveRun(float ftime)
    {
        if (ftime > 1) return;

        Vector3 vStart = m_vStartPos;
        Quaternion qStart = m_qStart;

        Vector3 vEnd = m_gEndObjPos.transform.position;
        Quaternion qEnd = m_gEndObjPos.transform.rotation;
        if (m_gLookAt)
        {
            Vector3 vdir = m_gLookAt.transform.position - vEnd;
            vdir.Normalize();
            qEnd = Quaternion.LookRotation(vdir);
        }

        // ftime += (Time.deltaTime / Lifttime);
        if (m_bMoveFlag)
            m_gTarget.transform.position = Vector3.Lerp(vStart, vEnd, ftime);

        if (m_gLookAt)
        {
            m_gTarget.transform.LookAt(m_gLookAt.transform);
        }
        else
        {
            if (m_bRotateFlag)
            {

                m_gTarget.transform.rotation = Quaternion.Slerp(qStart, qEnd, ftime);
            }

        }


    }

    
    List<BaseUI> m_kOpenWindow = new List<BaseUI>();
    List<BaseUI> m_kCloseWindow = new List<BaseUI>();
    List<GameObject> m_kOpenObject = new List<GameObject>();
    List<GameObject> m_kCloseObject = new List<GameObject>();

    void RestoreWindow()
    {
        foreach(var v in m_kOpenWindow)
        {
            v.Close();
        }
        foreach(var v in m_kCloseWindow)
        {
            v.Open();
        }
        foreach(var v in m_kOpenObject)
        {
            v.SetActive(false);
        }
        foreach(var v in m_kCloseObject)
        {
            v.SetActive(true);
        }
    }
    void ShowHideUI()
    {
        if (CWMapManager.SelectMap != null)
            m_gEndObjPos = CWLib.FindChild(CWMapManager.SelectMap.gameObject, m_szEndObj);

        foreach(var v in m_szOpenList)
        {
            GameObject gg = CWGlobal.FindObject(v);
            if (gg == null)
            {
                continue;
            }
                

            BaseUI ws =gg.GetComponent<BaseUI>();
            if(ws)
            {
                if(!ws.IsShow())
                {
                    ws.Open();
                    m_kOpenWindow.Add(ws);

                }
            }
            else
            {
                if(!gg.activeSelf)
                {
                    m_kOpenObject.Add(gg);
                }
                gg.SetActive(true);
            }
        }
        foreach(var v in m_szCloseList)
        {
            GameObject gg= CWLib.FindChild(transform.root.gameObject, v);
            if (gg == null) continue;
            BaseUI ws =gg.GetComponent<BaseUI>();
            if(ws)
            {
                if(ws.IsShow())
                {
                    m_kCloseWindow.Add(ws);
                }
                ws.Close();
            }
            else
            {
                if(gg.activeSelf)
                {
                    m_kCloseObject.Add(gg);
                }
                gg.SetActive(false);
            }

        }



    }
    

    public void OnClick()
    {
        if (ClickEventFunction != null)
        {
            ClickEventFunction.Invoke();
        }
        OnClose();

    }

    #endregion

}
