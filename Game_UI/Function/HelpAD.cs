using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelpAD : MonoBehaviour
{
    public void OnOK()
    {
        HelpPackageDlg.Instance.Open();
    }

}
