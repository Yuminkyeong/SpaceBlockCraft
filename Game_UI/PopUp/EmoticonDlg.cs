using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using CWEnum;
public class EmoticonDlg : WindowUI<EmoticonDlg>
{

    public override void OnSelect(int num)
    {

        base.OnSelect(num);
    }
    public override void OnButtonClick(int num)
    {
        int nKey = m_gScrollList.GetInt(num, "key");
        CWSocketManager.Instance.SendEmoticon(nKey);
        Close();


        string szEmoticon = ((Emoticon)nKey).ToString();
        Vector3 vPos = CWHero.Instance.GetPosition();
        vPos.y += 10f;
        CWPoolManager.Instance.GetParticle(vPos, szEmoticon);

        base.OnButtonClick(num);
    }
}
