using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWStruct;
using CWUnityLib;
public class ShipInfoDlg : WindowUI<ShipInfoDlg>
{

    public void OnEdit()
    {

        byte[] bBuffer = null;
        int nID = CWHero.Instance.m_nID;
        bBuffer = CWHero.Instance.GetBuffer();
        int maxcount = CWHeroManager.Instance.m_nAirBlockCount; //m_gList.GetSelectValueInt("MaxCount");
        GameEdit.Instance.Show(nID, maxcount, bBuffer);
        GameEdit.Instance.CloseFuction = Space_Map.Instance.Open;
        Close();
        Space_Map.Instance.Close();

    }
    public void OnAirSlot()
    {
        Close();
        Space_Map.Instance.Close();
        ShipSlotDlg.Instance.Open();
    }
}
