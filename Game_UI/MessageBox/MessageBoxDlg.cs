using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

// 취소 버튼이 존재하는 박스 
public class MessageBoxDlg : MessageWindow<MessageBoxDlg>
{
    public Text m_Title;
    public Text m_Message;
    Action m_OnOk;
    Action m_OnCancel;
    bool m_bOk = false;
    public  void Show(Action _OnOk, Action _OnCancel, string szTitle,string szMessage)
    {
        CWGlobal.g_GameStop = true;
        m_bOk = false;
        if(m_Title!=null)
            m_Title.text = szTitle;

        m_Message.text = szMessage;
        m_OnOk = _OnOk;
        m_OnCancel = _OnCancel;
        base.Open();
    }
    public  void Click_OK()
    {
        if (m_OnOk != null)
        {
            m_OnOk();
        }
        Close();
    }
    public  void Click_Cancel()
    {
        if (m_OnCancel != null)
        {
            m_OnCancel();
        }
        Close();
    }
    public override void Close()
    {
        CWGlobal.g_GameStop = false;
        base.Close();
    }

}
