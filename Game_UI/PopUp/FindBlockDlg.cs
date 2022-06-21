using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class FindBlockDlg : WindowUI<FindBlockDlg>
{

    public Text m_kBlockName;
    public RawImage m_kBlockImage;

    public void Show(int block)
    {
        CWResourceManager.Instance.PlaySound("legend");
        int Item = CWArrayManager.Instance.GetItemFromBlock(block);
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(Item);
        m_kBlockImage.texture = CWResourceManager.Instance.GetItemIcon(gData.nID);
        m_kBlockName.text = gData.m_szTitle;

        Open();

    }

}
