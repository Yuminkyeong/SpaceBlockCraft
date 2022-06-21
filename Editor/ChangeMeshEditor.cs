using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ChangeMeshValue))]

public class ChangeMeshEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ChangeMeshValue m_kTarget = (ChangeMeshValue)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("");
        EditorGUILayout.Space();


        if (GUILayout.Button("메쉬세팅"))
        {
            m_kTarget.ChangeMesh();
        }


    }

}
