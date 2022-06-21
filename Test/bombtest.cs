using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class bombtest : MonoBehaviour {


    public int m_nCount = 1000;
    public GameObject m_gObject;
	void Start () {

        int x = 0;
        int y = 0;
        int z = 0;
        for (int i=0;i<m_nCount;i++)
        {
            CWSphereData.GetData(i,ref x,ref y,ref z);
            GameObject gg = Instantiate(m_gObject);
            gg.transform.localPosition = new Vector3(x, y, z);
        }
        

    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
