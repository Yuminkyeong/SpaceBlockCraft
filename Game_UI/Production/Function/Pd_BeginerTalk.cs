using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pd_BeginerTalk : MonoBehaviour
{
    public string[] TALKS;



    static int m_nCount = 0;
    public void ChangeText()
    {
        int Count = m_nCount %= TALKS.Length;
        m_nCount++;
        CWProductionPage pt = GetComponentInParent<CWProductionPage>();
        pt.m_szTalk = TALKS[Count];

    }
}
