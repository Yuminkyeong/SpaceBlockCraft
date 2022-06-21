using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class ListDBDlg : WindowUI<ListDBDlg>
{
    enum LTYPE { SENDMAIL,BLOCK,CASH};
    LTYPE m_Ltype;
    int MaxCount = 100;
    string m_szDBFile;
    string m_szQury;
    void Show(string szDBfile, string szqury)
    {
        m_szDBFile = szDBfile;
        m_szQury = szqury;
        Open();
    }
    public override void UpdateData(bool bselect = true)
    {

        JObject JData = new JObject();
        JData.Add("DBFile", m_szDBFile);
        JData.Add("query", m_szQury);
        JData.Add("limit", MaxCount);
        JData.Add("Start", 0);
        m_gScrollList.m_szDBParam = JData.ToString();

        base.UpdateData(bselect);
    }
    public void ShowSendMailUser(string szDBfile, string szqury)
    {
        m_Ltype = LTYPE.SENDMAIL;
        Show(szDBfile,szqury);
    }
    public void ShowBlockUser()
    {
        m_Ltype = LTYPE.BLOCK;
        JObject JData = new JObject();
        JData.Add("BlockUser",1);
        Show("UserDB", JData.ToString());
    }
    public void ShowCashUser()// 결제 한 유저
    {
        m_Ltype = LTYPE.CASH;
        JObject JData = new JObject();
        JData.Add("CashCount", 1);
        Show("UserDB", JData.ToString());
    }

    public override void OnButtonClick(int num)
    {
        if(m_Ltype == LTYPE.SENDMAIL)
        {
            int nID = m_gScrollList.GetInt(num, "_id");
            string  szName = m_gScrollList.GetString(num, "Name");
           // SendMailDlg.Instance.Show(nID, szName);
        }

        base.OnButtonClick(num);
    }
}
