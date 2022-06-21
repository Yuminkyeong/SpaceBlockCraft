using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeText))]
public class ChangeTextEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChangeText m_kTarget = (ChangeText)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 텍스트 변경 --");
        EditorGUILayout.Space();

        //guiContentLabel = new GUIContent("파일 이름");
        //m_szFile = EditorGUILayout.TextField(guiContentLabel, m_szFile);

        if (GUILayout.Button("텍스트 폰트 새로고침"))
        {
            m_kTarget.ChangeFont();
        }

        EditorGUILayout.Space();

        if (GUILayout.Button("Text 를 CWText 로변경"))
        {
            m_kTarget.ChangeDir();
        }
        EditorGUILayout.Space();
        if (GUILayout.Button("ShowHideUI false작업"))
        {
            m_kTarget.ShowHideWork();
        }

        //if (GUILayout.Button("맑은고딕으로 변경"))
        //{
        //    m_kTarget.ChangeFont();
        //}

    }
}
