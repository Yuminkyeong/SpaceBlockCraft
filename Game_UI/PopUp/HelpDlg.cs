using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class HelpDlg : WindowUI<HelpDlg>
{
    public GameObject m_kWindow;
    public RawImage m_kBkImage;
    public Text m_kText;

    public override void OnSelect(int num)
    {
        base.OnSelect(num);
        m_kWindow.SetActive(true);
        RawImage rImge = m_gScrollList.m_kSelect.GetComponentInChildren<RawImage>();
        m_kBkImage.texture = rImge.texture;
        m_kText.text = m_gScrollList.GetString(num, "Help");

        
    }
    public void CloseWindow()
    {
        m_kWindow.SetActive(false);
    }

}
