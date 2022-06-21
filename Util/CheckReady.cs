using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class CheckReady : MonoBehaviour {

	// Use this for initialization
	void Start () {
		

	}
    private void Awake()
    {
        if (CWHeroManager.Instance == null)
        {
            SceneManager.LoadSceneAsync(0);
        }

    }

    // Update is called once per frame
    void Update () {
		
	}
}
