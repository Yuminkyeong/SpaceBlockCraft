using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class CWDamagEffect : MonoBehaviour {

    // Use this for initialization
    public void Begin( string szValues)
    {

        Text tPro = gameObject.GetComponentInChildren<Text>();
        tPro.text = szValues;
        UITweener[] tt = gameObject.GetComponentsInChildren<UITweener>();
        foreach(var  v in tt)
        {
            v.ResetToBeginning();
            v.PlayForward();
        }

    }

    


}
