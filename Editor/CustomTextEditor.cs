using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.UIElements;

[CustomEditor(typeof(CWText))]
[CanEditMultipleObjects]
public class CustomTextEditor : UnityEditor.UI.TextEditor
{

    public override VisualElement CreateInspectorGUI()
    {
        CWText m_kTarget = (CWText)target;
    //    m_kTarget.resizeTextForBestFit = true;
        m_kTarget.raycastTarget = false;
        return base.CreateInspectorGUI();
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        CWText m_kTarget = (CWText)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("텍스트 변환 방법");
        m_kTarget.m_kTextType =(CWText.TEXTTYPE) EditorGUILayout.EnumPopup(m_kTarget.m_kTextType, GUILayout.Width(200f));



        EditorGUILayout.Space();
        EditorGUILayout.LabelField("폰트 변환 방법");
        m_kTarget.m_kFontType = (CWText.FONTTYPE)EditorGUILayout.EnumPopup(m_kTarget.m_kFontType, GUILayout.Width(200f));


        if (m_kTarget.m_kFontType == CWText.FONTTYPE.NORMAL)
        {
            m_kTarget.font = Resources.Load<Font>("Fonts/Maplestory Bold"); // 나눈고딕 일반적으로 Maplestory Bold
        }
        if (m_kTarget.m_kFontType == CWText.FONTTYPE.NUMBER)
        {
            m_kTarget.font = Resources.Load<Font>("Fonts/DS-DIGI");// 숫자 
        }
        if (m_kTarget.m_kFontType == CWText.FONTTYPE.HIGHWAY)// 전광판
        {
            m_kTarget.font = Resources.Load<Font>("Fonts/Electronic Highway Sign");
        }
        if (m_kTarget.m_kFontType == CWText.FONTTYPE.test_mk)// 테스트
        {
            m_kTarget.font = Resources.Load<Font>("Fonts/KoPubWorld Dotum Bold");
        }




    }
}
