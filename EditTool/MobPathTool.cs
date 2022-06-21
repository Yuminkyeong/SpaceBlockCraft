using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;
using DG.Tweening;

public class MobPathTool : MonoBehaviour
{

    public DOTweenPath m_kPath;
    public CWAirObject m_gDrone;
    
    public float fSpeed = 1f;
    void Start()
    {
        
    }
    void Update()
    {
        
    }
    private void OnGUI()
    {
        //
        if(GUI.Button(new Rect(0,0,100,100),"Click"))
        {

            DOTweenPath kPath = m_kPath;
            
            kPath.duration = kPath.duration/fSpeed;

            CWAirObject dd = Instantiate(m_gDrone);
            DOTweenPath gg = Instantiate(kPath);

            dd.Load("NPC_S_1_1");
            
            PostionLink pp =dd.gameObject.AddComponent<PostionLink>();
            pp.m_LinkPostion = gg.gameObject;
            pp.m_bRotate = true;

            DOTweenPath ph = gg.GetComponent<DOTweenPath>();
            ph.DOPlay();





        }
    }

}
