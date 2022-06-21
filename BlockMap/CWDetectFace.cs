using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDetectFace : MonoBehaviour
{


    public int m_nFace;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Hero")
        {
           // CWMapManager.SelectFace(m_nFace);
        }

    }
}
