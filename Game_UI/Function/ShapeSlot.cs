using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShapeSlot : MonoBehaviour
{
    
    public int number;

    public GameObject m_kLock;

    void Start()
    {
    }
    private void OnEnable()
    {
        UpdateData();
    }
    public void UpdateData()
    {
        if (CWHeroManager.Instance == null) return;
        if (number == 0) return;
        if (CWHeroManager.Instance.GetShape(number))
        {
            m_kLock.SetActive(false);
        }
        else
        {
            m_kLock.SetActive(true);
        }

    }
    void _Buy()
    {
        int tcnt = CWTableManager.Instance.GetTableInt("BAC_아이템 - 모양", "Price", number);
        int GemId = CWTableManager.Instance.GetTableInt("BAC_아이템 - 모양", "ItemID", number);
        int icount = CWInvenManager.Instance.GetItemTotalCount(GemId);
        if (icount >= tcnt)
        {
            CWHeroManager.Instance.UpdateShape(number);
            CWInvenManager.Instance.DelItem(GemId, tcnt);
       //     m_kLock.SetActive(false);
        }
        else
        {
            NoticeMessage.Instance.Show("블록이 부족합니다!");
        }


    }
    void _Cancel()
    {

    }

    public void OnShape()
    {
        //MessageBoxDlg.Instance.Show(_Buy, _Cancel, "모양", "모양을 구입하시겠습니까?");
        ShapeShopDlg.Instance.Open();

    }
}
