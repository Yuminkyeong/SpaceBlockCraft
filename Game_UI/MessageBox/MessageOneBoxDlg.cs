using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class MessageOneBoxDlg : MessageWindow<MessageOneBoxDlg>
{

    public Text m_Title;
    public Text m_Message;
    Action m_OnFinished;
    public virtual void Show(string szTitle, string szMessage)
    {
        Close();
        m_OnFinished = null;
        m_Title.text = szTitle;
        m_Message.text = szMessage;
        base.Open();
    }
    public virtual void Show(Action _OnFinished, string szTitle, string szMessage)
    {
        Close();
        m_OnFinished = _OnFinished;
        m_Title.text = szTitle;
        m_Message.text = szMessage;
        base.Open();
    }

    public virtual void Click_Close()
    {
        if (m_OnFinished != null)
        {
            m_OnFinished();
        }
        base.Close();
    }


}
