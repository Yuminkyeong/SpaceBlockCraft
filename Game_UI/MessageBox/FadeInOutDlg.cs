using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using UnityEngine.UI;
using DG.Tweening;
using System.Runtime.InteropServices;

public class FadeInOutDlg : WindowUI<FadeInOutDlg>
{
    public Image m_kBkImage;
  
    //bFlag =true => 점점 어두어 진다
    //bFlag= false => 점점 밝아 진다
    bool m_bFlag=false;
    public void Show(bool bFlag,float ftime,string szText)
    {
        
        if (bFlag)
        {
            if (m_bFlag) return;
            Open();
            
            m_kBkImage.color = new Color(0, 0, 0, 0);

            m_kBkImage.DOFade(1, ftime).SetEase(Ease.Linear);
            
        }
        else
        {
            
            m_kBkImage.DOFade(0, ftime).SetEase(Ease.Linear).OnComplete(() => {
                Close();
            });

        }
        m_bFlag = bFlag;
        
    }
    public override void Close()
    {
        m_bFlag = false;
        base.Close();
    }

}
