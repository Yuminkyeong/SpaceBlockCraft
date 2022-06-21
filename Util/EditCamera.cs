using UnityEngine;
using System.Collections;

public class EditCamera : MonoBehaviour {

    
	void Start () {
	
	}
    void SetEditCamera()
    {
#if UNITY_EDITOR
        ArrayList sceneViews;
        sceneViews = UnityEditor.SceneView.sceneViews;
        if (sceneViews.Count == 0) return;
        UnityEditor.SceneView sceneView = (UnityEditor.SceneView)sceneViews[0];
        

        Camera.main.transform.position = sceneView.camera.transform.position;
        Camera.main.transform.rotation = sceneView.camera.transform.rotation;
#endif
    }
	
	// Update is called once per frame
	void Update () 
    {
        SetEditCamera();
	}
}
