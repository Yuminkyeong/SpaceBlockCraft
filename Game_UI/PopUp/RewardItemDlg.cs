using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class RewardItemDlg : WindowUI<RewardItemDlg>
{

    public RawImage m_kImage;
    public Text m_kName;
    public void Show(int Item)
    {

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(Item);
        m_kName.text =gData.m_szTitle;
        m_kImage.texture = CWResourceManager.Instance.GetItemIcon(Item);
        Open();

        m_kImage.transform.DOScale(1, 0).OnComplete(() => {

            m_kImage.transform.DOScale(2.2f, 0.5f);
        });
        Invoke("Close", 1.2f);


    }


}
