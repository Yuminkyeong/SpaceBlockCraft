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

public class ADShowDlg : WindowUI<ADShowDlg>
{



    public void Show(CBClose cbClose)
    {
        CloseFuction = cbClose;
        Open();
    }
}
