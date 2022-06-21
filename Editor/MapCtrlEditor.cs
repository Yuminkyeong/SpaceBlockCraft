using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MapCtrl))]

public class MapCtrlEditor : Editor
{
    GUIContent guiContentLabel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MapCtrl m_kTarget = (MapCtrl)target;


        m_kTarget.m_nRange = EditorGUILayout.IntSlider("범위(1~32)", m_kTarget.m_nRange,1, 64);





        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 파일번호--");
        EditorGUILayout.Space();
        guiContentLabel = new GUIContent("파일 이름");
        if (m_kTarget.m_nFile == 0) m_kTarget.m_nFile = 1000;
        m_kTarget.m_nFile = EditorGUILayout.IntField(guiContentLabel, m_kTarget.m_nFile);

        if (GUILayout.Button("불러오기"))
        {
            m_kTarget.LoadMap();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //if (GUILayout.Button("오브젝트생성"))
        //{
        //    m_kTarget.CreateObject();
        //}

        //EditorGUILayout.Space();
        //EditorGUILayout.Space();
        //if (GUILayout.Button("테스트"))
        //{
        //    m_kTarget.Test();
        //}

    }

}
