using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ParticleEdit))]
public class ParticleEditorMenu : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        ParticleEdit scaler = (ParticleEdit)target;

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("-- 파티클 스케일 변경 툴 --");
        EditorGUILayout.Space();
        EditorGUILayout.LabelField("ScalingMode가 Hierarchy로 되어있어야 Scale 변경이 원활하게 됩니다.");
        EditorGUILayout.LabelField("처음 한번만 실행하면 됩니다.");

        if (GUILayout.Button("Hierarchy로 변경"))
        {
            scaler.ModeChange();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        scaler.scaleFactor = EditorGUILayout.FloatField("파티클 크기", scaler.scaleFactor);
        if (GUILayout.Button("파티클 스케일 변경"))
        {
            
            scaler.ParticleScaleChange();
        }

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        scaler.RotateFactor = EditorGUILayout.FloatField("파티클 속도크기변화", scaler.scaleFactor);
        if (GUILayout.Button("파티클 속도크기변화 "))
        {


            scaler.ParticleSpeedChange();
        }



        EditorGUILayout.Space();
        EditorGUILayout.Space();
        scaler.RotateFactor = EditorGUILayout.FloatField("파티클 회전속도", scaler.RotateFactor);
        if (GUILayout.Button("파티클 회전 속도 "))
        {
            

            scaler.ParticleRotateSpeed();
        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        scaler.SimulFactor = EditorGUILayout.FloatField("파티클 시뮬속도", scaler.SimulFactor);
        if (GUILayout.Button("파티클 시뮬속도 "))
        {


            scaler.ParticleSimulSpeed();
        }


    }
}
