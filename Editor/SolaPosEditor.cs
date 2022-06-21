using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SolaPos))]
public class SolaPosEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SolaPos m_kTarget = (SolaPos)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("--  --");
        EditorGUILayout.Space();

        //GUIContent guiContentLabel = new GUIContent("파일 이름");
        //m_szFile = EditorGUILayout.TextField(guiContentLabel, m_szFile);
        //EditorGUILayout.FloatField(guiContentLabel, f);
        if (GUILayout.Button("적용"))
        {
            m_kTarget.Create();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("현재카메라적용"))
        {
            m_kTarget.ApplayCam();
        }

    }

}
