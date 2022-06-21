using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransText : MonoBehaviour
{
    
    int m_nNumber = 0;


    private void Awake()
    {
        
        

    }
    private void OnEnable()
    {
        UpdateTrans();
    }
    public void UpdateTrans()
    {
        if (CWLocalization.Instance == null) return;
        if (CWLocalization.Instance.GetDataCount() == 0) return;

        Text tt = GetComponent<Text>();
        m_nNumber = CWLocalization.Instance.GetLanguageNumber(tt.text);
        if (m_nNumber == 0) return;

        tt.text = CWLocalization.Instance.GetLanguage(m_nNumber); 

    }
}
