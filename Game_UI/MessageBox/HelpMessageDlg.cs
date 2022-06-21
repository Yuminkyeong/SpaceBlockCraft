using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HelpMessageDlg : WindowUI<HelpMessageDlg>
{
    List<string> m_kList = new List<string>();
    public GameObject m_gRenderDir;
    public Text m_Message;
    Action m_OnFinished;
    Action m_OnOK;
    string m_szSound = "Robots2";
    float m_fLifetime = 0.5f;

    public GameObject [] m_gButtons;

    protected override void _Open()
    {
        m_gRenderDir.SetActive(true);
        base._Open();
    }

    public void Show( string szMessage, Action _OnFinished=null,float fTime=5f)
    {
        foreach (var v in m_gButtons) v.SetActive(false);
        m_gButtons[0].SetActive(true);
        m_fLifetime = fTime;
        m_OnFinished = _OnFinished;
        if (CWChattingManager.Instance)
            CWChattingManager.Instance.SystemMessage(szMessage);
        CreateTalk(szMessage);
        base.Open();
        
    }
    public void Show2(string szMessage, Action _OnOK = null, float fTime = 5f)
    {
        foreach (var v in m_gButtons) v.SetActive(false);
        m_gButtons[1].SetActive(true);
        m_fLifetime = fTime;
        m_OnFinished = null;
        m_OnOK = _OnOK;
        if (CWChattingManager.Instance)
            CWChattingManager.Instance.SystemMessage(szMessage);
        CreateTalk(szMessage);
        base.Open();

    }


    void CreateTalk(string szText)
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

    public void OnOK()
    {
        if (m_OnOK != null) m_OnOK();
        Close();
    }

    public override void Close()
    {
        m_gRenderDir.SetActive(false);
        if (m_OnFinished != null)
        {
            m_OnFinished();
            m_OnFinished = null;
        }
        base.Close();
    }


    IEnumerator ITalk(int num)
    {
        
        string szString = m_kList[num];
        int p = 0;
        m_Message.text = "";
        yield return new WaitForSeconds(0.3f);

        while (true)
        {
            if (p >= szString.Length)
            {
                break;
            }
            if (p % 10 == 0)
            {

                CWResourceManager.Instance.PlaySound(m_szSound);
            }
            m_Message.text += szString[p]; p++;
            yield return new WaitForSeconds(0.1f);

        }
        yield return new WaitForSeconds(1f);
        num++;
        if (num < m_kList.Count)
        {
            yield return StartCoroutine(ITalk(num));
        }
        yield return new WaitForSeconds(m_fLifetime);
        Close();

    }
}
