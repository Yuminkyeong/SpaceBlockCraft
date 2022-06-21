using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {

    // 타겟을 주위로 돈다 
    public GameObject m_gTarget;
    public float m_fSpeed = 20f;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        transform.RotateAround(m_gTarget.transform.position, Vector3.up, m_fSpeed * Time.deltaTime);

	}
}
