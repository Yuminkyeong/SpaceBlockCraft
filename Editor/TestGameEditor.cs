using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(TestGame))]
public class TestGameEditor : Editor
{
    GUIContent guiContentLabel;
    GUIContent guiContentLabel2;
    string m_szFile= "Hero_Story";
    string m_szNodefile= "";
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        TestGame kObject = (TestGame)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 게임 테스트 --");
        EditorGUILayout.Space();

        guiContentLabel = new GUIContent("파일 이름");
        m_szFile = EditorGUILayout.TextField(guiContentLabel, m_szFile);

        

        if (GUILayout.Button("비행기 교체 변경"))
        {
            kObject.ChagneFile(m_szFile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        guiContentLabel2 = new GUIContent("노드파일 재설정");
        m_szNodefile = EditorGUILayout.TextField(guiContentLabel2, m_szNodefile);
        if (GUILayout.Button("재설정"))
        {
            kObject.RefreshServerFile(m_szNodefile);
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //float ftime = EditorGUILayout.Slider("타임스케일", Time.timeScale, 0,1);
        
        //if (GUILayout.Button("타임스케일 적용"))
        //{
        //    Time.timeScale = ftime;
        //}




    }
}
