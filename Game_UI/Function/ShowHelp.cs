using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 튜토리얼은 한번만 한다
public class ShowHelp : MonoBehaviour
{

    // 순서대로 가는가 

    public static bool g_bActive = false;

    public bool m_bCursorRight = false;
    public bool m_bTop = false;
    public bool m_bGameStop = false;

    public bool m_bBkimage=true;
    public enum STYPE {NORMAL,CURSOR,SIMPLECURSOR };//curs
    public STYPE m_kType;
    public bool m_bClickbtn;// 버튼을 클릭한다
    public GameObject m_gForcus;// 
    public GameObject m_gPos;// 

    public int m_nNumber = 0;
    public string m_szTalk;
    public GameObject m_gVisible;
    public ShowHelp m_gChild;
    

    public    bool m_bShowed = false;

    public bool m_bClosed = false;

    public void AllFalse()
    {
        if (m_gVisible)
            m_gVisible.SetActive(false);
        if (m_gChild)
            m_gChild.gameObject.SetActive(false);

    }
    private void Awake()
    {
        if(m_gVisible)
            m_gVisible.SetActive(false);
        if (m_gChild)
            m_gChild.gameObject.SetActive(false);

        if(transform.GetChild(0)!=null)
        {
            
        }
        

    }
    void Close()
    {
        m_bClosed = true;
        g_bActive = false;
        if (m_gChild)
        {
            m_gChild.gameObject.SetActive(true);
            if (m_gChild.Show())
            {
                m_gChild = null;
            }
        }
        else
        {
        }


    }
    public bool Show()
    {
        if (gameObject.activeSelf==false)
        {
            return false;
        }
        if(gameObject.activeInHierarchy==false)
        {
            return false;
        }
        g_bActive = true;
        if (m_gVisible)
            m_gVisible.SetActive(true);

        if (m_bShowed)
        {
            if(m_bClosed)
            {
                //종료를 눌렀다
                if(m_gChild)
                {
                    // 이런 상황이 왔다 
                    m_gChild.gameObject.SetActive(true);
                    if (m_gChild.Show())
                    {
                        m_gChild = null;
                    }

                }
                else
                {

                }
            }

            return false;
        }
        m_bShowed = true;


        //if (m_kType == STYPE.NORMAL)
        //{
        //    Game_App.Instance.Show(m_bBkimage,m_szTalk, (int)m_kType, Close);
        //}
        //else if (m_kType == STYPE.CURSOR)
        //{
        //    int ndir = 0;
        //    if (m_bCursorRight) ndir = 1;
        //    if (m_bTop) ndir = 2;//
        //    if(m_bCursorRight&&m_bTop) ndir = 3;//

        //    Game_App.Instance.ShowCursor(m_bBkimage, ndir, m_gForcus.transform, m_bClickbtn, m_gPos.transform.position, m_szTalk, Close);
        //}
        //else if (m_kType == STYPE.SIMPLECURSOR)
        //{
        //    Game_App.Instance.ShowSimpleCursor(m_bCursorRight, m_gForcus.transform, m_bClickbtn, m_gPos.transform.position, m_szTalk, Close);
        //}
        //CWGlobal.g_GameStop = m_bGameStop;


        

        return true;
    }





}
