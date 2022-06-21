using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWUnityLib;

public class BossTalkBoxDlg : TalkWindow<BossTalkBoxDlg>
{
    public RawImage m_kImage;
    CWProductionRoot m_kRoot;
    public void Show(CWProductionRoot kRoot, Texture2D kImage, string sztext, float ftime, CBClose cbfuc, bool bclose = true)
    {
        m_kRoot = kRoot;
       m_bClosed = bclose;
        CloseFuction = cbfuc;
        m_kImage.texture = kImage;
        m_kText.text = sztext;

        CreateTalk(sztext);
        Open();
        Invoke("Close", ftime);
    }

    
 
    public void OnSkipProduction()
    {

        m_kRoot.Close();
    }
}
