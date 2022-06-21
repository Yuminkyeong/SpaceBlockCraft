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

public class MailDlg : WindowUI<MailDlg>
{

    public override void OnButtonClick(int num)
    {
        int nMailId= m_gScrollList.GetInt(num, "_id");
        string szTitle = m_gScrollList.GetString(num, "Subject");
        string szMessage = m_gScrollList.GetString(num, "Maintext");
        string szDate = m_gScrollList.GetDateTime(num, "Regdate").ToString();
        int Cash = m_gScrollList.GetInt(num, "Cash");

        List<int> kItem = new List<int>();
        List<int> kItemCount = new List<int>();

        JToken jItem = m_gScrollList.GetJObject(num, "Item");
        JToken jItemCount = m_gScrollList.GetJObject(num, "ItemCount");

        if(CWLib.IsJSonData(jItem))
        {
            kItem.AddRange(jItem.ToObject<int[]>());
            kItemCount.AddRange(jItemCount.ToObject<int[]>());
        }
        MailPopupDlg.Instance.Show(nMailId, szTitle,szMessage, szDate, Cash, kItem,kItemCount);
        base.OnButtonClick(num);
       
    }
    public void OnAll()
    {
        CWSocketManager.Instance.UseAllMail( (jData) => {

            if (jData["Result"].ToString() == "ok")
            {
                CWCoinManager.Instance.SetData(jData["Coins"]);
            }
            else
            {
                print("shop fail!!");
            }
            UpdateData();
            NoticeMessage.Instance.Show("선물을 받았습니다!", 2);

        }, "UseData");

    }
    public override void Open()
    {
        
        base.Open();
    }
    public override void Close()
    {

        CWSocketManager.Instance.CheckMail();
        base.Close();
    }

}
