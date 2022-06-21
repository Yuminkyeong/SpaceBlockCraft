using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;
public class BlockInfoDlg : WindowUI<BlockInfoDlg>
{

    public RawImage m_kItem;
    public Text m_kTitle;
    public Text m_kHelp;
    public Text m_kLevel;

    public void Show(int nItem)
    {
        if (nItem == 0) return;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kTitle.text = gData.m_szTitle;
        m_kLevel.text ="Lv."+ gData.level.ToString();
        m_kHelp.text = gData.szInfo;

        Open();
    }
}
