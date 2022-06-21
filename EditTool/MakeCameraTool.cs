using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeCameraTool : MonoBehaviour
{

    public Transform m_kDir;

  
    public void MakeDir()
    {
#if UNITY_EDITOR
        ArrayList sceneViews;
        sceneViews = UnityEditor.SceneView.sceneViews;
        if (sceneViews.Count == 0) return;
        UnityEditor.SceneView sceneView = (UnityEditor.SceneView)sceneViews[0];

        GameObject gChild = new GameObject();
        int cnt = m_kDir.childCount + 1;
        gChild.name = "Cam_"+ cnt.ToString();
        gChild.transform.parent = m_kDir;
        gChild.transform.position = sceneView.camera.transform.position;
        gChild.transform.rotation = sceneView.camera.transform.rotation;
#endif
    }

}
