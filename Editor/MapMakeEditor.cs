using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapMake))]
public class MapMakeEditor : Editor
{
    GUIContent guiContentLabel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapMake m_kTarget = (MapMake)target;


        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("패턴가져오기"))
        {
            m_kTarget.GetPatternData();
        }

        EditorGUILayout.Space();
        if (GUILayout.Button("맵만들기"))
        {
            m_kTarget.CreateMap();
        }

        //m_kTarget.m_nRange = EditorGUILayout.IntSlider("범위(1~32)", m_kTarget.m_nRange, 1, 64);



        //EditorGUILayout.Space();
        //EditorGUILayout.Space();
        //EditorGUILayout.Space();
        //EditorGUILayout.LabelField("-- 파일번호--");
        //EditorGUILayout.Space();
        //guiContentLabel = new GUIContent("파일 이름");
        //m_kTarget.m_nFile = EditorGUILayout.IntField(guiContentLabel, m_kTarget.m_nFile);


    }

}
