using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCameraFollow : MonoBehaviour
{

    public Transform m_tTarget;
    public Camera m_Cam;

    void Start()
    {

    }
    void SetEditCamera()
    {
        if (m_tTarget == null) return;
        if (m_Cam == null) return;
#if UNITY_EDITOR
        ArrayList sceneViews;
        sceneViews = UnityEditor.SceneView.sceneViews;
        if (sceneViews.Count == 0) return;
        UnityEditor.SceneView sceneView = (UnityEditor.SceneView)sceneViews[0];

        if(m_tTarget)
        {
            m_tTarget.position = sceneView.camera.transform.position;
            m_tTarget.rotation = sceneView.camera.transform.rotation;

        }
        Camera cc = m_Cam;
        cc.transform.position = sceneView.camera.transform.position;
        cc.transform.rotation = sceneView.camera.transform.rotation;

#endif
    }

    // Update is called once per frame
    void Update()
    {

        SetEditCamera();
    }
}
