using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDontDestroyOnLoad : MonoBehaviour {

	// Use this for initialization
	void Start () {

        DontDestroyOnLoad(this);
    }
	
}
