using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AxisRotate : MonoBehaviour
{

    public Vector3 m_vAxis;
    public float m_fSpeed=10f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(m_vAxis, m_fSpeed*Time.deltaTime);

     
    }
}
