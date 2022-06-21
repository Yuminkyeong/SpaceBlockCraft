using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MakeMob))]

public class MakeMobEditor : Editor
{
    
    GUIContent guiContentLabel;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MakeMob m_kTarget = (MakeMob)target;

        EditorGUILayout.LabelField("-- 몹--");
        EditorGUILayout.Space();
        guiContentLabel = new GUIContent("몹이름");
        if(m_kTarget.m_kMapTool)
        {
            m_kTarget.m_kMapTool.m_szTurret = EditorGUILayout.TextField(guiContentLabel, m_kTarget.m_kMapTool.m_szTurret);
        }

        guiContentLabel = new GUIContent("터렛");
        if (m_kTarget.m_kMapTool)
        {
            m_kTarget.m_kMapTool.m_bTurret = EditorGUILayout.Toggle(guiContentLabel, m_kTarget.m_kMapTool.m_bTurret);
        }

    }
}
