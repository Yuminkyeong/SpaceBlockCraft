using UnityEngine;
using UnityEngine.UI;
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

using CWStruct;
using CWUnityLib;
using CWEnum;

public class SockTest : MonoBehaviour
{
    public Text m_kText;
    public InputField m_kInput;

    public string m_szstr = "";
    void Start()
    {
        m_szstr = "";
        m_kText.text = "";
        List<string> klist = new List<string>();
        klist.Add("123.123.1.21");
        klist.Add("localhost");
        
    }
    void Update()
    {
        
    }
    public void SendData(string str)
    {

        
    }

    public void OnSendData()
    {
        SendData(m_kInput.text);
    }


}
