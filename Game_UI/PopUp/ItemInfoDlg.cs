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

public class ItemInfoDlg : WindowUI<ItemInfoDlg>
{

    enum GTYPE {INVEN,TEMPINVEN,PARMING };
    GTYPE m_gType;
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

    int m_nItem;
    int m_nCount;
    int m_nSlot;
    
    
    public Text m_kbtntext;

    void UpdateItem()
    {

        m_kTitlePower1.gameObject.SetActive(false);
        m_kTitlePower2.gameObject.SetActive(false);
        m_kTitlePower3.gameObject.SetActive(false);
        

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nItem);
        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(m_nItem);
        m_kCount.text = m_nCount.ToString();
        m_kGrade.text = CWGlobal.GetGradeItemName(gData.level);

        m_kTitle.text = gData.m_szTitle;

        m_kPrice.text = gData.pricesell.ToString();

        if (gData.type=="shipblock")
        {
            m_kTitlePower1.gameObject.SetActive(true);
            m_kPower1.text = gData.hp.ToString();
            m_kTitlePower1.text = "HP";

            m_kTip.text = "비행기의 HP를 늘리고 모형을 만듭니다";

        }
        if (gData.type == "weapon")
        {
            m_kTitlePower1.gameObject.SetActive(true);
            m_kTitlePower2.gameObject.SetActive(true);

            m_kTitlePower1.text = "Damage";
            m_kTitlePower2.text = "Speed";
            WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
            m_kPower1.text = ws.Damage.ToString();
           // m_kPower2.text = ws.Speed.ToString();
            m_kTip.text = "업그레이드를 하여 공격력을 늘립니다";

        }
        if (gData.type == "보석")
        {
            m_kTip.text = "강화에 사용됩니다";
        }
        if (gData.type == "color")
        {
            m_kTip.text = "비행기에 색을 칠합니다";

        }
        if (gData.type == "Buster")
        {
            m_kTip.text = "비행 속도를 늘립니다";
        }
        //if (gData.type == "폭탄")
        //{
        //    m_kTitlePower1.gameObject.SetActive(true);
        //    m_kTip.text = "많은 블록을 얻을 수 있습니다";
        //    m_kTitlePower1.text = "범위";
        //    WEAPON ws = CWArrayManager.Instance.GetWeapon(m_nItem);
        //    m_kPower1.text = ws.Range.ToString();

        //}
        if (gData.type == "아이템")
        {
            m_kTip.text = "";
        }

        // 보석, 폭탄은 판매금지 
        // 
        if (gData.type == "폭탄" || gData.type == "보석")
        {
            m_kbtntext.text = "판매불가";
        }
        else
        {
            m_kbtntext.text = "판매";
        }
        



    }
    public void UseCoin_ResultFuc(JObject _Data)
    {

        if (_Data["Result"].ToString() == "ok")
        {
            CWCoinManager.Instance.SetData(_Data["Coins"]);
            if(m_gType == GTYPE.INVEN)
            {
                CWInvenManager.Instance.DelSlot(m_nSlot); 
            }
            if (m_gType == GTYPE.TEMPINVEN|| m_gType == GTYPE.PARMING)
            {
                
            }
            
        }
        else
        {
            //faile!!
            print("shop fail!!");
        }
        Close();
    }

    public void OnBuy()
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nItem);
        if(gData.nID==(int)GITEM.Diamond)
        {
            // 다이아 몬드는 바로 캐시로 
            CWSocketManager.Instance.UseCoin(COIN.GEM,  m_nCount, UseCoin_ResultFuc, "Iteminfo");
            return;
        }
        if(gData.nID==(int)GITEM.Ticket)
        {
            CWSocketManager.Instance.UseCoin(COIN.TICKET, m_nCount, UseCoin_ResultFuc, "Iteminfo");
            return;

        }

        if (gData.type == "폭탄" ||  gData.type == "보석")
        {
            NoticeMessage.Instance.Show("판매 할수 없습니다!");
            return;
        }

        CWSocketManager.Instance.UseCoin(COIN.GOLD, gData.pricesell* m_nCount, UseCoin_ResultFuc, "Iteminfo");

    }
    public void Show(int nType,int nSlot, CBClose _Closefuc,Vector2 vPos)
    {
        m_gType = (GTYPE)nType;
        CloseFuction = _Closefuc;
        
        m_nSlot = nSlot;
        
        RectTransform rt = m_visible[0].GetComponent<RectTransform>();
        rt.anchoredPosition = vPos;// new Vector2(-198,44);

        if (m_gType == GTYPE.INVEN)
        {



            m_nItem = CWInvenManager.Instance.GetItemSlot(nSlot);
            m_nCount = CWInvenManager.Instance.GetItemSlotCount(nSlot);
        }
        else
        {
            //RectTransform rt = m_visible[0].GetComponent<RectTransform>();
            //rt.anchoredPosition = new Vector2(-662, -90);



        }
        if(m_nItem==0)
        {
            if (m_bShow)
            {
                Close();
            }
            return;
        }

        if (m_bShow)
        {
            UpdateItem();
            return;
        }
        Open();
    }
    protected override void _Open()
    {
        UpdateItem();
        base._Open();
    }


}
