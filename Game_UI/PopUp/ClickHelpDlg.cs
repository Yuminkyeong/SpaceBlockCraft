using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ClickHelpDlg : MessageWindow<ClickHelpDlg>
{
    // 0 : 탭하는 손 
    // 1 : 조이스틱 손
    // 2 : 슈팅하는 손 


    public GameObject m_gTarget;
    Action m_kClickFunc=null;

    public void Show3D(GameObject gTarget, Action kClickFunc)
    {
        m_nSelectMode = 0;
        m_gTarget = gTarget;
        m_kClickFunc = kClickFunc;
        UIOverlayPos pp = GetComponentInChildren<UIOverlayPos>(true);
        if(pp)
        {
            pp.m_gTarget = m_gTarget;
        }
        Open();

    }
    public void Show(GameObject gTarget, int nSelect=0, Action kClickFunc=null)
    {
        m_nSelectMode = nSelect;
        m_gTarget = gTarget;
        m_kClickFunc = kClickFunc;
        Open();
    }
    public void OnClickBtn()
    {
        if(m_kClickFunc!=null)
        {
            m_kClickFunc();
        }
        Close();
    }
    
    public override void Close()
    {
        m_gTarget=null;
        m_visible[m_nSelectMode].transform.position = new Vector3(10000, 10000);
        base.Close();
    }
    private void Update()
    {
        if (!m_bShow) return;
        
        if (m_gTarget == null) return;
        m_visible[m_nSelectMode].transform.position = m_gTarget.transform.position;
    }
}
