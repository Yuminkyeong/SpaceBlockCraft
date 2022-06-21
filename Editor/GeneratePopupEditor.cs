using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(GeneratePopup))]
public class GeneratePopupEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        GeneratePopup m_kTarget = (GeneratePopup)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("        -- 팝업창 만들기 --");
        EditorGUILayout.Space();
        if (GUILayout.Button("팝업만들기"))
        {
            m_kTarget.MakePopup();
        }
        
    }
}
