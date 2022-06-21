using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
public class ItemHelpDlg : WindowUI<ItemHelpDlg>
{

    public RawImage m_kImage;
    public Text m_kItemname;
    public Text m_kText1;
    public Text m_kText2;
    public void Show(int nItem,string szMessage,string szMessage2, CBClose cbFuc=null)
    {
        CloseFuction = cbFuc;
        m_kImage.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kText1.text = szMessage;
        m_kText2.text = szMessage2;

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        m_kItemname.text = gData.m_szTitle;

        Open();
    }
}
