using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
using CWEnum;
public class CWDebugManager :  CWManager<CWDebugManager>
{

    public bool NotTuto = false; // 투토 안함
    public bool m_bMusuk = false;
    public bool m_bDontAttack = false;

    public int m_nGold = 0;
    public int m_nCash=0;
    

    public bool m_bDebugText = false;
    public Text [] m_kText;
    public Text m_kDebugText;

    public void Print(string sz)
    {
        if (m_kDebugText == null) return;
        m_kDebugText.text = sz;
    }

    List<string> m_List= new List<string>();

    public override void Create()
    {
        
        base.Create();
//        foreach (var v in m_kText) v.text = "";
#if UNITY_EDITOR

        StartCoroutine("IRun");
#endif

    }
    public void Log(string sz)
    {
     
        Debug.Log(sz);

        if (m_kText == null) return;
        if (m_bDebugText)
        {
            m_List.Add(sz);
            if (m_List.Count >= m_kText.Length)
            {
                m_List.RemoveAt(0);
            }
            for (int i = 0; i < m_List.Count; i++)
            {
                if (m_kText[i] == null) break;
                m_kText[i].text = m_List[i];
            }
        }

        
    }
    public void LogText(int num,string sz)
    {
        if (m_kText == null) return;
        if (num >= m_kText.Length) return;
        m_kText[num].text = sz;
    }

    IEnumerator IRun()
    {
        yield return new WaitForSeconds(1f);
        while (true)
        {
            if(m_nGold>0)
            {
                CWSocketManager.Instance.UseCoinEx(COIN.GOLD, m_nGold);
                m_nGold = 0;
            }
            if (m_nCash > 0)
            {
                CWSocketManager.Instance.UseCoinEx(COIN.GEM, m_nCash);
                m_nCash = 0;
            }

            yield return new WaitForSeconds(3f);
        }
    }
}
