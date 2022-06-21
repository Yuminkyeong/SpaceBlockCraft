using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JoyTest : MonoBehaviour
{

    public float m_fRadius = 100;
    public Text m_kText;

    public Transform m_kBall1;
    public Transform m_kBall2;
         

    void Start()
    {
        
    }
    private void OnEnable()
    {

    }
    
    // Update is called once per frame
    void Update()
    {

            


        float v1 =  Input.GetAxis("Horizontal");
        float v2=  Input.GetAxis("Vertical");
        float v3 =  Input.GetAxis("HSCreen");
        float v4=  Input.GetAxis("VSCreen");

        float v5 = Input.GetAxis("LT");

        m_kBall1.localPosition = new Vector3(v1*m_fRadius,v2*m_fRadius);
        m_kBall2.localPosition = new Vector3(v3*m_fRadius,v4*m_fRadius);


        m_kText.text = string.Format(" {0} ,{1} ,{2},{3} {4}", v1,v2,v3,v4, v5);

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("Fire");
        }

    }
}
