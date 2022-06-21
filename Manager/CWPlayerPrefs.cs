using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class CWPlayerPrefs : CWManager<CWPlayerPrefs>
{
    Dictionary<string, string> m_PlayerPrefs = new Dictionary<string, string>();
    bool m_bUpdated=false;
    public override void Create()
    {
        StartCoroutine("IRun");
        base.Create();
    }
    public void UpdateData()
    {
        if (m_bUpdated)
        {
            m_bUpdated = false;
            CWSocketManager.Instance.UpdatePlayerData(m_PlayerPrefs);
            Debug.Log("업데이트 playerprefs");
        }

    }
    IEnumerator IRun()
    {
        while(true)
        {
            yield return new WaitForSeconds(1f);
            UpdateData();
            

        }
    }
    
    public void InitPlayerPrefs()
    {
        m_PlayerPrefs.Clear();
        //m_PlayerPrefs값을 서버에서 미리 가져 온다
        CWSocketManager.Instance.GetPlayerData((jData)=> {

            Debug.Log("InitPlayerPrefs()");

            if (jData["Result"].ToString() == "ok")
            {
                JArray ja = CWJSon.GetArray(jData, "PData");
                for (int i = 0; i < ja.Count; i++)
                {
                    JToken jj = ja[i];
                    string key = CWJSon.GetString(ja[i], "key");
                    string value = CWJSon.GetString(ja[i], "value");
                    if(!m_PlayerPrefs.ContainsKey(key))
                        m_PlayerPrefs.Add(key, value);
                }
            }
        });

    }
    public static string GetString(string key)
    {
        return CWPlayerPrefs.Instance._GetString(key);
    }

    public string _GetString(string key)
    {
        //m_PlayerPrefs여기에서 값을 가져옴
        if (m_PlayerPrefs.ContainsKey(key))
        {
            return m_PlayerPrefs[key];
        }
        return null;
    }
    public  void _SetString(string key, string szValue)
    {
        // 서버에 넣고 
        //m_PlayerPrefs에도 값을 넣는다 
        if (m_PlayerPrefs.ContainsKey(key))
        {
            m_PlayerPrefs[key] = szValue;
        }
        else
        {
            m_PlayerPrefs.Add(key, szValue);
        }
        m_bUpdated = true;
    }
    public static void SetString(string key, string szValue)
    {

        CWPlayerPrefs.Instance._SetString(key, szValue);
    }


    public static float GetFloat(string key)
    {
        string values = CWPlayerPrefs.GetString(key);
        return CWLib.ConvertFloat(values);

    }
    //
    public static int GetInt(string key)
    {
        string values = CWPlayerPrefs.GetString(key);
        return CWLib.ConvertInt(values);

    }

    public  bool _HasKey(string key)
    {
        if (m_PlayerPrefs.ContainsKey(key))
        {
            return true;
        }
        return false;
    }

    public static  bool HasKey(string key)
    {

        return CWPlayerPrefs.Instance._HasKey(key);
    }
    public static void SetFloat(string key, float value)
    {
        CWPlayerPrefs.SetString(key, value.ToString());
    }
    //
    // 요약:
    //     Sets a single integer value for the preference identified by the given key. You
    //     can use PlayerPrefs.GetInt to retrieve this value.
    //
    // 매개 변수:
    //   key:
    //
    //   value:
    public static void SetInt(string key, int value)
    {
        CWPlayerPrefs.SetString(key, value.ToString());
    }
    //
    // 요약:
    //     Sets a single string value for the preference identified by the given key. You
    //     can use PlayerPrefs.GetString to retrieve this value.
    //
    // 매개 변수:
    //   key:
    //
    //   value:

}
