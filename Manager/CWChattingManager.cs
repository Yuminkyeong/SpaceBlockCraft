using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWChattingManager : CWManager<CWChattingManager>
{
    const int MAXCOUNT = 100;
    public struct ChatData
    {
        public int ID;
        public int CharNumber;
        public string Name;
        public string ChattingMsg;
        public string ChatTime;
        public int Grade;
    }

    public List<ChatData> m_kList = new List<ChatData>();

    public override void Create()
    {

        base.Create();
    }
    public void SystemMessage(string szstr)
    {
        ////<color=#00ffff> aaaaaaaaaaa</color>
        //string szRecord = string.Format("<color=#00ffff>{0}</color>", CWLocalization.Instance.GetLanguage(szstr));
        //m_kList.Add(szRecord);
        //if (m_kList.Count > MAXCOUNT)
        //{
        //    m_kList.RemoveAt(0);
        //}
        AddChatt(0, 0, "", szstr,DateTime.Now.ToString(),0);
    }
    public void AddChatt(int ID,int CharNumber,string Name,string ChattingMsg,string ChatTime,int Grade)
    {
        ChatData cdata = new ChatData();
        cdata.ID = ID;
        cdata.CharNumber = CharNumber;
        cdata.Name = Name;
        cdata.ChattingMsg = ChattingMsg;
        cdata.ChatTime = ChatTime;
        cdata.Grade = Grade;
        m_kList.Add(cdata);
        if(m_kList.Count> MAXCOUNT)
        {
            m_kList.RemoveAt(0);
        }
        ChattingDlg.Instance.UpdateData();
        
    }
    public List<ChatData> GetData(int nMax)
    {
        return m_kList;
    

    }


}
