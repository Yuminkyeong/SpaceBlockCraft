using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDestroy : MonoBehaviour
{
    public float time = 2.0f;


    private void OnEnable()
    {
        if (!CWGlobal.G_bGameStart) return;
        Destroy(gameObject, time);
    }
}
