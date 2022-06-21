using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectEditChang : MonoBehaviour
{
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
        
    }
    private void OnTriggerStay(Collider other)
    {
        Renderer mf = GetComponent<Renderer>();
        Color kcolor = mf.material.color;
        kcolor.a = 0;
        mf.material.color = kcolor;


    }
    private void OnTriggerExit(Collider other)
    {
        Renderer mf = GetComponent<Renderer>();
        Color kcolor = mf.material.color;
        kcolor.a =1;
        mf.material.color = kcolor;


    }

}
