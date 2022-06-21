using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWWater : MonoBehaviour {


    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

        if (CWHeroManager.Instance == null) return;
        int tx, tz;
        int wx = (int)transform.position.x;
        int wz = (int)transform.position.z;

        wx = (wx / 500) * 500;
        wz = (wz / 500) * 500;

        Vector3 vPos= CWHeroManager.Instance.GetPosition();
        tx = ((int)vPos.x / 500) * 500;
        tz = ((int)vPos.z / 500) * 500;
        if (wx != tx || wz != tz)
        {
            transform.position = new Vector3(tx, transform.position.y, tz);
        }


    }
}
