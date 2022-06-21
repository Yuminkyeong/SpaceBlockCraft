using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
public class NoticeBox : MessageWindow<NoticeBox>
{
    public Text m_kTitle;
    public Text m_kTitle2;
    public Text m_kText;
    public Text m_kText2;
    public Text m_kDate;
    public Text m_kDate2;
    public Text m_kGem;
    public GameObject[] m_View;
    public void Show(int nType,string szTitle,  string szText,string szDate,int Reward, CBClose cbfun)
    {
        int nSelect = CWPrefsManager.Instance.GetLanguage();
        if (nSelect > 0)
        {
            cbfun();
            return;
        }
            
        foreach (var v in m_View) v.SetActive(false);
        m_View[nType].SetActive(true);
        CloseFuction = cbfun;
        m_kTitle.text = szTitle;
        m_kTitle2.text = szTitle;
        m_kText.text = szText;
        m_kText2.text = szText;
        m_kDate.text = szDate;
        m_kDate2.text = szDate;
        m_kGem.text = Reward.ToString();
        Open();
    }
}
