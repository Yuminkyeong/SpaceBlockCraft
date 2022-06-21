using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;

public class BuyMessageDlg : WindowUI<BuyMessageDlg>
{
    public RawImage m_kImage;
    public Image m_kSprite;
    CBClose ClickOkFunc;
    //public int g_nCoinType
    //g_nPrice
    public void Show(string szGoods,string szGoodsHelp, string szImage,int nItem, int nCoinType,string szPrice, CBClose Func)
    {
        ValueUI.g_szPrice = szPrice;
        ValueUI.g_nCoinType = nCoinType;
        ValueUI.g_szGoodsHelp = szGoodsHelp;
        ValueUI.g_szGoodsName = szGoods;

        if(CWLib.IsString(szImage))
        {
            Sprite ss= CWResourceManager.Instance.GetSprite(szImage);
            if(ss)
            {
                m_kImage.gameObject.SetActive(false);
                m_kSprite.gameObject.SetActive(true);
                m_kSprite.sprite = ss;
            }
            else
            {
                m_kSprite.gameObject.SetActive(false);
                m_kImage.gameObject.SetActive(true);
                m_kImage.texture = CWResourceManager.Instance.GetTexture(szImage);

                


            }
            
            
        }
        else   if( nItem>0)
        {
            m_kSprite.gameObject.SetActive(false);
            m_kImage.gameObject.SetActive(true);

            m_kImage.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        }
            

        ClickOkFunc = Func;
        Open();
    }

    public void Show(string szImage, CBClose Func)
    {
        if (CWLib.IsString(szImage))
        {
            Sprite ss = CWResourceManager.Instance.GetSprite(szImage);
            if (ss)
            {
                m_kImage.gameObject.SetActive(false);
                m_kSprite.gameObject.SetActive(true);
                m_kSprite.sprite = ss;
            }
            else
            {
                m_kSprite.gameObject.SetActive(false);
                m_kImage.gameObject.SetActive(true);
                m_kImage.texture = CWResourceManager.Instance.GetTexture(szImage);
            }


        }

        ClickOkFunc = Func;
        Open();
    }

 
    public void ClickOK()
    {
        ClickOkFunc();
        Close();
    }
}
