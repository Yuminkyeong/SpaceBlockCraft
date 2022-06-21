using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DailymissionSlot : SlotItemUI
{
    public Image m_kGage;
    public Text m_kText;
    public Text m_kTitle;
    public override bool UpdateData()
    {
        base.UpdateData();

        int Key = m_gList.GetInt(m_nNumber,"key");

        int cnt= Dailymission.Instance.GetCount(Key);
        int mcnt = Dailymission.Instance.GetMaxCount(Key);

        if(mcnt>1)
        {
            string szstr= m_gList.GetString(m_nNumber, "submission_txt");
            m_kTitle.text = string.Format(szstr,mcnt);
        }

        m_nGType = 0;
        if (cnt>=mcnt)
        {
            if(Dailymission.Instance.IsReward(Key))
            {
                m_nGType = 2;
            }
            else
            {
                m_nGType = 1;
            }
        }
        if(mcnt>=1000)
        {
            if (cnt >= 1000)
            {
                m_kText.text = string.Format("{0:0,0}/{1:0,0}", cnt, mcnt);//{0:0,0}
            }
            else
            {
                m_kText.text = string.Format("{0}/{1:0,0}", cnt, mcnt);//{0:0,0}
            }
            
        }
        else
        {
            m_kText.text = string.Format("{0}/{1}", cnt, mcnt);//{0:0,0}
        }
        
        m_kGage.fillAmount = (float)cnt / mcnt;
        TypeFunction();
        return true;
    }
    public void OnReward()
    {
        int Gold = m_gList.GetInt(m_nNumber, "reward_txt");
        int Gem = m_gList.GetInt(m_nNumber, "reward_txt2");
        int Key = m_gList.GetInt(m_nNumber, "key");
        Dailymission.Instance.OnReward(Key, Gold,Gem);
        UpdateData();
    }
}
