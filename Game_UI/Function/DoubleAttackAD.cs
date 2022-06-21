using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DoubleAttackAD : MonoBehaviour
{
    
    public void OnOK()
    {
        CWADManager.Instance.RewardShow(() =>
        {
            CWGlobal.g_bDamageDouble = true;
            ATKUPDlg.Instance.Open();
           // NoticeMessage.Instance.Show("공격력이 2배로 업그레이드 되었습니다!");
            AnalyticsLog.Print("ADLog", "DoubleAttackAD");
        });
        
    }
}
