using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
public class CharDetectGround : MonoBehaviour
{

    private void OnTriggerEnter(Collider other)
    {
        // 낙하 하고 있다면

        if (other.tag == "map")// 블록 충돌 
        {

            

            CWChHero.Instance.m_GroundState = DETECTTYPE.ENTER;
            if (CWChHero.Instance.m_bLandingflag)
            {
                CWChHero.Instance.LandingEffect();
            }

            
            Debug.Log("점프 가능");

            CWDebugManager.Instance.Print("Enter");

        }

    }
    void AllowJump()//점프 허용으로 
    {
        CWChHero.Instance.m_GroundState = DETECTTYPE.ENTER;
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "map")// 블록 충돌 
        {
            CWChHero.Instance.m_GroundState = DETECTTYPE.EXIT;
            Invoke("AllowJump", 2f);
            CWDebugManager.Instance.Print("Exit");
        }
            
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "map")// 블록 충돌 
        {

            CWChHero.Instance.m_GroundState = DETECTTYPE.STAY;
            CWDebugManager.Instance.Print("Stay");
            // CWChHero.Instance.JumpOK();
            //Debug.Log("점프 가능 stay");
        }
            
    }
}
