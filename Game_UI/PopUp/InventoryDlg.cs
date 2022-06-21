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

public class InventoryDlg : WindowUI<InventoryDlg>
{
    //0:블록 1: 색블록, 2 무기, 3 아이템

    public Image m_kCoin;

    int _nType = 0;
    public int m_nType
    {
        get
        {
            return _nType;
        }
        set
        {
            _nType = value;

            UpDateInven();
        }
    }
    // 현재 선택된 탭에 맞게 인벤에 맞춘다
    SLOTITEM GetConvertSlot(int num)
    {
        int nSlot= m_gScrollList.GetInt(num, "Slot");
        return CWInvenManager.Instance.GetItembySlot(nSlot);
    }

    int GetListCount()
    {
        return CWInvenManager.Instance.m_nInvenDB.Count;
    }
    int GetListColumnCount()
    {
        return 7;
    }
    string GetListColumn(int Col)
    {
        if (Col == 0) return "Slot";
        if (Col == 1) return "ItemID";
        if (Col == 2) return "Count";
        if (Col == 3) return "New";
        if (Col == 4) return "Check";
        if (Col == 5) return "InvenType";
        if (Col == 6) return "Level";
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].m_nSlot.ToString();
        }
        if (CWInvenManager.Instance.m_nInvenDB[Raw].NItem == 0) return "";
        if (CWInvenManager.Instance.m_nInvenDB[Raw].NCount == 0) return "";

        if (Col == 1)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].NItem.ToString();
        }
        if (Col == 2)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].NCount.ToString();
        }
        if (Col == 3)
        {
            return CWInvenManager.Instance.m_nInvenDB[Raw].m_nNewItem.ToString();
        }
        if (Col == 4)
        {
            if (CWInvenManager.Instance.m_nInvenDB[Raw].m_bCheck) return "true";
            return "false";
        }
        if (Col == 5)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(CWInvenManager.Instance.m_nInvenDB[Raw].NItem);
            return gData.InvenType.ToString();
            //return CWInvenManager.Instance.m_nInvenDB[Raw].m_nNewItem.ToString();
        }

        if (Col == 6)
        {

            int level = CWArrayManager.Instance.GetItemLevel(CWInvenManager.Instance.m_nInvenDB[Raw].NItem);
           
            return "Lv."+level.ToString();
        }

        return "";
    }

    void UpDateInven()
    {
        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;
        m_gScrollList.m_szCondition = "InvenType = " + m_nType.ToString();
        

        m_bCheck = false;
        CWInvenManager.Instance.ReArrange();
        UpdateData();
        SetSelect(0);
    }
    protected override void _Open()
    {
        UpDateInven();
        base._Open();
        SetSelect(0);

    }
    public override void SetSelect(int num)
    {
        m_nSelectItem = m_gScrollList.GetInt(num, "ItemID");
        base.SetSelect(num);
    }
    public void OnReArrange()
    {
        CWInvenManager.Instance.ReArrange();

        UpdateData();

    }
    public void Show(CBClose cbfunc)
    {
        CloseFuction = cbfunc;
        Open();
        m_nType = 0;
    }
    public override void OnSelect(int num)
    {

        m_nCursor = 0;

        m_nSelectItem = m_gScrollList.GetInt(num, "ItemID");
        //
        SLOTITEM ss = GetConvertSlot(num);
        if (ss!=null)
            ss.m_nNewItem = 0;
        m_kSelectSlotItem = ss;
        UpdateItem();
        base.OnSelect(num);
    }
    void DelCheckFile()
    {
        foreach (var v in CWInvenManager.Instance.m_nInvenDB)
        {
            if (v.m_bCheck)
            {
                v.NCount = 0;
                v.NItem = 0;
            }
        }
        UpdateData();
    }
    public void OnBuyItem()
    {

        MessageBoxDlg.Instance.Show(() => {

            GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nSelectItem);
            int nPrice = gData.pricesell * m_nCount;
            COIN nType = COIN.GOLD;
            if(gData.nID==(int)GITEM.Diamond)
            {
                nType = COIN.GEM;
            }
            if (gData.nID == (int)GITEM.Ticket)
            {
                nType = COIN.TICKET;
            }

            

            CWSocketManager.Instance.UseCoinEx(nType, nPrice, () => {
                m_kSelectSlotItem.NCount -= m_nCount;
                CWResourceManager.Instance.PlaySound("sellsound");
                UpdateItem();
                UpdateData();
                SetSelect(0);
                m_nCursor = 0;
            });



        }, null, "", "판매하시겠습니까?");


    }
    public void OnBuyCheckItem()
    {
        int nPrice = 0;
        foreach (var v in CWInvenManager.Instance.m_nInvenDB)
        {

            if (v.m_bCheck)
            {

                if (v.NItem == (int)GITEM.Diamond)
                {
                    // 다이아 몬드는 바로 캐시로 
                    CWSocketManager.Instance.UseCoinEx(COIN.GEM, v.NCount, ()=> {


                    });
                    continue;
                }
                if (v.NItem == (int)GITEM.Ticket)
                {
                    CWSocketManager.Instance.UseCoinEx(COIN.TICKET, v.NCount, ()=> {
                    });
                    continue;

                }

                GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
                nPrice += gData.pricesell * v.NCount;
            }
        }

        

        if (nPrice > 0)
        {
            CWSocketManager.Instance.UseCoinEx(COIN.GOLD, nPrice, ()=> {
            });
        }

        DelCheckFile();
        //ItemInfoDlg.Instance.Close();

    }

    public override void OnButtonClick(int num)
    {

        SLOTITEM ss= GetConvertSlot(num);
        ss.m_bCheck = !ss.m_bCheck;
        UpdateData();
        base.OnButtonClick(num);
    }


    public GameObject[] m_kCheck;
    bool _bCheck;
    bool m_bCheck
    {
        get
        {
            return _bCheck;
        }
        set
        {
            _bCheck = value;
            m_kCheck[0].SetActive(_bCheck);
            m_kCheck[1].SetActive(!_bCheck);
        }
    }


    public void OnAllCheck()
    {
        m_bCheck = !m_bCheck;
        int tcnt = m_gScrollList.GetCount();
        for(int i=0;i<tcnt;i++)
        {
            SLOTITEM ss=GetConvertSlot(i);
            ss.m_bCheck = m_bCheck;
        }
        
        //foreach (var v in CWInvenManager.Instance.m_nInvenDB)
        //{
        //    // 폭탄, 다이아 제외
        //    GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.NItem);
        //    if (gData.type == "shipblock")
        //    {
        //        if (gData.level >= 11)
        //        {
        //            continue;
        //        }
        //    }
        //    if (gData.type == "폭탄" || gData.type == "Ticket" || gData.type == "보석")
        //    {
        //        continue;
        //    }
        //    if (gData.nID == (int)GITEM.Diamond) continue;
        //    v.m_bCheck = m_bCheck;
        //}
        UpdateData();
    }

    public void OnType1()
    {
        m_nType = 0;
    }
    public void OnType2()
    {
        m_nType = 1;
    }
    public void OnType3()
    {
        m_nType = 2;
    }
    public void OnType4()
    {
        m_nType = 3;
    }

    #region 아이템 정보 

    SLOTITEM m_kSelectSlotItem;

    public RawImage m_kItem;
    public Text m_kCount;

    public Text m_kGrade;
    public Text m_kPower1;
    public Text m_kPower2;
    public Text m_kPower3;
    public Text m_kPrice;

    public Text m_kTitlePower1;
    public Text m_kTitlePower2;
    public Text m_kTitlePower3;

    public Text m_kTip;
    public Text m_kTitle;
    public Text m_kMoneyTotal;
    int m_nSelectItem;

    // 슬롯이 없을 경우 처리 
    public override void EmptySlot()
    {
        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(0);
        m_kCount.text = "";
        m_kGrade.text = "";
        m_kPower1.text = "";
        m_kPower2.text = "";
        m_kPower3.text = "";
        m_kPrice.text = "";

        m_kTitlePower1.text = "";
        m_kTitlePower2.text = "";
        m_kTitlePower3.text = "";

        m_kTip.text = "";
        m_kTitle.text = "";
        m_kMoneyTotal.text = "";
        ValueUI.g_kSelectItemData.m_szTitle="";
    }


    int m_nCount
    {
        get
        {
            if (m_kSelectSlotItem == null) return 0;
            return m_kSelectSlotItem.NCount + m_nCursor;

        }
    }

    int m_nCursor = 0;

    // 판매 정보
    void BuyInfo()
    {
        if (m_kSelectSlotItem == null) return;

        if (m_nCursor >= 0) m_nCursor = 0;
        if (m_kSelectSlotItem.NCount + m_nCursor <= 0) m_nCursor = -m_kSelectSlotItem.NCount;

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nSelectItem);
        if (m_kCount)
            m_kCount.text = m_nCount.ToString();

        m_kMoneyTotal.text = (gData.pricesell * m_nCount).ToString();

    }

    public void OnLeft()
    {
        m_nCursor--;
        BuyInfo();

    }
    public void OnRight()
    {
        m_nCursor++;
        BuyInfo();

    }
    public void OnWeaponEnchant()
    {
        Close();
        WeaponUpgradeDlg.Instance.Open();
        WeaponUpgradeDlg.Instance.CloseFuction = Open;

    }
    public void OnBlockEnchant()// 분해로 바꿈
    {
        Close();
        divideDlg.Instance.Show(m_nSelectItem);
        divideDlg.Instance.CloseFuction = Open;
        
    }

    void UpdateItem()
    {
        
        m_kTitlePower1.gameObject.SetActive(false);
        m_kTitlePower2.gameObject.SetActive(false);
        m_kTitlePower3.gameObject.SetActive(false);


        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nSelectItem);
        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(m_nSelectItem);
        if(m_kCount)
            m_kCount.text = m_nCount.ToString();
        
        int nLevel = (m_nSelectItem / 256) + 1;

        if (m_kGrade)
            m_kGrade.text = "Lv." + nLevel.ToString();

        if (m_kTitle)
            m_kTitle.text = gData.m_szTitle;

        if (m_kPrice)
            m_kPrice.text = gData.pricesell.ToString();

        if (gData.type == "shipblock")
        {
            m_kTitlePower1.gameObject.SetActive(true);
            m_kPower1.text = gData.hp.ToString();
            m_kTitlePower1.text = "HP";
            if (m_kTip)
                m_kTip.text = "비행기의 HP를 늘리고 모형을 만듭니다";

        }
        if (gData.type == "weapon")
        {
            m_kTitlePower1.gameObject.SetActive(true);
            ///m_kTitlePower2.gameObject.SetActive(true);

            m_kTitlePower1.text ="Damage";
            //m_kTitlePower2.text = "Speed";
            WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nSelectItem);
            m_kPower1.text = ws.Damage.ToString();
            // m_kPower2.text = ws.Speed.ToString();


            int damage=CWHeroManager.Instance.GetWeaponDamage(ws.nType, nLevel);//
            m_kPower1.text = damage.ToString();
            if (m_kTip)
                m_kTip.text = "업그레이드를 하여 공격력을 늘립니다";

        }
        if (gData.type == "보석")
        {
            if (m_kTip)
                m_kTip.text = "강화에 사용됩니다";
        }
        if (gData.type == "color")
        {
            if (m_kTip)
                m_kTip.text = "비행기에 색을 칠합니다";

        }
        if (gData.type == "Buster")
        {
            if (m_kTip)
                m_kTip.text = "비행 속도를 늘립니다";
        }
        BuyInfo();

        if(gData.nID==(int)GITEM.Diamond)
        {
            m_kCoin.sprite = CWResourceManager.Instance.GetSprite("gem");
        }
        else
        {
            m_kCoin.sprite = CWResourceManager.Instance.GetSprite("gold");
        }

    }

    
    


    #endregion

}
