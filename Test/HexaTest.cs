using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using UnityEngine.UI;
public class HexaTest : MonoBehaviour
{


    public Image m_Target;

    public int hexaDx = 5;
    public Vector2 vSize;

    void Start()
    {

        RectTransform rt= m_Target.GetComponent<RectTransform>();
        List<Vector2> kList= CWMath.MakeHedron(0, 0,(int)vSize.x, (int)vSize.y, hexaDx);

        foreach(var v in kList)
        {
            Image tt= Instantiate(m_Target,transform);
            tt.transform.localPosition = v;
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
