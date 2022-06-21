using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class DamageText : MonoBehaviour
{

    public void Begin(string szText,Color kcolor)
    {

        Text tPro = gameObject.GetComponentInChildren<Text>();
        tPro.text = szText;
        tPro.color = kcolor;
        UITweener[] tt = gameObject.GetComponentsInChildren<UITweener>();
        foreach (var v in tt)
        {
            v.ResetToBeginning();
            v.PlayForward();
        }


    }
}
