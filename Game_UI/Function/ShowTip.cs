
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowTip : MonoBehaviour
{
    public string m_szText;
    private void OnEnable()
    {
        if (ShowHelp.g_bActive) return;
        if (TipMessageDlg.Instance == null) return;
        TipMessageDlg.Instance.Show(m_szText);
    }
}
