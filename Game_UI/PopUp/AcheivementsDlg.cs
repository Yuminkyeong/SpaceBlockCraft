using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;

public class AcheivementsDlg : WindowUI<AcheivementsDlg>
{

    protected override int GetUINumber()
    {
        return 8;
    }

    #region 퀘스트
    int GetListCount()
    {
        return CWQuestManager.Instance.GetCount();
    }
    int GetListColumnCount()
    {
        return 11;//
    }
    // 
    string GetListColumn(int Col)
    {
        if (Col == 0) return "key";
        if (Col == 1) return "ID";
        if (Col == 2) return "title";
        if (Col == 3) return "reward";
        if (Col == 4) return "Count";
        if (Col == 5) return "Icon";
        if (Col == 6) return "State";
        if (Col == 7) return "Rate";
        if (Col == 8) return "RateString";// 달성율 표시
        if (Col == 9) return "subtile";


        return "";
    }
    string GetListValue(int Raw, int Col)
    {
        int num = Raw;
        QUESTDATA qData = m_kData[num];// CWQuestManager.Instance.GetQuest(num);
        if (qData == null)
        {
            return "";
        }
        if (qData.m_nID == 0) return "";
        if (Col == 0)
        {
            return num.ToString();
        }
        if (Col == 1)
        {
            return qData.m_nID.ToString();
        }
        if (Col == 2)
        {
            return qData.m_szTitle;
        }
        if (Col == 3)
        {
            return qData.m_nRewardCount.ToString();
        }
        if (Col == 4)
        {
            return qData.m_nCount.ToString() + "/" + qData.m_nMaxCount.ToString();
        }
        if (Col == 5)
        {
            return qData.m_szIcon;
        }
        if (Col == 6)
        {

            return CWQuestManager.Instance.GetState(qData.m_nID).ToString();

        }
        if (Col == 7)
        {
            return CWQuestManager.Instance.GetRate(qData.m_nID).ToString();

        }
        if (Col == 8)
        {
            return CWQuestManager.Instance.GetRateString(qData.m_nID);

        }
        if (Col == 9)
        {
            return qData.m_szHelp;

        }


        return "";
    }

    List<QUESTDATA> m_kData = new List<QUESTDATA>();

    void AddData(QUESTDATA qData)
    {
        //Exists(x => x == UserID))
        if (m_kData.Exists(x => x == qData)) return;

        if(qData.m_nID==31)
        {
            Debug.Log("");
        }
        m_kData.Add(qData);

    }

    public void OnQuest()
    {
        CWQuestManager.Instance.UpdatePvpData();
        m_kData.Clear();
        for (int i = 1; i < CWQuestManager.MAXQUEST; i++)
        {
            QUESTDATA qData = CWQuestManager.Instance.GetQuestByID(i);
            if (qData == null) continue;
            if (qData.m_nCount == CWQuestManager.MAXCOUNT) continue;
            if (qData.m_nCount >= qData.m_nMaxCount)
            {
                AddData(qData);
            }
        }
        for (int i = 1; i < CWQuestManager.MAXQUEST; i++)
        {
            QUESTDATA qData = CWQuestManager.Instance.GetQuestByID(i);
            if (qData == null) continue;
            if (qData.m_nCount < qData.m_nMaxCount)
            {
                AddData(qData);
            }
        }
        for (int i = 1; i < CWQuestManager.MAXQUEST; i++)
        {
            QUESTDATA qData = CWQuestManager.Instance.GetQuestByID(i);
            if (qData == null) continue;
            if (qData.m_nCount == CWQuestManager.MAXCOUNT) 
            {
                AddData(qData);
            }
        }



        m_gScrollList.GetListCount = GetListCount;
        m_gScrollList.GetListColumnCount = GetListColumnCount;
        m_gScrollList.GetListValue = GetListValue;
        m_gScrollList.GetListColumn = GetListColumn;
        m_gScrollList.m_Type = ScrollListUI.TYPE.LIST;
        m_gScrollList.m_bStartClick = false;


    }

    #endregion
    protected override void _Open()
    {
        OnQuest();
        base._Open();
    }

   
}
