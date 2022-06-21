using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWUnityLib;
using CWStruct;
using CWEnum;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Linq;


public class MakeUserTool : CWManager<MakeUserTool>
{
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tChild = transform.GetChild(i);
            if (tChild)
            {
                if (tChild.gameObject.activeSelf)
                    tChild.SendMessage("Create");
            }
        }

        StartCoroutine("IRun");
    }

    IEnumerator IRun()
    {
            yield return null;
        bool bflag = false;
        // 유저 데이타를 가져온다 
        CWSocketManager.Instance.SendAskMakeruser((jData)=> {
            bflag = true;





        });
        while (!bflag)
        {
            yield return null;
        }


        
    }
}
