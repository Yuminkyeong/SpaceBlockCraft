using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pd_WaitRareBlockGet : MonoBehaviour
{
    void Start()
    {
        TutoMissionDlg.Instance.Open();
        StartCoroutine("IRun");
        
    }
    IEnumerator IRun()
    {
        while (true)
        {

            int nItem = CWArrayManager.Instance.GetItemFromBlock(  CWMapManager.SelectMap.m_nResblock3);

            int cnt = EquipInvenList.Instance.GetItemTotalCount(nItem);

            if (cnt >= 3)
            {
                TutoMissionDlg.Instance.Close();
                yield return new WaitForSeconds(2f);
                CWProductionPage pt = GetComponentInParent<CWProductionPage>();
                pt.OnClose();
                break;
            }
            yield return new WaitForSeconds(0.4f);
        }
    }

}
