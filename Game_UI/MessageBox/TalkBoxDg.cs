using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWUnityLib;

public class TalkBoxDg : TalkWindow<TalkBoxDg>
{
    
    public RawImage m_kImage;
    public void Show(Texture2D kImage,string sztext, float ftime,CBClose cbfuc,bool bclose=true)
    {
        m_bClosed = bclose;
        CloseFuction = cbfuc;
        m_kImage.texture = kImage;
        m_kText.text = sztext;
        CreateTalk(sztext);
        Open();

        

        Invoke("Close", ftime);

    }
  
}
