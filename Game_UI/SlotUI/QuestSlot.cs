using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestSlot : SlotItemUI
{

    public override void TypeFunction()
    {
        foreach (var v in m_gGroupType) v.SetActive(false);
        int nState = m_gList.GetInt(m_nNumber, "State");
        m_gGroupType[nState].SetActive(true);
    }
    // 보상
    public void OnReward()
    {
        int nID = m_gList.GetInt(m_nNumber, "ID");


        AcheivementsDlg.Instance.Close();
        CWQuestManager.Instance.RewardQuest(nID,()=> {
            AcheivementsDlg.Instance.Open();
        });


    }
}
