using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

using System.Linq;
using SimpleJSON;

using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

using CWUnityLib;

public class Make_AI : MonoBehaviour {


    //public Transform m_tAiDir;
    //public string m_szFile= "AIData";

    [System.Serializable]
    public struct AIDATA
    {
        public Transform m_tAiDir;
        public string m_szFile ;

    }
    public AIDATA[] m_kAidata;


    JObject m_jData;

    CWJSon m_kJSon=new CWJSon();


    void MakeDir( Transform tt,JObject jParent)
    {
        JObject jj = new JObject();
        jParent.Add(tt.name, jj);

        

        for (int i=0;i<tt.childCount;i++)
        {
            Transform tChild = tt.GetChild(i);
            MakeDir(tChild, jj);
        }
    }
    public void Make(Transform tt,string szname)
    {
        m_jData = new JObject();

        MakeDir(tt, m_jData);
        m_kJSon = new CWJSon(m_jData);
        m_kJSon.SaveFile(szname);
        Debug.Log(m_jData.ToString());

    }



    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"make"))
        {
            foreach(var v in m_kAidata)
            {
                if (v.m_tAiDir == null) continue;
                Make(v.m_tAiDir,v.m_szFile);
            }
            
        }
      
    }

}
