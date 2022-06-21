using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroMissileDetect : MonoBehaviour
{
    public Action cbDetect;
    private void OnTriggerEnter(Collider other)
    {
        if (CWHero.Instance == null) return;
        if (other.tag != "Hero") return;
        if (cbDetect != null) cbDetect();
    }
}
