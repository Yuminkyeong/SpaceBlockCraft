using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWLifetime : MonoBehaviour {


	
    void Timefuc()
    {
        gameObject.SetActive(false);
    }
    public void Begin(float fTime)
    {
        gameObject.SetActive(true);
        CancelInvoke("Timefuc");
        Invoke("Timefuc", fTime);
    }

	void Start () {
		
	}
	void Update () {
		
	}
}
