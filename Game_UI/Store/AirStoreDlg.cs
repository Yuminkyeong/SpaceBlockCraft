using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using System.IO;
using SimpleJSON;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWStruct;
using CWEnum;
using CWUnityLib;
using System;

public class AirStoreDlg : WindowUI<AirStoreDlg>
{

    public Text m_kAirName;
    public Text m_kBlockInfo;
    public RawImage m_kBlockImage;
    public RawImage m_kIngredient;
    public Text m_HP;
    public Text m_kPrice;
    public Text m_kBlockname;

    public void OnMadePlane()
    {
        int nKey = m_gScrollList.GetSelectValueInt("key");
        PlaneMadDlg.Instance.Show(nKey);
    }

    public int GetSlotPrice(int BlockCount)
    {
        CWArrayManager.SlotPowerData ss = CWArrayManager.Instance.GetSlotData(BlockCount);

        if(ss!=null)
        {
            return ss.m_Price;
        }
        return 0;
    }

    void UpdateInfo(int nKey)
    {
        int blockcount = CWTableManager.Instance.GetTableInt("상점 - 비행기", "BlockCount", nKey);
        int slotlevel = CWTableManager.Instance.GetTableInt("상점 - 비행기", "SlotLevel", nKey);
        int blocklevel = CWTableManager.Instance.GetTableInt("상점 - 비행기", "BlockLevel", nKey);

        // 블록 가격

        int nID = CWArrayManager.Instance.GetLevelBlock(blocklevel);
        GITEMDATA gdata = CWArrayManager.Instance.GetItemData(nID);

        int price1 = blockcount * gdata.price;// 블록 가격 현재 내 인벤에 존재하는 블록은??
        int price2 = CWTableManager.Instance.GetTableInt("상점 - 비행기", "Price", nKey);
        int price3 = GetSlotPrice(blockcount);// CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "AllPrice", nKey);
        int price4 = CWGlobal.WEAPONPRICE;

        int price = price1 + price2 + price3+ price4;

        int tcnt = CWInvenManager.Instance.GetItemTotalCount(nID);

        m_kBlockInfo.text = string.Format("{0}/{1}", tcnt, blockcount);

        m_kAirName.text = CWTableManager.Instance.GetTable("상점 - 비행기", "Title", nKey);

        string szname = CWTableManager.Instance.GetTable("상점 - 비행기", "Name", nKey);
        
        m_HP.text = CWTableManager.Instance.GetTable("상점 - 비행기", "HP_text", nKey);
        m_kPrice.text = price.ToString();
        m_kBlockname.text = CWTableManager.Instance.GetTable("상점 - 비행기", "block_name_text", nKey);

        //
        m_kBlockImage.texture = CWResourceManager.Instance.GetTexture(szname);
        m_kIngredient.texture = CWResourceManager.Instance.GetItemIcon(nID);// CWResourceManager.Instance.GetTexture(ingredientname);


    }
    public override void OnSelect(int num)
    {
        base.OnSelect(num);

        int nKey = m_gScrollList.GetSelectValueInt("key");

        UpdateInfo(nKey);

    }


}
