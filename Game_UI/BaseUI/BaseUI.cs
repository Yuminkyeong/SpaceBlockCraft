using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWEnum;
public class BaseUI : MonoBehaviour
{

    static public List<int> g_kOpenList = new List<int>();

    static public BaseUI m_gWindow;
    static public BaseUI m_gPrevPage=null;

    public delegate void CBClose();
    public CBClose CloseFuction;

    public delegate void CBFunction();
    public CBFunction m_CBFuction;

    protected virtual int GetUINumber()
    {
        return 0;
    }
    bool bShow = false;
    protected bool m_bShow
    {
        get
        {
            return bShow;
        }
        set
        {
            bShow = value;
            //print(string.Format("Show {0}  -{1}",bShow,name));
        }
    }

    public virtual void Open()
    {
       // Debug.Log(string.Format("open {0}- {1}", name, transform.parent.name));

        if (GetUINumber()>0)
            g_kOpenList.Add(GetUINumber());

    }
    public virtual void EmptySlot()
    {

    }
    public virtual void ReceiveDB(JObject jData)
    {

    }
    public bool IsShow()
    {
        return m_bShow;
    }


    public virtual void Close()
    {
        if (CloseFuction != null) CloseFuction();
        CloseFuction = null;

    }
    public virtual void SetShow(bool bflag)
    {

    }
    public virtual void BaseClose()
    {

    }
    public virtual void OnSelect(int num)
    {

    }
    public virtual void OnButtonClick(int num)
    {

    }
    public virtual void UpdateData(bool bselect = true)
    {

    }

    // 재화를 이용해 구입을 했다는 표현은 모두 여기서  
    public virtual void OnBuy(int num)
    {
        print("OnBuy");
    }
    public virtual void OnEscKey()
    {
        MessageBoxDlg.Instance.Show(OnExit, null, "메세지", "종료하시겠습니까");
    }
    public virtual void OnExit()
    {
        Close();
    }

    public Vector3 GetLocalPos(GameObject gChild)
    {
        return gChild.transform.position- transform.position;
    }


    // 오브젝트 타입

    public int m_nGroupType=1;// 1 부터 시작
    // 오브젝트 분류 

}
public class WindowUI<T> : BaseUI
{
    #region 화면전환
    protected UITYPE m_kUIType= UITYPE.NONE;
    public enum SHOWTYPE { NONE,SCALE,BKIMAGE };
    public SHOWTYPE m_ShowType;
    public Texture2D m_kScreenTexture;
    public bool m_bCoinInfo = false;

    public override void Open()
    {
        if (m_bShow) return;
        m_bShow = true;
        if (gameObject.activeSelf == false) return;

        base.Open();

        _Open();
        m_gWindow = this;

        if (m_bCoinInfo)
        {
            CoininfoDlg.Instance.Open();
        }


    }
    // 움직임이 끝난후 호출 
    protected virtual void OnOpen()
    {
    }

    #endregion
    #region 스크롤뷰



    public ScrollListUI m_gScrollList;

    ScrollListUI m_gPrevScrollList=null;
    public override void UpdateData(bool bselect=true)
    {
        if (m_gScrollList)
        {
            m_gScrollList.UpdateData(null, bselect);
            ScrollListUI.g_kSelectScrol = m_gScrollList;
        }

    }

    #endregion
    protected int m_nSelectMode = 0;
    public GameObject[] m_visible;
    public GameObject m_bBkImage;
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
        if(_Instance==null)
        {
            //Debug.Log("");
            Debug.Log(string.Format("!! {0}_{1} _{2}", transform.parent.name, transform.parent.parent.name,gameObject.name));

        }
        for (int i = 0; i < m_visible.Length; i++)
        {
            if (m_visible[i] == null) continue;
            m_visible[i].SetActive(false);
        }
        OnceFunction();
    }
    protected virtual void OnceFunction()
    {

    }

    protected void ShowType()
    {
        if(m_ShowType==SHOWTYPE.SCALE)
        {
            ShowType1();
            return;
        }
        if (m_ShowType == SHOWTYPE.BKIMAGE)
        {
            ShowType2();
            return;
        }
        UpdateData();

    }

    protected virtual void ShowType1()
    {
        transform.DOScaleY(0, 0).OnComplete(() => {
            transform.DOScaleY(1, 0.2f).OnComplete(() => {
                UpdateData();
            });
        });



    }
    protected virtual void ShowType2()
    {
        Transform tBkObject = null;
        if (m_bBkImage!=null)
        {
            tBkObject = m_bBkImage.transform;
        }
        
        for (int i=0;i< m_visible[m_nSelectMode].transform.childCount; i++)
        {
            Transform tChild1 = m_visible[m_nSelectMode].transform.GetChild(i);

            if(tBkObject==null&& tChild1.name== "bkimage")
            {
                tBkObject = tChild1;
                tChild1.gameObject.SetActive(true);
            }
            else
            {
                tChild1.gameObject.SetActive(false);
            }
            
        }
        if(tBkObject==null)
        {
            Debug.LogError("배경그림이 없단다! " + gameObject.name);
            return;
        }
        tBkObject.gameObject.SetActive(true);




        tBkObject.DOScale(0, 0).OnComplete(()=> {
            tBkObject.DOScale(1, 0.3f).OnComplete(() => {

                for (int i = 0; i < m_visible[m_nSelectMode].transform.childCount; i++)
                {
                    Transform tChild1 = m_visible[m_nSelectMode].transform.GetChild(i);
                    tChild1.gameObject.SetActive(true);
                }
                UpdateData();

            });
        });

    }

    public void SetVisible(bool bflag)
    {
        m_visible[m_nSelectMode].SetActive(bflag);
    }
    public override void SetShow(bool bflag)
    {
        if ( bflag)
        {
            for (int i = 0; i < m_visible.Length; i++)
            {
                if (m_visible[i] == null) continue;
                m_visible[i].SetActive(false);
            }
            m_visible[m_nSelectMode].SetActive(true);

            ShowType();
            return;
        }
        for (int i = 0; i < m_visible.Length; i++)
        {
            if (m_visible[i] == null) continue;
            m_visible[i].SetActive(bflag);
        }

    }

    public override void  BaseClose()
    {
        if (!m_bShow) return;
        m_bShow = false;
        _Close();
        StopAllCoroutines();
        SetShow(false);
        m_gWindow = m_gPrevPage;
        base.Close();
    }
    public override void Close()
    {
        if(m_gPrevScrollList!=null)
        {

//            Debug.Log(string.Format("Close prev {0} - {1}", m_gPrevScrollList.gameObject.name, name));
            ScrollListUI.g_kSelectScrol = m_gPrevScrollList;
        }

            
        
        BaseClose();
        
    }
    protected virtual void _Open()
    {
        if(m_gScrollList)
        {
            m_gScrollList.name = name;
            
            m_gPrevScrollList = ScrollListUI.g_kSelectScrol;
            ScrollListUI.g_kSelectScrol = m_gScrollList;

        }
        OnOpen();
        SetShow(true);
        
    }

    protected virtual void _Close()
    {
    }
    public void Refresh()
    {
        Close();
        Open();
    }


    float m_fIIRunTime = 1f;
    IEnumerator IIRun()
    {
        yield return new WaitForSeconds(m_fIIRunTime);
        m_CBFuction();
    }
    protected void InvokeFunc(float fTime, CBFunction cfunc)
    {
        m_fIIRunTime = fTime;
        m_CBFuction = cfunc;
        StartCoroutine("IIRun");
      //  cfunc();
    }

    public virtual void SetSelect(int num)
    {
        m_gScrollList.SetSelect(num);
    }
}
// 개념 : 페이지 원도우인가?
// 화면을 전체 가리면 , PopUPwindow로 사용
// 

public class MessageWindow<T> : WindowUI<T>
{
    
    public override void Open()
    {
        if (m_bShow) return;
        m_bShow = true;
        if (gameObject.activeSelf == false) return;
        _Open();

    }

}
public class TalkWindow<T> : WindowUI<T>
{

    bool m_bExit = false;
    protected bool m_bClosed = false;
    public Text m_kText;
    string m_szSound = "Robots2";
    List<string> m_kList = new List<string>();
    public void CreateTalk(string szText)
    {
        m_kList.Clear();
        string[] aa = szText.Split('&');

        for (int i = 0; i < aa.Length; i++)
        {
            string sz = CWLocalization.Instance.GetLanguage(aa[i]);
            m_kList.Add(sz);

        }

        StartCoroutine(ITalk(0));
    }
    public override void Close()
    {
        if (m_kList.Count > 0)
        {
            string szString = m_kList[0];
            m_kText.text = szString;
        }
        base.Close();
    }
    string getStr(string str,int p)
    {
        return str[p].ToString();

    }
    public virtual void OnClose()
    {
        if (m_bClosed)
        {
            m_bExit = true;
            string szString = m_kList[0];
            if (m_kText.text==szString)
            {
                Close();
            }
        }

    }
    public void FullTalk()
    {
        string szString = m_kList[0];
        m_kText.text = szString;
    }
    IEnumerator ITalk(int num)
    {

        string szString = m_kList[num];

        

        int p = 0;
        m_kText.text = "";

        yield return new WaitForSeconds(0.3f);
        m_bExit = false;
        while (!m_bExit)
        {
            if (p >= szString.Length)
            {
                break;
            }
            if (p % 10 == 0)
            {

                //CWResourceManager.Instance.PlaySound(m_szSound);
            }
            m_kText.text = szString.Substring(0, p);p++;
            yield return new WaitForSeconds(0.06f);
        }
        m_kText.text = szString;

        yield return new WaitForSeconds(1f);
        num++;
        if (num < m_kList.Count)
        {
            yield return StartCoroutine(ITalk(num));
        }


    }


}

// 다른 원도우가 열릴때 같이 생기는 개념
public class SubWindow<T> : WindowUI<T>
{

    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        Open();
    }
    private void OnDisable()
    {
        if (!CWGlobal.G_bGameStart) return;
        Close();
    }

}

