using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TakeCamera))]

public class TakeCameraEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TakeCamera kObject = (TakeCamera)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 카메라 위치 가져오기 --");
        EditorGUILayout.Space();



        if (GUILayout.Button("획득"))
        {
            kObject.TakeCam();
        }

    }

}
