using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class ShapeShopDlg : WindowUI<ShapeShopDlg>
{

    protected override int GetUINumber()
    {
        return 15;
    }

    // 1 골드 판매
    //2 보석  판매 
    bool bFlag = false;
    public void Show(int nType)
    {
        m_nGroupType = nType;
        Open();
    }
    public override void Open()
    {
        bFlag = CoininfoDlg.Instance.IsShow();
        CoininfoDlg.Instance.Open();

        base.Open();
    }

    public override void Close()
    {
        if(bFlag==false)
        {
            CoininfoDlg.Instance.Close();
        }
        //GameEdit.Instance.m_kShapeBtn.OnClickEvent();
        base.Close();
    }

 
    public override void OnSelect(int num)
    {
        
        base.OnSelect(num);
    }
    public override void OnBuy(int num)
    {
        CWHeroManager.Instance.UpdateShape(num + 1);
        CWQuestManager.Instance.CheckUpdateData(22, 1);//모양블록 구입
        UpdateData();
//        m_gScrollList.UpdateData();
        //ShapeSlot[] aa = EditInvenDlg.Instance.gameObject.GetComponentsInChildren<ShapeSlot>();
        //foreach (var v in aa)
        //{
        //    v.UpdateData();
        //}


    }

}
