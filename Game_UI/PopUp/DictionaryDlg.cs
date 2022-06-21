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

public class DictionaryDlg : WindowUI<DictionaryDlg>
{

    

    protected override void _Open()
    {
        m_nGroupType = 1;
        base._Open();
    }

    public override void Close()
    {
        base.Close();
    }

    SLOTITEM GetConvertSlot(int num)
    {
        int nSlot = m_gScrollList.GetInt(num, "Slot");
        return CWInvenManager.Instance.GetItembySlot(nSlot);
    }

    
    public override void OnSelect(int num)
    {

        m_nSelectItem = m_gScrollList.GetInt(num, "ItemID");
        SLOTITEM ss = GetConvertSlot(num);
        if (ss != null)
            ss.m_nNewItem = 0;
        m_kSelectSlotItem = ss;
        UpdateItem();
        base.OnSelect(num);
    }

    [SerializeField]
    int m_nSelectItem;

    SLOTITEM m_kSelectSlotItem;

    public RawImage m_kItem;
    public Text m_kLevel;
    public Text m_kHp;
    public Text m_kPrice;
    public Text m_kName;
    public Text m_kInfo;
    void UpdateItem()
    {
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(m_nSelectItem);
        m_kItem.texture = CWResourceManager.Instance.GetItemIcon(m_nSelectItem);

        m_kInfo.text = gData.szInfo;
        if (m_kLevel)
            m_kLevel.text = gData.level.ToString();

        if (m_kHp)
            m_kHp.text = gData.hp.ToString();

        if (m_kPrice)
            m_kPrice.text = gData.price.ToString();

        if (m_kName)
            m_kName.text = gData.m_szTitle;
    }

    public void OnRewardItem()
    {

        CWHeroManager.Instance.UpdateDicItem(ValueUI.g_kSelectItemData.nID, 2, () => {

            CWSocketManager.Instance.UseCoinEx(COIN.GEM, 1);

            UpdateData();
        });
    }

}
