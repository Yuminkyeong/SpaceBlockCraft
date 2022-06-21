using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    void _FontChange(Transform tChild)
    {
        Text[] kTexts = tChild.GetComponentsInChildren<Text>(true);
        foreach(var v in kTexts)
        {
            v.font = Resources.Load<Font>("Fonts/Maplestory Bold"); // 나눈고딕 일반적으로 
        }

        CWText[] kCWTexts = tChild.GetComponentsInChildren<CWText>(true);
        foreach (var v in kCWTexts)
        {
            v.font = Resources.Load<Font>("Fonts/Maplestory Bold"); // 나눈고딕 일반적으로 
        }
       



    }
    void _NodeChange(Transform tChild)
    {
        Text kText = tChild.GetComponent<Text>();
        if (kText == null) return;


        string szText = kText.text;
        Color color = kText.color;
        TextAnchor alignment = kText.alignment;

        DestroyImmediate(kText);
        CWText Text2 = tChild.gameObject.AddComponent<CWText>();



        Text2.text = szText;
        Text2.color = color;
        Text2.alignment = alignment;

        TransText tt1 = tChild.GetComponent<TransText>();
        if (tt1 != null)
        {
            Text2.m_kTextType = CWText.TEXTTYPE.TRANSTEXT;

            DestroyImmediate(tt1);
        }
        TransNumber tt2 = tChild.GetComponent<TransNumber>();
        if (tt2 != null)
        {
            Text2.m_kTextType = CWText.TEXTTYPE.NUMBER;
            Text2.m_kFontType = CWText.FONTTYPE.NUMBER;
            DestroyImmediate(tt2);
        }

        Text2.resizeTextForBestFit = true;
        Text2.raycastTarget = false;

        if (Text2.m_kFontType == CWText.FONTTYPE.NORMAL)
        {
            Text2.font = Resources.Load<Font>("Fonts/CookieRun Bold"); // 나눈고딕 일반적으로 
        }
        if (Text2.m_kFontType == CWText.FONTTYPE.NUMBER)
        {
            Text2.font = Resources.Load<Font>("Fonts/DS-DIGI");// 숫자 
        }
        if (Text2.m_kFontType == CWText.FONTTYPE.HIGHWAY)// 전광판
        {
            Text2.font = Resources.Load<Font>("Fonts/Electronic Highway Sign");
        }


    }


    
    void _ChangeDir(Transform tParent)
    {
        _NodeChange(tParent);
        for (int i = 0; i < tParent.childCount; i++)
        {
            Transform tChild = tParent.GetChild(i);
            _ChangeDir(tChild);

        }

    }
    void _ChangeFont(Transform tParent)
    {
        _FontChange(tParent);

    }
    public void RefreshDir()
    {
        _ChangeDir(transform);
    }

    public void ChangeDir()
    {
        _ChangeDir(transform);
    }
    public void ChangeFont()
    {
        _ChangeFont(transform);
    }
    public void ShowHideWork()
    {
        CWShowUI[] ss = GetComponentsInChildren<CWShowUI>(true);

        foreach(var v in ss)
        {
            v.AllFalse();
        }

        ShowHelp[] hh = GetComponentsInChildren<ShowHelp>(true);
        foreach (var v in hh)
        {
            v.AllFalse();
        }

        
    }
}
