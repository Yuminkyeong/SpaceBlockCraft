using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DictionarySlot : SlotItemUI
{

    public GameObject m_gYellow;// 보상 받을 수 있는 아이템이 있는가ㅣ
    public GameObject m_gLock;// 한번도 획득하지 못한 아이템

    public override bool UpdateData()
    {
         base.UpdateData();
        int nItem = m_nItemID;
        if(CWHeroManager.Instance.IsTakeItem(nItem))
        {
            m_gLock.SetActive(false);
        }
        else
        {
            m_gLock.SetActive(true);
        }
        if(CWHeroManager.Instance.IsRewardItem(nItem))
        {
            m_gYellow.SetActive(true);
        }
        else
        {
            m_gYellow.SetActive(false);
        }

        return true;
    }
}
