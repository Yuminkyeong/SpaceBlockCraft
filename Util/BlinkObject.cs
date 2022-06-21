using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlinkObject : MonoBehaviour {


    public float m_fSec;

	void Start () {

        StartCoroutine("Run");
	}
    IEnumerator Run()
    {
        while(true)
        {
            Renderer rr = GetComponent<Renderer>();
            if(rr!=null)
            {
                rr.enabled = !rr.enabled;
            }
            yield return new WaitForSeconds(m_fSec);
        }
    }
}
