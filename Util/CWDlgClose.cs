using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWDlgClose : MonoBehaviour {


    void OnClick()
    {
        BaseUI cs = gameObject.GetComponentInParent<BaseUI>();
        if(cs!=null)
        {
            cs.Close();
        }
    }
}
