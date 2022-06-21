using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;


public class TestUI : CWSingleton<TestUI>
{
    public BaseUI m_gStartOpen;

    private void Start()
    {
        StartCoroutine("IRun");
    }

IEnumerator IRun()
    {
        yield return new WaitForSeconds(0.1f);
     //   CWTableManager.Instance.ToolLoad();
        yield return new WaitForSeconds(0.1f);

        m_gStartOpen.Open();
        CWGlobal.G_bGameStart = true;
    }

    public void OnPlay()
    {
        
        CWTableManager.Instance.UpdateCSV(() => {
            CWMainGame.Instance.Login();

        });

    }

}
