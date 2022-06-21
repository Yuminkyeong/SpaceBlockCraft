using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;

public class HidenBlockInfoDlg : WindowUI<HidenBlockInfoDlg>
{
    public RawImage m_kItem;
    public Text m_kTitle;
    public Text m_kHelp;
    public Text m_kLevel;

    public void ShowItem1()
    {
        int nItem =  CWArrayManager.Instance.GetBlockLevel(2, Space_Map.Instance.GetStageID()); 
        if (nItem == 0) return;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kTitle.text = gData.m_szTitle;
        m_kLevel.text = "Lv." + gData.level.ToString();
        m_kHelp.text = gData.szInfo;

        Open();
    }
    public void ShowItem2()
    {
        int nItem = CWArrayManager.Instance.GetBlockLevel(3, Space_Map.Instance.GetStageID());
        if (nItem == 0) return;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kTitle.text = gData.m_szTitle;
        m_kLevel.text = "Lv." + gData.level.ToString();
        m_kHelp.text = gData.szInfo;

        Open();
    }

}
