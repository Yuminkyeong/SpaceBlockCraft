using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;

public class RewardKillDlg : WindowUI<RewardKillDlg>
{

    public ItemInfoSlot m_kInfoSlot;
    public GameObject m_gTicket;
    
    public Text m_kCount;
    public void Show(int nItem,int nCount=1, CBClose Function = null)
    {
        CloseFuction = Function;
        
        m_kInfoSlot.m_nItem = nItem;
        m_kInfoSlot.m_nCount = nCount;
        m_kInfoSlot.gameObject.SetActive(true);
 

        Open();
        Invoke("Close", 1.5f);

    }

    

}
