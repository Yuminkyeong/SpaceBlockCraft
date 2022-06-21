using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Story))]
public class StoryEditor : Editor
{
    GUIContent guiContentLabel;
    int num = 0;
    public override void OnInspectorGUI()
    {
        Story m_kTarget = (Story)target;
        if (GUILayout.Button("정렬"))
        {
            m_kTarget.ReSort();
        }
        if (GUILayout.Button("번호넣기"))
        {
            m_kTarget.OrderNumber();
        }
        EditorGUILayout.Space();

        guiContentLabel = new GUIContent("번호뒤에");
        num = EditorGUILayout.IntField(guiContentLabel, num);
        if (GUILayout.Button("추가하기"))
        {
            m_kTarget.OrderNumber();
        }

        base.OnInspectorGUI();

    }
}
