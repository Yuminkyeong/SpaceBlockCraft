using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCameraLink : MonoBehaviour
{

    // 에디터 카메라와 좌표를 동일하게 한다
    void SetEditCamera()
    {
#if UNITY_EDITOR
        ArrayList sceneViews;
        sceneViews = UnityEditor.SceneView.sceneViews;
        if (sceneViews.Count == 0) return;
        UnityEditor.SceneView sceneView = (UnityEditor.SceneView)sceneViews[0];


        transform.position = sceneView.camera.transform.position;
        transform.rotation = sceneView.camera.transform.rotation;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        SetEditCamera();
    }

}
