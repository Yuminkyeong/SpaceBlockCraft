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

public class ChattingDlg : WindowUI<ChattingDlg>
{

    public InputField m_kInput;

    int GetListCount()
    {
        return CWChattingManager.Instance.m_kList.Count;
    }
    int GetListColumnCount()
    {
        return 6;
    }
    string GetListColumn(int Col)
    {

        if (Col == 0) return "ID";
        if (Col == 1) return "CharNumber";
        if (Col == 2) return "Name";
        if (Col == 3) return "ChattingMsg";
        if (Col == 4) return "ChatTime";
        if (Col == 5) return "Grade";
        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        if (Col == 0)
        {
            return CWChattingManager.Instance.m_kList[Raw].ID.ToString();
        }
        if (Col == 1)
        {
            return "char_"+  CWChattingManager.Instance.m_kList[Raw].CharNumber.ToString();
        }
        if (Col == 2)
        {
            return CWChattingManager.Instance.m_kList[Raw].Name;
        }
        if (Col == 3)
        {
            return CWChattingManager.Instance.m_kList[Raw].ChattingMsg;
        }
        if (Col == 4)
        {
            return CWChattingManager.Instance.m_kList[Raw].ChatTime;
        }
        if (Col == 5)
        {
            return CWChattingManager.Instance.m_kList[Raw].Grade.ToString();
        }

        return "";
    }


    bool m_bChatflag = false;
    public override void Open()
    {
        
        if(m_bChatflag)
        {
            return;
        }
        m_bChatflag = true;

        CWSocketManager.Instance.GetChattingList((jData) => {


            CWChattingManager.Instance.m_kList.Clear();
            JArray ja = CWJSon.GetArray(jData, "ChattData");
            for (int i = 0; i < ja.Count; i++)
            {
                int ID = CWJSon.GetInt(ja[i], "ID");
                int CharNumber = CWJSon.GetInt(ja[i], "CharNumber");
                string Name = CWJSon.GetString(ja[i], "Name");
                string ChattingMsg = CWJSon.GetString(ja[i], "ChattingMsg");
                string ChatTime = CWJSon.GetString(ja[i], "ChatTime");
                int Grade = CWJSon.GetInt(ja[i], "Grade");
                CWChattingManager.Instance.AddChatt(ID, CharNumber, Name, ChattingMsg, ChatTime, Grade);
            }

            m_gScrollList.GetListCount = GetListCount;
            m_gScrollList.GetListColumnCount = GetListColumnCount;
            m_gScrollList.GetListValue = GetListValue;
            m_gScrollList.GetListColumn = GetListColumn;
            m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
            m_gScrollList.m_bStartClick = false;
            UpdateData();
            m_bChatflag = false;
            //StartCoroutine("IRun");
            base.Open();
        });
    }
    public override void Close()
    {
        CWSocketManager.Instance.CloseChatt();

        base.Close();
    }

    public void OnSendChatting()
    {
        CWSocketManager.Instance.SendChatting(m_kInput.text);
        m_kInput.text = "";
        ResetCursor();
    }
    public void ResetCursor()
    {
        m_gScrollList.NCursor = 0;
    }

    public void ShowUserInfo()
    {
        
    }
    public override void OnButtonClick(int num)
    {

        int ID= m_gScrollList.GetInt(num, "ID");

        base.OnButtonClick(num);
        UserInfoDlg.Instance.Show(ID);
    }
}
