using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CWDemageManager : CWManager<CWDemageManager>
{

    const int _MaxCount = 3;
    public GameObject m_gDamageInfo;
    public GameObject m_gHPTextInfo;
    public GameObject m_gNumTextInfo;

    
    private void Start()
    {
    }
    void GetObject(Transform pParent, GameObject gPrefab,  string szValues, Color kcolor,   float fLifetime = 0)
    {
        GameObject gg;
        gg = Instantiate(gPrefab);
        
        gg.transform.SetParent(pParent);
        gg.transform.localPosition = Vector3.zero;
        gg.transform.localScale = Vector3.one;
        gg.transform.localRotation = new Quaternion();
        
        Text tPro = gg.GetComponentInChildren<Text>();
        tPro.text = szValues;
        if(kcolor!=Color.clear)
            tPro.color = kcolor;
        UITweener[] tt = gg.GetComponentsInChildren<UITweener>();
        foreach (var v in tt)
        {
            v.ResetToBeginning();
            v.PlayForward();
        }
        Destroy(gg, 2f);
    }

    public void ShowDamage(Transform tParent, string szValues,  float fLifetime = 0)
    {
        if (szValues == null) return;
        if (szValues.Length < 1) return;
        GetObject(tParent, m_gDamageInfo, szValues, Color.clear, fLifetime);
    }
    public void ShowHpText(string szValues,Color kcolor, float fLifetime = 0)
    {
        if (szValues == null) return;
        if (szValues.Length < 1) return;
        GetObject(Game_App.Instance.m_gUIDir.transform, m_gHPTextInfo, szValues, kcolor,  fLifetime);
    }
    public void ShowNumText(Transform tParent, string szValues, float fLifetime = 0)
    {
        if (szValues == null) return;
        if (szValues.Length < 1) return;
        GetObject(tParent, m_gNumTextInfo, szValues, Color.clear, fLifetime);
    }


}
