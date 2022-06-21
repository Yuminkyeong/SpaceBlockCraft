using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;

public class MultiWinDlg : WindowUI<MultiWinDlg>
{
    public RawImage m_kItem;
    public Text m_kTitle;
    public Text m_kHelp;
    public Text m_kLevel;


    public void Show(CBClose Function = null)
    {
        CloseFuction = Function;
        Open();

        int RR = Random.Range(0, 100);
        int rr = 5;
        if(RR<10)
        {
             rr = Random.Range(4, 12);
        }
        if (RR>= 10 && RR<70)
        {
             rr = Random.Range(4, 7);
        }
        if (RR >= 70 && RR < 95)
        {
             rr = Random.Range(7, 10);
        }
        if (RR >= 95 && RR < 100)
        {
             rr = Random.Range(11, 14);
        }



        int nItem = CWArrayManager.Instance.GetLevelBlock(rr);
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);

        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kTitle.text = gData.m_szTitle;
        m_kLevel.text = "Lv." + gData.level.ToString();
        m_kHelp.text = gData.szInfo;

        CloseFuction = Function;
        //CWInvenManager.Instance.AddItem(nItem,1);
        CollectInvenList.Instance.AddItem(nItem, 1);

        Invoke("Close", 4f);
        Open();

    }

    public override void Close()
    {
       
        base.Close();
    }

}
