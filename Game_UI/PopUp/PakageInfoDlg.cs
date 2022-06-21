using System;
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

public class PakageInfoDlg : WindowUI<PakageInfoDlg>
{
    public Text m_kTitle;
    public RawImage m_kImageIcon;
    public RawImage[] m_kImages;
    public Text[] m_kItemNames;
    //public Text[] m_kItemCount;

    CBClose ClickOkFunc;

    
    public void Show(string szTitle, string szItemArray,string szCountArray,string szImage, CBClose Func)
    {


        m_kTitle.text = szTitle;
        

        m_kImageIcon.texture = CWResourceManager.Instance.GetTexture(szImage);
        string[] szArray = szItemArray.Split(',');
        string[] szArrayCount = szCountArray.Split(',');
        foreach (var v in m_kImages) v.gameObject.SetActive(false);
        foreach (var v in m_kItemNames) v.gameObject.SetActive(false);

        for (int i=0;i<szArray.Length;i++)
        {
            if(m_kImages.Length<=i)
            {
                break;
            }
            int nID = CWLib.ConvertInt(szArray[i]);
            int nCount = CWLib.ConvertInt(szArrayCount[i]); 
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nID);
            m_kImages[i].gameObject.SetActive(true);
            m_kItemNames[i].gameObject.SetActive(true);

            m_kImages[i].texture = CWResourceManager.Instance.GetItemIcon(nID);
            m_kItemNames[i].text = gData.m_szTitle+"x"+ nCount.ToString(); 

        }


        ClickOkFunc = Func;
        Open();
    }

    public void OnBuy()
    {
        ClickOkFunc();
        Close();
    }

}
