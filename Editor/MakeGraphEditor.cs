using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeGraph))]

public class MakeGraphEditor : Editor
{
    GUIContent guiContentLabel;
    string m_szFile = "Graph_1";
    int m_nRange = 100;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MakeGraph kObject = (MakeGraph)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 그래프만들기  --");
        EditorGUILayout.Space();


        guiContentLabel = new GUIContent("범위");
        m_nRange = EditorGUILayout.IntField(guiContentLabel, m_nRange);
        


        if (GUILayout.Button("적용"))
        {
            kObject.Making(m_nRange);
        }
        


    }


}
