using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWStruct;
using CWEnum;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class FuncReset : WindowUI<FuncReset>
{
    public InputField m_kInput;

    public void OnResetFunction()
    {
        //		fucname	"./DB/GameConfig.js"	
        string szpath = string.Format("./file/{0}", m_kInput.text);
        CWSocketManager.Instance.SendResetFunction(szpath);
        Close();
    }


}
