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
           // NoticeMessage.Instance.Show("���ݷ��� 2��� ���׷��̵� �Ǿ����ϴ�!");
            AnalyticsLog.Print("ADLog", "DoubleAttackAD");
        });
        
    }
}
