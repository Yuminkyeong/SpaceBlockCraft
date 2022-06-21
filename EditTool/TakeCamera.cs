using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TakeCamera : MonoBehaviour
{
    public void TakeCam()
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

}
