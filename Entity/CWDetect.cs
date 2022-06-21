using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;

public class CWDetect : MonoBehaviour {

    public string m_szDetectTag;

    

  
    private void OnTriggerEnter(Collider other)
    {
        if (CWHero.Instance == null) return;
        if (!GamePlay.Instance.IsGamePlay()) return;
        if (GamePlay.Instance.CharMode) return;

        if (other.tag == m_szDetectTag)
        {
            ObejctAction[] OAction = gameObject.GetComponentsInChildren<ObejctAction>();//GetComponent<ObejctAction>();
            foreach(var v in OAction)
            {
                v.DetectBegin();
            }

            

            CWPoolManager.Instance.GetParticle(CWHero.Instance.GetDetectPos(), "smogBlast", 2f);
            CWVibration.Vibrate(300);

            return;
            //Debug.Log("Trigger start");
            //if (m_bDetectflag)
            //{
            //    return;
            //}
            //StartCoroutine("IRun");
            //// 1초에 한번만 
            //CWHero hero = CWHero.Instance;
            //if (hero == null) return;
            //if (hero.GetHpRate() > 0)
            //{
            //    hero.OnMapDetect();
            //    if (NoticeMessage.Instance)
            //        NoticeMessage.Instance.Show(string.Format("블록과 충돌하였습니다!"));


            //}
            //int ncnt = CWHero.Instance.NBlockCount; //CWHero.Instance.NBlockCount / 20 + 1;
            //CWMapManager.SelectMap.Hit(true, CWHero.Instance.GetDetectPos(), ncnt, false);
            //CWPoolManager.Instance.GetParticle(CWHero.Instance.GetDetectPos(), "smogBlast", 2f);
            //CWVibration.Vibrate(300);
        }



    }

    private void OnTriggerExit(Collider other)
    {

        ObejctAction[] OAction = gameObject.GetComponentsInChildren<ObejctAction>();//GetComponent<ObejctAction>();
        foreach (var v in OAction)
        {
            v.DetectExit();
        }


    }
    private void OnTriggerStay(Collider other)
    {
        //ObejctAction OAction = GetComponent<ObejctAction>();
        //if (OAction == null) return;
        //OAction.DetectStay();
        //ObejctAction[] OAction = gameObject.GetComponentsInChildren<ObejctAction>();//GetComponent<ObejctAction>();
        //foreach (var v in OAction)
        //{
        //    v.DetectExit();
        //}

    }

}
