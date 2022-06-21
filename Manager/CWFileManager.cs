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
public class CWFileManager : CWManager<CWFileManager> 
{
    CWJSon m_kJSonData=new CWJSon();

    public void Reset()
    {
        m_kJSonData.Close();
    }
    public CWJSon GetObject(string szname)
    {
        JObject j = (JObject)m_kJSonData.GetJson(szname);
        if(j==null)
        {
            JObject n = new JObject();
            m_kJSonData.Add(szname,n);
            return new CWJSon(n);
        }
        return new CWJSon(j);
    }
    #region LOAD
    public byte[] OpenReadByte(string szname)
    {
        return m_kJSonData.GetBytes(szname);
    }
    public MemoryStream OpenRead(string szname)
    {
        byte [] bdata= m_kJSonData.GetBytes(szname);
        if (bdata == null) return null;
        return new MemoryStream(bdata);
    }
    public override void Create()
    {
        if (m_kJSonData.LoadGamedata("Gamedata/cwgame") == null) return;
    }

    #endregion LOAD
    #region SAVE
    public void AddFile(string szname, byte [] bData)
    {
        m_kJSonData.Add(szname, bData);
    }
    public void Save()
    {
        m_kJSonData.SaveFile("cwgame");
    }
    #endregion SAVE


}
