using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
public class GemAD : MonoBehaviour
{
    public void OnOK()
    {
        CWADManager.Instance.RewardShow(() => {
            CWSocketManager.Instance.UseCoinEx(COIN.GEM, 3);

            AnalyticsLog.Print("ADLog", "GemAD");
        });


    }
}
