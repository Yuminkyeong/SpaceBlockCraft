using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FowardView : MonoBehaviour
{
    public float Dist = 100f;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 forward = transform.TransformDirection(Vector3.forward) * Dist;
        Debug.DrawRay(transform.position, forward, Color.green);


        Vector3 forward2 = transform.forward*Dist;

        Debug.DrawRay(transform.position, forward2, Color.red);

    }
}
