using UnityEngine;
using System.Collections;

public class ScrollPopup<T> : NormalPopup<T>
{
    public CWBridgeList[] m_DynamicScroll;
    int m_nSelectList = 0;
    public void SelectList(int num)
    {
        m_nSelectList = num;
    }
    public  void UpdateData()
    {
        for (int i = 0; i < m_DynamicScroll.Length; i++)
        {
            m_DynamicScroll[i].UpdateData();
        }

    }
    public override CWBridgeList GetSelectList()
    {
        return m_DynamicScroll[m_nSelectList];
    }
    public override void Open(Returnfuction fuc=null)
    {

        base.Open(fuc);
        for (int i = 0; i < m_DynamicScroll.Length; i++)
        {
            m_DynamicScroll[i].gameObject.SetActive(true);
            m_DynamicScroll[i].Create(this, fuc);
        }
        
    }
    public override void SetShow(bool bflag)
    {
        base.SetShow(bflag);

        for (int i = 0; i < m_DynamicScroll.Length; i++)
        {
            m_DynamicScroll[i].gameObject.SetActive(bflag);
        }

    }
    public override void Close()
    {
        for (int i = 0; i < m_DynamicScroll.Length; i++)
        {
            m_DynamicScroll[i].Close();
        }

        base.Close();
    }
    public void SetCursor(int num)
    {
        m_DynamicScroll[0].SetCursor(num);
    }
    public void PositionReset()
    {
        m_DynamicScroll[0].PositionReset();

    }
    public void SetCursorCapture()
    {
        m_DynamicScroll[0].SetCursorCapture();
    }
    public void BackupCursorCapture()
    {
        m_DynamicScroll[0].BackupCursorCapture();
    }
}
