using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class CWPlaySound : MonoBehaviour
{
    public string m_szFile;
    public bool m_bAutoflag = true;// 자동 소리
    private void OnEnable()
    {
    
        if(m_bAutoflag)
        {
            Play();
        }

    }
    public void Play()
    {
        if (CWResourceManager.Instance == null) return;
        RectTransform rt = GetComponent<RectTransform>();
        if (rt == null)
        {
            
            CWResourceManager.Instance.PlaySound(m_szFile, gameObject);
        }
        else
        {
            CWResourceManager.Instance.PlaySound(m_szFile, null);
        }

    }
}
