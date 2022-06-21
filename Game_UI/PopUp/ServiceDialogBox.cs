using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
using CWStruct;
using CWEnum;
using DG.Tweening;

public class ServiceDialogBox : WindowUI<ServiceDialogBox>
{
    public GameObject m_gCheck1;
    public GameObject m_gCheck2;

    bool _bflag1;
    bool m_bFlag1
    {
        get
        {
            return _bflag1;
        }
        set
        {
            _bflag1 = value;
            m_gCheck1.SetActive(value);
        }
    }
    bool _bflag2;
    bool m_bFlag2
    {
        get
        {
            return _bflag2;
        }
        set
        {
            _bflag2 = value;
            m_gCheck2.SetActive(value);
        }
    }

    public void OnClick1()
    {
        m_bFlag1 = !m_bFlag1;
        if(m_bFlag1&& m_bFlag2)
        {
            Close();
        }
    }
    public void OnClick2()
    {
        m_bFlag2 = !m_bFlag2;
        if (m_bFlag1 && m_bFlag2)
        {
            Close();
        }

    }
    public override void Close()
    {
        base.Close();
        CWMainGame.Instance.AutoLogin();

    }

}
