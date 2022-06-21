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

public class ShipSlotDlg : WindowUI<ShipSlotDlg>
{


    ////public override void OnSelect(int num)
    ////{
    ////    base.OnSelect(num);
    ////    int nID = m_gScrollList.GetSelectValueInt("_id");
    ////    if(nID != CWHeroManager.Instance.m_nAirSlotID)
    ////    {
    ////        CWHeroManager.Instance.SelectAirSlot(nID);
    ////        UpdateData(false);
    ////    }
       
    ////}
    
    //public void Show(CBClose _close)
    //{
    //    CloseFuction = _close;
    //    Open();
    //}

    protected override void _Open()
    {
        m_nGroupType = 1;
        base._Open();
    }

    public override void Close()
    {
        base.Close();
    }
    public void OnExitBtn()
    {
        Space_Map.Instance.Open();
        Close();
    }

}
