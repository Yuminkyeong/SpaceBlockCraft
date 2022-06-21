using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWAIOwnerManager : CWManager<CWAIOwnerManager>
{
    #region AI오너

    Dictionary<int, int> m_kOwnerList = new Dictionary<int, int>();

    public int GetOwner(int nAiID)
    {
        if (m_kOwnerList.ContainsKey(nAiID)) return m_kOwnerList[nAiID];

        return 0;
    }

    public void AddOwnerList(int nAiID, int nOwner)
    {
        if (m_kOwnerList.ContainsKey(nAiID)) return;

        m_kOwnerList.Add(nAiID, nOwner);
    }

    public void SetMyOwner(int nID)
    {
        if (m_kOwnerList.ContainsKey(nID))
        {
            m_kOwnerList[nID] = CWHero.Instance.m_nID;
            return;
        }

        m_kOwnerList.Add(nID, CWHero.Instance.m_nID);
    }


    public void RemoveOwner(int nAiNumber)
    {
        if (m_kOwnerList.ContainsKey(nAiNumber))
        {
            m_kOwnerList.Remove(nAiNumber);
        }
    }
    public bool IsEmpty(int nAiNumber)
    {
        if (m_kOwnerList.ContainsKey(nAiNumber)) return false;
        return true;

    }


    public bool IsMyOwner(int nAiNumber)
    {
        if (!m_kOwnerList.ContainsKey(nAiNumber)) return false;
        if (m_kOwnerList[nAiNumber] == CWHeroManager.Instance.GetIDX()) return true;// 주인이 나라면 AI 활동
        return false;
    }
    public void DeleteOWnerID(int nID)
    {
        List<int> ktemp = new List<int>();
        foreach (var v in m_kOwnerList)
        {
            if (v.Value == nID)
            {
                ktemp.Add(v.Key);
            }
        }
        foreach (var v in ktemp)
        {
            m_kOwnerList.Remove(v);
        }
    }

    public void SendRemoveOwner(int nAiNumber)
    {
        CWSocketManager.Instance.SendRemoveOwner(nAiNumber);
        RemoveOwner(nAiNumber);

    }
    public void Clear()
    {

        m_kOwnerList.Clear();
    }
    // 디버깅
    //private void Update()
    //{
    //    int count = 0;
    //    foreach(var v in m_kOwnerList)
    //    {
    //        CWDebugManager.Instance.LogText(count, string.Format("AI:{0} Owner: {1} ",v.Key,v.Value));
    //        count++;
    //    }


    //}


    #endregion
}
