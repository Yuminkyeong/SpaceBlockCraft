using UnityEngine;
using System.Collections;

using System.IO;
using SimpleJSON;

public class ConfigMgr : CWManager<ConfigMgr>
{
    public string _Key;

    private JSONNode _Data;

    public void LoadConfig()
    {
        StringReader _SR = new StringReader(Resources.Load("Config").ToString());

        _Data = JSON.Parse(CryptUtil.AESDecryptString(_SR.ReadLine(), _Key));

        _SR.Close();

        //NetworkMgr.Instance.m_Url = _Data["Http"];

    }

    public int GetSystemType()
    {
        return int.Parse(_Data["Type"]);
    }

    public string GetHttp()
    {
        return _Data["Http"];
    }

    public string GetIP()
    {
        return _Data["IP"];
    }

    public string GetUrl()
    {
        return _Data["Url"];
    }

    public string GetUnityAD()
    {
        return _Data["UnityAD"];
    }

    public string GetADColony()
    {
        return _Data["ADColony"];
    }

    public string GetZoneString()
    {
        return _Data["ZoneString"];
    }

    public string GetOneStore()
    {
        return _Data["OneStore"];
    }

    public bool GetDebug()
    {
        return int.Parse(_Data["Debug"]) == 1;
    }

    public string GetGooglePublicKey()
    {
        return _Data["GooglePublicKey"];
    }
}
