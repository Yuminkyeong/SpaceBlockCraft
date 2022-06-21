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

public class CWButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    //
    
    public enum ShowType {NORMAL,NONE };

    [Header("NORMAL : 클릭시 작아짐")]
    public ShowType m_ShowType = ShowType.NORMAL;
    
    // 눌러진 것을 인식
    [Header("눌리면 계속 실행")]
    public bool m_bPressflag=false;
    
    [Header("클릭시 SHOW")]
    public GameObject[] m_gShow;
    [Header("클릭시 HIDE")]
    public GameObject[] m_gHide;

    [Header("클릭시 TOGGLE")]
    public GameObject[] m_gToggle;

    //[Space(10f)]
    [Header("탭 버튼 중에 시작시 선택")]
    public bool m_bFirstSelect;
    [Header("1이상이면 토글 및 탭 기능")]
    public int m_nLayer = 0;

    //[Space(10f)]
    [Header("선택시 반영되는 이미지")]
    public Sprite m_kSelectImage;//선택된 이미지가 있다면
    Sprite m_kImage;
    [Header("선택시 반영되는 텍스트 컬러")]
    public Color m_kTextcolor= Color.clear;
    



    bool m_bDisable = false;
    string m_szDisableMessage = "";

    public UnityEvent ClickFunction;

    [HideInInspector]
    public int m_nNumber = 0;
    public delegate void CBClickEvent(int number);

    public CBClickEvent CBClickFunction;
    [HideInInspector]
    public bool m_bClicked = false;


    bool _bShow = true;
    public bool m_bShow
    {
        get
        {
            return _bShow;
        }
        set
        {
            _bShow = value;
        }
    }

    bool m_bToggle = false;
    public void ResetTogle()
    {
        m_bShow = true;
        UpdateData();
    }

    // 눌렸을 때 이미지 및 이펙트
    void OnPressEffect()
    {
        if (m_ShowType != ShowType.NONE)
        {
            transform.DOScale(m_vOrgScale.x*0.8f, 0.24f).OnComplete(() => {

                transform.DOScale(m_vOrgScale.x, 0.1f);
            });

        }
        if (CWResourceManager.Instance)
           CWResourceManager.Instance.PlaySound("button1");
    }
    // 원래 상태로 
    // 실행 버튼 
    public void OnRun()
    {
        if (m_bDisable) return;
        //Debug.Log(string.Format("btn {0}- {1}",name,transform.parent.name));
        foreach (var v in m_gHide)
        {
            if (v != null)
            {
                BaseUI bs = v.GetComponent<BaseUI>();
                if (bs == null)
                    v.SetActive(false);
                else
                {
                    bs.Close();
                }
            }
        }
        foreach (var v in m_gShow)
        {
            if (v != null)
            {
                BaseUI bs = v.GetComponent<BaseUI>();
                if (bs != null)
                {
                    if (_bShow) bs.Open();
                    else bs.Close();
                }
                else
                {
                    v.SetActive(_bShow);
                }

            }
        }
        ClickFunction.Invoke();
        CBClickFunction?.Invoke(m_nNumber);
        CBClickFunction = null;


        m_bClicked = true;


    }

    public void Clear()
    {
        
        m_bShow = false;
        foreach (var v in m_gHide)
        {
            if (v != null)
            {
                BaseUI bs = v.GetComponent<BaseUI>();
                if (bs == null)
                    v.SetActive(false);
                else
                {
                    bs.Close();
                }
            }
        }



    }
    bool CheckTap()
    {
        bool bTap = false;
        Transform tParent = transform.parent;
        for (int i = 0; i < tParent.childCount; i++)
        {
            CWButton bs = tParent.GetChild(i).GetComponent<CWButton>();
            if (bs != null)
            {
                if (bs == this) continue;
                if (bs.m_nLayer == m_nLayer)
                {
                    bTap = true; // 탭 버튼 
                }
            }
        }
        return bTap;
    }
    void AllClear()
    {
        Transform tParent = transform.parent;
        for (int i = 0; i < tParent.childCount; i++)
        {
            CWButton bs = tParent.GetChild(i).GetComponent<CWButton>();
            if (bs != null)
            {
                if (bs == this) continue;
                if (bs.m_nLayer == m_nLayer)
                {
                    bs.Clear();
                }
            }
        }

    }
    public void OnClickEvent()
    {
        OnPointerDown(null);
        OnPointerUp(null);
    }
    public void OnPointerDown(PointerEventData data)
    {
        if (m_bDisable) return;

        if (StoreDlg.Instance == null)
        {
            Debug.Log(string.Format("{0}_{1}", transform.parent.name, transform.parent.parent.name));
        }

        if (m_nLayer == 0) // 레이어가 없다, 혼자 진행 
        {
            m_bShow = true;
        }
        else
        {
            if(CheckTap())
            {
                SetTapSelect();
            }
            else // 토글 버튼 
            {
                m_bToggle = true;
                m_bShow = !m_bShow;
                foreach (var v in m_gToggle) if (v) v.SetActive(!m_bShow);

            }
        }
        UpdateData();
        OnPressEffect();// 눌러진 효과 표현

        if(m_bPressflag)
        {
           
            StartCoroutine("IPressRun");
            
        }

        if(StoreDlg.Instance==null)
        {
            Debug.Log(string.Format("{0}_{1}", transform.parent.name, transform.parent.parent.name));
        }

    }
    public void OnPointerUp(PointerEventData data)
    {
        if (StoreDlg.Instance == null)
        {
            Debug.Log(string.Format("{0}_{1}", transform.parent.name, transform.parent.parent.name));
        }

        if (m_bDisable)
        {
            if(m_szDisableMessage!=null)
            {
                NoticeMessage.Instance.Show(m_szDisableMessage);
            }
            return;
        }
        
        if(m_bShow|| m_bToggle)
            OnRun();

        StopAllCoroutines();

        if (StoreDlg.Instance == null)
        {
            Debug.Log(string.Format("{0}_{1}", transform.parent.name, transform.parent.parent.name));
        }


    }
    IEnumerator IPressRun()
    {
        yield return new WaitForSeconds(0.5f);
        while (true)
        {
            yield return new WaitForSeconds(0.1f);
            ClickFunction.Invoke();
            m_bClicked = true;


        }
    }

    class ImageData
    {
        public Color m_kColor;
        public Image m_kImage;
    }
    List<ImageData> m_kImageList = new List<ImageData>();

    Color FindColor(Image kImage)
    {
        foreach(var v in m_kImageList)
        {
            if(v.m_kImage==kImage)
            {
                return v.m_kColor;
            }
        }
        return Color.clear;
    }

    Vector3 m_vOrgScale = Vector3.one;
    // 먼저 보여줄 것은 모두 닫는다 
    protected void Awake()
    {

        m_vOrgScale = transform.localScale;
        foreach (var v in m_gToggle) if(v)v.SetActive(!m_bShow);
        if (m_kImage == null)
        {
            Image ii = GetComponent<Image>();
            if (ii != null)
            {
                m_kImage = ii.sprite;
            }

        }

        {
            Image[] ii = gameObject.GetComponentsInChildren<Image>();
            foreach(var v in ii)
            {
                ImageData Idata = new ImageData();
                Idata.m_kColor = v.color;
                Idata.m_kImage = v;
                m_kImageList.Add(Idata);
            }
        }
        




    }

    private void OnEnable()
    {
        if(m_kTextcolor==Color.clear)
        {
            Text tt = GetComponentInChildren<Text>();
            if(tt)
            {
                m_kTextcolor =tt.color;
            }
            
        }
        m_bClicked = false;
        if(m_bToggle)
        {
            m_bShow = false;
        }
        UpdateData();
        if(m_bFirstSelect)
        {
            SetTapSelect();
        }
        if (m_ShowType != ShowType.NONE)
            transform.DOScale(m_vOrgScale.x, 0f);
    }
    // 탭버튼으로 선택 
    public void SetTapSelect()
    {
        Transform tParent = transform.parent;
        for (int i = 0; i < tParent.childCount; i++)
        {
            CWButton bs = tParent.GetChild(i).GetComponent<CWButton>();
            if (bs != null)
            {
                if (bs == this)
                {
                    bs.m_bShow = true;
                }
                else bs.m_bShow = false;
                bs.UpdateData();
            }
        }

    }
    public void UpdateData()
    {

        if (m_ShowType == ShowType.NONE) return;
        if (m_bDisable)
        {
            
            Image[] ii = gameObject.GetComponentsInChildren<Image>();
            foreach (var v in ii)
            {
                if (v.color.a == 0) continue;// 투명 용도의 파일을 통과
                if (v.gameObject.GetComponent<CWButton>()) continue;
                v.color = new Color(0.25f, 0.25f, 0.25f);
            }
            Text tt = gameObject.GetComponentInChildren<Text>();
            if(tt)
            {
                if (m_kTextcolor == Color.clear)
                {
                    m_kTextcolor = tt.color;
                }

                 tt.color = new Color(0.4f, 0.4f, 0.4f);
            }
            if (m_kSelectImage)
            {
                Image kk = GetComponent<Image>();
                if(kk)
                {
                    kk.sprite = m_kImage;
                }
                
            }

        }
        else
        {
            if(m_ShowType==ShowType.NORMAL) ShowType1(m_bShow);
        }


    }
    /// <summary>
    ///  보이는 그림만 디저블 처리
    /// </summary>
    /// <param name="bflag"></param>
    /// <param name="szMessage"></param>
    public void SetDisableView(bool bflag, string szMessage = null)
    {
        m_szDisableMessage = szMessage;
        if (gameObject.activeInHierarchy)
        {
            bool blfag = m_bDisable;
            m_bDisable = bflag;
            UpdateData();
            m_bDisable = false;
        }

    }

    public void SetDisable(bool bflag,string szMessage=null)
    {
        m_szDisableMessage = szMessage;
        m_bDisable = bflag;
        if (gameObject.activeInHierarchy)
        {
            UpdateData();
        }
        
    }

    #region 움직임 효과

    // 밝기 조절
    void ShowType1(bool bfalse)
    {
        if (bfalse)
        {

            Image[] ii = gameObject.GetComponentsInChildren<Image>();
            foreach (var v in ii)
            {
                if (v.color.a == 0) continue;// 투명 용도의 파일을 통과
                if (v.gameObject.GetComponent<CWButton>()) continue;


                v.color = FindColor(v);//Color.white;

            }

            Text tt = gameObject.GetComponentInChildren<Text>();
            if(tt)
            {
                if(m_kTextcolor!=Color.clear)
                    tt.color = m_kTextcolor;
            }
            

            if (m_kSelectImage)
            {
                Image kk = GetComponent<Image>();
                if (kk)
                {
                    kk.sprite = m_kSelectImage;
                }

            }


        }
        else
        {
            Image[] ii = gameObject.GetComponentsInChildren<Image>();
            foreach (var v in ii)
            {
                if (v.color.a == 0) continue;// 투명 용도의 파일을 통과
                if (v.gameObject.GetComponent<CWButton>()) continue;
                v.color = new Color(0.25f, 0.25f, 0.25f);
            }
            Text tt = gameObject.GetComponentInChildren<Text>();
            if (tt)
            {
                if (m_kTextcolor == Color.clear)
                {
                    m_kTextcolor = tt.color;
                }

                tt.color = new Color(0.4f, 0.4f, 0.4f);
            }

            if (m_kSelectImage)
            {
                Image kk = GetComponent<Image>();
                if (kk)
                {
                    kk.sprite = m_kImage;
                }

            }

        }

    }
    // 크기 조절


    #endregion


}
