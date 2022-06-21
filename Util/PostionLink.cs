using UnityEngine;
using System.Collections;
using CWUnityLib;
public class PostionLink : MonoBehaviour {


    public GameObject m_LinkPostion;
    public string GlobalValue;

    public bool m_bRotate;
	void Start () {

        if(CWLib.IsString(GlobalValue))
        {
            m_LinkPostion = CWGlobal.FindObject(GlobalValue);
        }
    }

    private void FixedUpdate()
    {
        if (m_LinkPostion == null) return;
        //transform.position= m_LinkPostion.transform.position;
        transform.localPosition = m_LinkPostion.transform.position;
        if (m_bRotate)
        {
            transform.rotation = m_LinkPostion.transform.rotation;
        }

    }
    
}
