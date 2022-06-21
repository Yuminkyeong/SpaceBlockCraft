using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class TransMoney : MonoBehaviour {

    string ConvertString(string szText)
    {
        return CWLib.ConvertMoney(szText);
    }
    private void OnEnable()
    {
        CWText kLabel = GetComponentInChildren<CWText>();
        if (kLabel != null)
        {
            kLabel.text = CWLib.ConvertMoney(kLabel.text);
        }
    }

}
