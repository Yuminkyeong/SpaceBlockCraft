using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityMove : MonoBehaviour
{
    // 충돌이 일어나면 사라짐
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("IRun");

    }
    bool Exit = true;
    IEnumerator IRun()
    {
        Exit = false;
        CharicAction aa =gameObject.GetComponent<CharicAction>();
        aa.enabled = false;

        Rigidbody rr = gameObject.GetComponent<Rigidbody>();
        rr.isKinematic = false;
        rr.useGravity = true;
             

        while(!Exit)
        {
            

            yield return null;
        }
        aa.enabled = true;
        rr.isKinematic = true;
        rr.useGravity = false;

        Destroy(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Exit = true;
    }

}
