using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class CWWindow : MonoBehaviour
{

    static public CWWindow g_kPageWindow;
    public CWJSon JSonData;

    public enum WINDOWTYPE {POPUP,WINDOW };

    public WINDOWTYPE NWinType=WINDOWTYPE.WINDOW;
    public delegate void CBClose();
    public delegate void Returnfuction(int Num);
    
    public CBClose CloseFuction;
    public int m_nSelect;

    protected bool m_bShow;
    public bool IsShow()
    {
        return m_bShow;
    }

    public virtual void Open(Returnfuction fuc=null)
    {
        
        NWinType = WINDOWTYPE.WINDOW;
    }
    public virtual void Close()
    {
        if (CloseFuction!=null) CloseFuction();
        CloseFuction = null;
    }
    public virtual void SelectItem(int num,GameObject gSelectObject)
    {
        m_nSelect = num;
    }
    public virtual void SetShow(bool bflag)
    {
        
    }

    public virtual void Exit()
    {
        Close();
    }
    public virtual CWBridgeList GetSelectList()
    {
        return null;
    }

}

//CWSingleton
public class WindowSingleton<T> : CWWindow
{
    
    

    #region 효과

    public float m_fEffectTime = 1f;
    public bool m_bStartEffect; // 스므스하게 나타난다
    public Vector3 m_vEffectStart;// 시작 좌표
    public Vector3 m_vEffectEnd; // 마지막 좌표 
    // 시작 이펙트
    IEnumerator EffectRun(bool bStartFlag)
    {


        UISprite bkImage = _Visible[0].GetComponent<UISprite>();

        Vector3 vStart;
        Vector3 vEnd;

        // 이펙트 중에는 버튼 클릭 금지!!
        // 버튼으로 enable 사용 금지
        if (bkImage)
            bkImage.alpha = 1f;
        if (bStartFlag)
        {
            SetShow(bStartFlag);
            vStart = m_vEffectStart;
            vEnd = m_vEffectEnd;

        }
        else
        {
            vStart = m_vEffectEnd;
            vEnd = m_vEffectStart;

            // 모든 버튼 enable
        }

        UIButton[] array = gameObject.GetComponentsInChildren<UIButton>(true);
        for (int i = 0; i < array.Length; i++)
        {
            array[i].enabled = bStartFlag;
        }

        if (vStart != Vector3.zero)
            transform.localPosition = vStart;

        Vector3 vDir = vEnd - vStart;
        vDir.Normalize();

        float fDist = Vector3.Distance(vEnd, vStart);

        float fvalue = 0f;
        float fStart = Time.time;
        while (true)
        {
            float fRest = Time.time - fStart;


            if (fRest > m_fEffectTime)
            {

                if (bStartFlag)
                {
                    if (bkImage)
                        bkImage.alpha = 1f;
                    if (m_vEffectEnd != Vector3.zero)
                        transform.localPosition = m_vEffectEnd;
                }
                else
                {
                    if (bkImage)
                        bkImage.alpha = 0;
                    if (m_vEffectStart != Vector3.zero)
                        transform.localPosition = m_vEffectStart;

                }

                break;
            }
            if (m_vEffectStart != Vector3.zero)
            {
                Vector3 vPos;
                fvalue = fRest / m_fEffectTime;
                vPos = vStart + fDist * fvalue * vDir;
                transform.localPosition = vPos;
            }


            if (bkImage)
            {
                if (bStartFlag)
                    bkImage.alpha = fvalue;
                else bkImage.alpha = 1f - fvalue;

            }
            yield return null;
        }
        if (bStartFlag == false)
        {
            SetShow(bStartFlag);
            StopAllCoroutines();
        }


    }


    #endregion
    static T _Instance;
    public static T Instance
    {
        get
        {
            return _Instance;
        }
    }
    protected void Awake()
    {
        _Instance = this.GetComponent<T>();
        for (int i = 0; i < _Visible.Length; i++)
        {
            if (_Visible[i] == null) continue;
            _Visible[i].SetActive(false);
        }

    }

    int m_nSelectMode=0;
    bool m_bSelectFlag = false;
    

    public GameObject[] _Visible;
    public override void SetShow(bool bflag)
    {
        if(m_bSelectFlag && bflag)
        {

            for (int i = 0; i < _Visible.Length; i++)
            {
                if (_Visible[i] == null) continue;
                _Visible[i].SetActive(false);
            }
            _Visible[m_nSelectMode].SetActive(true);

            return;
        }
        for (int i = 0; i < _Visible.Length; i++)
        {
            if (_Visible[i] == null) continue;
            _Visible[i].SetActive(bflag);
        }

    }
    protected virtual void _Open()
    {
    }
    protected virtual void _Close()
    {
    }
    public void SelectOpen(int nSelect)
    {
        m_nSelectMode = nSelect;
        m_bSelectFlag = true;
        m_bShow = true;
        SetShow(true);
        base.Open(null);
        _Open();

    }
    public override void Open(Returnfuction fuc = null)
    {
        if (m_bShow) return;
        m_bShow = true;
        if (gameObject.activeSelf == false) return;

        if (m_bStartEffect)
        {
            StopAllCoroutines();
            StartCoroutine(EffectRun(true));
        }
        else
        {
            SetShow(true);

        }


        base.Open(fuc);
        _Open();
    }
    public override void Close()
    {
        if (!m_bShow) return;
        m_bShow = false;
        _Close();

        StopAllCoroutines();
        if (m_bStartEffect)
        {

            StartCoroutine(EffectRun(false));
        }
        else
        {
            SetShow(false);
        }
        base.Close();

    }

}
public class BaseWindow<T> : WindowSingleton<T>
{
    public bool bGameStop = false;

    public bool m_bPopUp=false; // 다른 윈도우를 모두 닫고,끝난후 원상 복귀 시킨다

    protected override void _Open()
    {
        if (bGameStop == true)
        {
            DebugX.Log("게임 스톱");
            CWGlobal.g_GameStop = true;
            CWMobManager.Instance.Close();

        

        }
            
    }
    protected override void _Close()
    {
        if(bGameStop==true)
            CWGlobal.g_GameStop = false;


    }
    public override void Open(Returnfuction fuc = null)
    {
        base.Open(fuc);
        _Open();

    }
    public override void Close()
    {
        base.Close();
        
    }
}

/// <summary>
/// 항상 활동하는 윈도우를 중심으로 만든다.
/// Run이 항상 돌아가는 윈도우
/// Run을 사용하지 않을 경우는 굳이 붙일 필요는 없다
/// </summary>
/// <typeparam name="T"></typeparam>
public class CWWindowRun<T> : BaseWindow<T>
{


    protected bool g_bOnce = false;

    public virtual bool OnceRun()
    {
        return true;
    }
    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;

        g_bOnce = false;
    }
    private void OnDisable()
    {
        g_bOnce = false;

    }
    protected override void _Open()
    {
        g_bOnce = false;
        base._Open();
    }

    public virtual void Run() { }
    void Update()
    {
        if (!m_bShow) return;
        if (!CWGlobal.G_bGameStart) return;
        if (!g_bOnce)
        {
            g_bOnce = OnceRun();
        }
        Run();
    }
    public override void Open(Returnfuction fuc = null)
    {
        if (m_bShow) return;
        m_bShow = true;

        SetShow(true);
        _Open();
    }
    public override void Close()
    {
        if (!m_bShow) return;
        m_bShow = false;
        g_bOnce = false;
        _Close();
        SetShow(false);
        StopAllCoroutines();
    }
    
    public override void Exit()
    {
        MessageOneBoxDlg.Instance.Show(CWMainGame.Instance.Quit, "종료", "종료하시겠습니까?");
    }


}
