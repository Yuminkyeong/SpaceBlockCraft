using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeCloud))]
public class MakeCloudEditor : Editor
{
    //GUIContent guiContentLabel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MakeCloud m_kTarget = (MakeCloud)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 텍스트 변경 --");
        EditorGUILayout.Space();

        //guiContentLabel = new GUIContent("파일 이름");
        //m_szFile = EditorGUILayout.TextField(guiContentLabel, m_szFile);



        if (GUILayout.Button("구름만들기"))
        {
            m_kTarget.Make();
        }


    }

}
