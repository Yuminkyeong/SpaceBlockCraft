using System;
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

public class FindUserDlg : WindowUI<FindUserDlg>
{

    public InputField m_kText;

    public void OnSend()
    {
        //string m_szQueryRanking = @"{{$regex: {} }}";
        //string.Format(m_szQueryRanking, m_kText.text);


        JObject JData = new JObject();
        JObject JData2 = new JObject();
        JData2.Add("$regex", m_kText.text);
        JData.Add("Name", JData2);
        Close();
        ListDBDlg.Instance.ShowSendMailUser("UserDB", JData.ToString());
        
    }
    public void Show()
    {
        
        Open();
    }


}
