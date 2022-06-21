using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class Pd_MapRotate : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        CWMapManager.Instance.Rotate();
        

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
