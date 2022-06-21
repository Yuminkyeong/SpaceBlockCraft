using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
public class ChangeNameUI : MonoBehaviour
{

    public string Change(string szText)
    {
        if (CWHero.Instance == null) return "";
        return CWLib.ChangeString(szText, "@User", CWHero.Instance.name);
    }
}
