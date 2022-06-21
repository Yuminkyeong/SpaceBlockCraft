using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;
using CWStruct;
public class TakeItemDlg : EffectUIDlg<TakeItemDlg>
{
    public GameObject m_gEffectDir;
    
    public CWAirObject m_kSelectBlock;

    public Text m_kDescription;
    public RawImage m_kImage;
    public Text m_kTitle;
    int m_nItem;
    string m_szTitle;
    public void ShowCoin(COIN kCoin,CBClose fuc)
    {
        m_kDescription.text = "";
        CloseFuction = fuc;
        m_kSelectBlock.gameObject.SetActive(false);
        m_kImage.gameObject.SetActive(true);
        
        if(kCoin==COIN.GEM)
        {
            m_szTitle = "다이아";
            m_kImage.texture = CWResourceManager.Instance.GetTexture("Dia_1");
        }
        if (kCoin == COIN.GOLD)
        {
            m_szTitle = "골드";
            m_kImage.texture = CWResourceManager.Instance.GetTexture("Gold_1");
        }
        if (kCoin == COIN.TICKET)
        {
            m_szTitle = "입장권";
            m_kImage.texture = CWResourceManager.Instance.GetTexture("Ticket_1");
        }

        Open();
    }
    public void ShowImage(string szImage,string szTitle,string szDescript, CBClose fuc)
    {
        m_kDescription.text = szDescript;
        CloseFuction = fuc;
        m_kSelectBlock.gameObject.SetActive(false);
        m_kImage.gameObject.SetActive(true);
        m_szTitle = szTitle;

        m_kImage.texture = CWResourceManager.Instance.GetTexture(szImage);
        Open();
    }
    public void Show(int nItem, CBClose fuc)
    {
        
        CloseFuction = fuc;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if(gData.type== "shipblock" || gData.type == "color" || gData.type == "Gold" )
        {
            m_kImage.gameObject.SetActive(false);
            m_kSelectBlock.gameObject.SetActive(true);
            m_kSelectBlock.m_nLayer = GameLayer.UI;
            m_kSelectBlock.EmptyCreate();
            m_kSelectBlock.ClearBlock();
            m_kSelectBlock.SetBlock(16, 5, 16, nItem, 0, 0);
            m_kSelectBlock.UpdateBlock();

        }
        else
        {
            m_kSelectBlock.gameObject.SetActive(false);

            m_kImage.gameObject.SetActive(true);

            m_kImage.texture = CWResourceManager.Instance.GetItemIcon(nItem);




        }
        m_kDescription.text = gData.szInfo;


        m_szTitle = gData.m_szTitle;
        m_nItem = nItem;
        Open();
    }

    protected override void _Open()
    {
        Game_App.Instance.m_gUIDir.SetActive(false);
        Game_App.Instance.m_gEffectCam.orthographic = false;
        m_gEffectDir.SetActive(true);
        
        m_kTitle.text = m_szTitle;



        base._Open();
    }
    public override void Close()
    {
        Game_App.Instance.m_gUIDir.SetActive(true);
        Game_App.Instance.m_gEffectCam.orthographic = true;
        m_gEffectDir.SetActive(false);
        
        
        base.Close();
    }
}
