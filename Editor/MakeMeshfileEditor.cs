using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MakeMeshFile))]

public class MakeMeshfileEditor : Editor
{
    string szfile="";
    GUIContent guiContentLabel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MakeMeshFile kObject = (MakeMeshFile)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 메쉬어셋만들기 --");
        EditorGUILayout.Space();

        
        guiContentLabel = new GUIContent("몹이름");
        szfile = EditorGUILayout.TextField(guiContentLabel, szfile);

        if (GUILayout.Button("만들기"))
        {
            kObject.MakeMesh(szfile);
        }

    }



}
