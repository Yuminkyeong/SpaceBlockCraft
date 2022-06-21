using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDisbale : MonoBehaviour
{

    public float time = 2f;
    void Close()
    {
        gameObject.SetActive(false);
    }
    private void OnEnable()
    {
        Invoke("Close", time);
    }
}
