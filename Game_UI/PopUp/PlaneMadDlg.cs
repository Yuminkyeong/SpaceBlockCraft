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

public class PlaneMadDlg : WindowUI<PlaneMadDlg>
{

    public Text m_kPrice1;
    public Text m_kPrice2;
    public Text m_kPrice3;
    public Text m_kPrice4;

    public Text m_kPrice;
    public Text m_kBlockName;
    public Text m_kSlotLevel;
    public Text m_kBlockLevel;
    public RawImage m_kBlockImage;
    public RawImage m_kAirplane;

    int m_nPrice = 0;
    int m_nKey = 0;

    public void Show(int nKey)
    {
        m_nKey = nKey;
        Open();
       
    }
    public override void UpdateData(bool bselect = true)
    {
        base.UpdateData(bselect);
        // 블록가격
        int blockcount = CWTableManager.Instance.GetTableInt("상점 - 비행기", "BlockCount", m_nKey);
        int slotlevel = CWTableManager.Instance.GetTableInt("상점 - 비행기", "SlotLevel", m_nKey);
        int blocklevel = CWTableManager.Instance.GetTableInt("상점 - 비행기", "BlockLevel", m_nKey);

        m_kBlockLevel.text = blocklevel.ToString();
        // 블록 가격

        int nID = CWArrayManager.Instance.GetLevelBlock(blocklevel);
        GITEMDATA gdata = CWArrayManager.Instance.GetItemData(nID);

        int price1 = blockcount * gdata.price;// 블록 가격 현재 내 인벤에 존재하는 블록은??
        int price2 = CWTableManager.Instance.GetTableInt("상점 - 비행기", "Price", m_nKey);
        int price3 = AirStoreDlg.Instance.GetSlotPrice(blockcount);// CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "AllPrice", nKey);
        int price4 = CWGlobal.WEAPONPRICE;
        int price = price1 + price2 + price3+price4;
        m_nPrice = price;
        m_kPrice1.text = price1.ToString();
        m_kPrice2.text = price2.ToString();
        m_kPrice3.text = price3.ToString();
        m_kPrice4.text = price4.ToString();

        m_kBlockImage.texture = CWResourceManager.Instance.GetItemIcon(nID);



        m_kBlockName.text = CWTableManager.Instance.GetTable("상점 - 비행기", "Block", m_nKey);
        int lv= CWTableManager.Instance.GetTableInt("상점 - 비행기", "SlotLevel", m_nKey);//

        string szvalues= CWLocalization.Instance.GetLanguage("Lv.{0}슬롯");
        m_kSlotLevel.text = string.Format(szvalues,lv);
        m_kPrice.text = price.ToString();
        string szname2 = CWTableManager.Instance.GetTable("상점 - 비행기", "Name", m_nKey);

        m_kAirplane.texture = CWResourceManager.Instance.GetTexture(szname2);


    }
    public void OnBuyItem()
    {
        // 새로운 슬롯
        // 비행기 습득 
        CWSocketManager.Instance.UseCoinEx(COIN.GOLD, -m_nPrice, () => {


            //CWAirObject aa = new CWAirObject
            string szname = CWTableManager.Instance.GetTable("상점 - 비행기", "Name", m_nKey);



            CWJSon jSon = new CWJSon();
            //if (m_kJSon.LoadGamedata(GetPath()) == null)
            string szfile= "AirCraft/" + szname;
            if(jSon.LoadGamedata(szfile)==null)
            {
                return;
            }
            int HP;
            int Attack;
            int Count;
            int MaxCount;
            int Level;
            byte[] BlockData = jSon.ToArray();

            HP     = CWTableManager.Instance.GetTableInt("상점 - 비행기", "HP_text", m_nKey);
            Attack = CWTableManager.Instance.GetTableInt("상점 - 비행기", "Attack", m_nKey);
            Count = CWTableManager.Instance.GetTableInt("상점 - 비행기", "BlockCount", m_nKey);
            Level = CWTableManager.Instance.GetTableInt("상점 - 비행기", "SlotLevel", m_nKey);
            MaxCount = CWTableManager.Instance.GetTableInt("슬롯강화 - 시트1", "blockcount", Level);

            //m_kIcon.texture = CWLib.LoadImage(string.Format("Ship_{0}", nID), CWGlobal.ICONIMAGESIZE, CWGlobal.ICONIMAGESIZE);
            


            CWSocketManager.Instance.CreateAirSlot(HP,Attack,Count,MaxCount,Level,BlockData,(jData)=> {

                CWJSon jj = new CWJSon(jData);
                int nID = jj.GetInt("ID");

                string szname2 = CWTableManager.Instance.GetTable("상점 - 비행기", "Name", m_nKey);
                Texture2D tt = CWResourceManager.Instance.GetTexture(szname2);

                CWLib.SaveImage(tt, string.Format("Ship_{0}", nID));
                NoticeMessage.Instance.Show("비행기를 구입하였습니다.");

            });

            Close();


        });


    }

}
