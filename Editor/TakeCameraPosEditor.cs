using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(MakeCameraTool))]

public class TakeCameraPosEditor : Editor
{
    
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MakeCameraTool kObject = (MakeCameraTool)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 카메라 위치 가져오기 --");
        EditorGUILayout.Space();

    

        if (GUILayout.Button("획득"))
        {
            kObject.MakeDir();
        }

    }

}
