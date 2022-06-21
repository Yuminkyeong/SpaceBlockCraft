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

public class divideCompleteDlg : MonoBehaviour
{


    public RawImage blockImage;
    public Text level;
    public Text blockname;
    public Text blockexplain;

    public RawImage blockImage2;
    public Text level2;
    public Text blockname2;
    public Text blockexplain2;



    GITEMDATA m_gItem1;
    GITEMDATA m_gItem2;

    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        UpdateData();
    }

    public void UpdateData()
    {

        m_gItem1 = CWArrayManager.Instance.GetItemData(divideDlg.Instance.m_nItem);
        m_gItem2 = CWArrayManager.Instance.GetItemData(divideDlg.Instance.m_nItem2);

        blockImage.texture = CWResourceManager.Instance.GetItemIcon(m_gItem1.nID);
        blockImage2.texture = CWResourceManager.Instance.GetItemIcon(m_gItem2.nID);

        level.text = m_gItem1.level.ToString();
        level2.text = m_gItem2.level.ToString();
        blockname.text = m_gItem1.m_szTitle;
        blockname2.text = m_gItem2.m_szTitle;
        blockexplain.text = m_gItem1.szInfo;
        blockexplain2.text = m_gItem2.szInfo;



    }
}

