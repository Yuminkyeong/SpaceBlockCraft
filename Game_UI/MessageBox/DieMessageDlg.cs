using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;

public class DieMessageDlg : WindowUI<DieMessageDlg>
{
    public Text m_kName;
    public Text m_kName2;
    public void ShowSingle(CBClose cbclose)
    {
        CWResourceManager.Instance.PlaySound("Soft Fail");
        CloseFuction = cbclose;
        m_nSelectMode = 0;
        Open();
    }
    public void ShowMulti(CBClose cbclose,string szname="")
    {
        CWResourceManager.Instance.PlaySound("Soft Fail");
        CloseFuction = cbclose;
        m_nSelectMode = 1;
        m_kName2.gameObject.SetActive(true);
        string str = CWLocalization.Instance.GetLanguage("[{0}]에게 당했습니다!!!"); // string.Format("[{0}]에게 당했습니다!!!", szname);
        m_kName2.text = string.Format(str, szname);
        Open();
    }
    public override void Close()
    {

        base.Close();
    }
}
