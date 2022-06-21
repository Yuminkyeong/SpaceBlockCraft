using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using DG.Tweening;

public class PathTest : MonoBehaviour
{

    public CWAirObject m_kAir;
    public DOTweenPath m_kPath;
    public CWPVPAI m_kPvpAI;

    private void OnGUI()
    {
        //
        if (GUI.Button(new Rect(0, 0, 100, 100), "Click"))
        {
            m_kAir.Load("NPC_S_1_1");
            //m_kPvpAI.Begin();
        }
    }

}
