using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
 
public class TutoHelpDlg : MonoBehaviour
{

    
    public Text[] m_kText;
    List<string> m_kList = new List<string>();


    public bool m_bClickButton;
    public Transform m_tParentDir;
    public Transform m_tDontClick;// 클릭이 되면 안된다

    public GameObject m_gCursor;

    public GameObject m_gCursor1;
    public GameObject m_gCursor2;
    public GameObject m_gNormalTalkDir;

    public GameObject m_gBkImage;
         
    public GameObject m_gTalkDir;

    public GameObject m_gSimpleTalkDir;
    public GameObject m_gSimpleCursor;
    public GameObject m_gSimpleCursor1;
    public GameObject m_gSimpleCursor2;

    Transform m_tTarget;
    Transform m_tParent;

    bool m_bClick = false;

    CallBackFunction CloseFuction;
    public GameObject[] m_visible;
    protected int m_nSelectMode = 0;
    void Open()
    {

        for (int i = 0; i < m_visible.Length; i++)
        {
            if (m_visible[i] == null) continue;
            m_visible[i].SetActive(false);
        }
        m_visible[m_nSelectMode].SetActive(true);

    }
    // 간단하게 표기만 한다
    public void ShowSimpleCursor( bool bRight, Transform tTarget, bool bClickBtn, Vector3 pos, string szTitle, CallBackFunction cbfuc)
    {
        if (CWLib.IsString(szTitle))
        {
            m_gSimpleTalkDir.SetActive(true);
        }
        else
        {
            m_gSimpleTalkDir.SetActive(false);
        }

        if (!bRight)
        {
            m_gSimpleTalkDir.transform.localPosition = Vector3.zero;
        }
        else
        {
            m_gSimpleTalkDir.transform.localPosition = new Vector3(-1200, 0, 0);
        }
        m_tTarget = tTarget;
        Show(false, szTitle, 2, cbfuc);
        m_tParent = null;
        m_gSimpleCursor.transform.position = pos;
        if (bClickBtn)
        {
            m_gSimpleCursor.SetActive(true);
            m_gSimpleCursor1.SetActive(true);
            m_gSimpleCursor2.SetActive(false);
        }
        else
        {
            m_gSimpleCursor.SetActive(true);
            m_gSimpleCursor1.SetActive(false);
            m_gSimpleCursor2.SetActive(true);
        }
        if(m_tTarget) 
            m_tTarget.localScale = Vector3.one;

        m_bClickButton = bClickBtn;


    }

    public void ShowCursor(bool bBkimage, int ndir, Transform tTarget,bool bClickBtn,Vector3 pos,  string szTitle, CallBackFunction cbfuc)
    {
        if (CWLib.IsString(szTitle))
        {
            m_gTalkDir.SetActive(true);
        }
        else
        {
            m_gTalkDir.SetActive(false);
        }

        if(ndir==0)
        {
            m_gTalkDir.transform.localPosition = Vector3.zero;
        }
        if (ndir == 1)
        {
            m_gTalkDir.transform.localPosition = new Vector3(-1200,0,0);
        }
        if (ndir == 2)//top
        {
            m_gTalkDir.transform.localPosition = new Vector3(0, -204, 0);
        }
        if (ndir == 3)//top
        {
            m_gTalkDir.transform.localPosition = new Vector3(-1200, -204, 0);
        }



        Show(bBkimage,szTitle, 1,cbfuc);

        m_tParent = tTarget.parent;
        m_tTarget = tTarget;
        
        
        m_gCursor.transform.position = pos;
        if(bClickBtn)
        {
            m_tTarget.SetParent(m_tParentDir);
            m_gCursor.SetActive(true);


            m_gCursor1.SetActive(true);
            m_gCursor2.SetActive(false);
        }
        else
        {
            m_tTarget.SetParent(m_tDontClick);
            m_gCursor.SetActive(true);

            m_gCursor1.SetActive(false);
            m_gCursor2.SetActive(true);
            

        }

        m_tTarget.localScale = Vector3.one;
        m_bClickButton = bClickBtn;


    }

    string szTemp;
    
    void Close()
    {
        if (m_tParent)
        {
            m_tTarget.SetParent(m_tParent);
            m_tTarget.localScale = Vector3.one;
        }
        m_gCursor.SetActive(false);
        if(CloseFuction!=null)
        {
            CloseFuction();
            CloseFuction = null;
        }
        Destroy(gameObject);
    }
    public void Show(bool bBkimage, string szTitle, int nSelect, CallBackFunction cbfuc)
    {
        //if (m_bShowFlag == true)
        //{
        //    return;
        //}
        //m_bShowFlag = true;
        m_gBkImage.SetActive(bBkimage);
        szTemp = szTitle;
        if (CWLib.IsString(szTitle))
        {
            m_gNormalTalkDir.SetActive(true);
        }
        else
        {
            m_gNormalTalkDir.SetActive(false);
        }

        m_bClickButton = false;
        m_tParent = null;
        m_tTarget = null;

        CloseFuction = cbfuc;
        m_nSelectMode = nSelect;

        m_kList.Clear();
        string szText = CWLocalization.Instance.GetLanguage(szTitle);
        string[] aa = szText.Split('&');

        for (int i = 0; i < aa.Length; i++)
        {
            string sz = CWLocalization.Instance.GetLanguage(aa[i]);
            m_kList.Add(sz);
        }

        StartCoroutine(ITalk(0));
        Open();
    }
    IEnumerator ITalk(int num)
    {
        yield return null;
        string szString = m_kList[num];
        int p = 0;
        m_kText[m_nSelectMode].text = "";

        int cnt = 0;
        while (true)
        {
            if (m_bClick)
            {
                m_bClick = false;
                break;
            }

            if (p >= szString.Length)
            {
                break;
            }
            if (p % 10 == 0)
            {
                cnt = Random.Range(1, 4);
                CWResourceManager.Instance.PlaySound(string.Format("v{0}",cnt));
            }

            
            m_kText[m_nSelectMode].text=szString.Substring(0,p);p++;
            yield return new WaitForSeconds(0.05f);

        }
        m_kText[m_nSelectMode].text = m_kList[num];
        num++;

        if (m_bClickButton)
        {
            CWButton ct = null;
            //ct가 널이 되면 안됨!!
            if(m_tTarget)
            {
                ct = m_tTarget.GetComponentInChildren<CWButton>();
            }
            if (ct!=null)
            {
            //    ct.m_bClicked = false;
                while (!ct.m_bClicked)
                {
                    yield return null;
                }
            }

        }
        else
        {
            while (true)
            {
                if (m_bClick)
                {
                    m_bClick = false;
                    break;
                }
                yield return null;
            }

            if (num < m_kList.Count)
            {
                yield return StartCoroutine(ITalk(num));
            }

        }
        
        Close();
    }

    public void OnClick()
    {
        m_bClick = true;
    }
}
