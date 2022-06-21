using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using CWUnityLib;

public class CharaterTest : MonoBehaviour
{
    public CWCharacter m_gChar;
    public string m_gMessage;

    
    private void Start()
    {
        StartCoroutine("IRun");

    }
    IEnumerator IRun()
    {
        yield return new WaitForSeconds(1f);
        CWGlobal.G_bGameStart = true;
        
    }


    public void OnChat1()
    {
        
       
    }
    public void OnChat2()
    {
        

    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),""))
        {
            OnChat1();
        }
        if (GUI.Button(new Rect(0, 100, 100, 100), ""))
        {
            OnChat2();
        }

    }


}
