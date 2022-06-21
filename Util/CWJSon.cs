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
/// <summary>
/// JSON을 이용하여, 패킷, 파일 저장,각종 데이타 보관등의 작업을 한다 
/// </summary>

public class CWJSon
{
    JObject m_jData;

    public CWJSon(byte[] bBuffer)
    {
        m_jData = ConvertJSon(bBuffer);
    }

    public CWJSon(JObject jData)
    {
        
        m_jData = jData;
    }
    public CWJSon()
    {
        m_jData = new JObject();
    }
    public void Close()
    {
        m_jData.RemoveAll();
        m_jData = null;
        m_jData = new JObject();
    }
    public bool IsLoad()
    {
        
        if (m_jData == null) return false;
        if (m_jData.Count == 0) return false;
        return true;
    }
    public MemoryStream GetStream()
    {
        if (m_jData == null) return null;
        MemoryStream ms = new MemoryStream();
        using (BsonWriter writer = new BsonWriter(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(writer, m_jData);
            return CWLib.compress(ms.ToArray());
        }

    }
    //Json -> bin
    public byte[] ConvertBin(JObject JData)
    {
        if (JData == null) return null;
        MemoryStream ms = new MemoryStream();
        using (BsonWriter writer = new BsonWriter(ms))
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(writer, JData);
            MemoryStream cdata = CWLib.compress(ms.ToArray());
            return cdata.ToArray();
        }
    }
    // bin -> Json
    public JObject ConvertJSon(byte [] bdata)
    {
        if (bdata == null) return null;
        MemoryStream ms = CWLib.Uncompress(bdata);
        if(ms==null)
        {
            return null;
        }
        JObject JData;
        using (BsonReader reader = new BsonReader(ms))
        {
            JData = (JObject)JToken.ReadFrom(reader);
        }
        return JData;
    }


    #region SAVE/LOAD
    public bool Save(string szPath)
    {
        byte[] kByte = ConvertBin(m_jData);

#if UNITY_EDITOR
        
        
        try
        {
            using (FileStream fs = File.OpenWrite(szPath))
            {
                fs.Write(kByte, 0, kByte.Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szPath + " (Reason: " + e.Message + ")");
        }
#endif

        return true;
    }

    public bool SaveFile(string szFile)
    {
        byte[]  kByte= ConvertBin(m_jData);

        szFile = CWLib.DelExtString(szFile);
#if UNITY_EDITOR
        string szpath = string.Format("{0}/Resources/Gamedata/", Application.dataPath);
        string szname = string.Format("{0}{1}.bytes", szpath, szFile);
        try
        {
            using (FileStream fs = File.OpenWrite(szname))
            {
                fs.Write(kByte, 0, kByte.Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szname + " (Reason: " + e.Message + ")");
        }
#endif

        return true;
    }

    public JObject Load(string szname)
    {
        TextAsset textAsset = Resources.Load<TextAsset>(szname);
        if (textAsset != null)
        {
            MemoryStream fs = new MemoryStream(textAsset.bytes);
            {
                byte[] kByte = null;
                if (fs.Length > 0)
                {
                    kByte = new byte[fs.Length];
                    fs.Read(kByte, 0, (int)fs.Length);
                }
                fs.Close();
                m_jData = ConvertJSon(kByte);
                return m_jData;
            }
        }
        return null;
    }
    //StreamingAssets에 있음
    //public JObject Load(string szname)
    //{
    //    byte[] bBuffer = StreamAssetManager.Instance.GetData(szname);
    //    if (bBuffer != null)
    //    {
    //        m_jData = ConvertJSon(bBuffer);
    //        return m_jData;
    //    }
    //    return null;
    //}

    public bool LoadFile(string szFile)
    {
        string szname = string.Format("Gamedata/{0}", szFile);
        TextAsset textAsset = Resources.Load<TextAsset>(szname);
        if (textAsset != null)
        {
            MemoryStream fs = new MemoryStream(textAsset.bytes);
            {
                byte[] kByte = null;
                if (fs.Length > 0)
                {
                    kByte = new byte[fs.Length];
                    fs.Read(kByte, 0, (int)fs.Length);
                }
                fs.Close();
                m_jData = ConvertJSon(kByte);
                return true;
            }
        }

        return false;
    }
    public JObject LoadGamedata(string szFile)
    {
        string szname = string.Format("{0}", szFile);
        TextAsset textAsset = Resources.Load<TextAsset>(szname);
        if (textAsset != null)
        {
            MemoryStream fs = new MemoryStream(textAsset.bytes);
            {
                byte[] kByte=null;
                if (fs.Length > 0)
                {
                    kByte = new byte[fs.Length];
                    fs.Read(kByte, 0,(int) fs.Length);
                }
                fs.Close();
                m_jData= ConvertJSon(kByte);
                return m_jData;
            }
        }

        return null;
    }

    public void SaveLocal(string szfile)
    {
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/{1}", Application.persistentDataPath, szfile);
        cf.PutBuffer(ConvertBin(m_jData));
        cf.Save(szpath);
        

    }
    public bool LoadLocal(string szfile)
    {
        CWFile cf = new CWFile();
        string szpath = string.Format("{0}/{1}", Application.persistentDataPath,szfile);
        if (!cf.Load(szpath)) return false;
        byte [] bBuffer= cf.GetBuffer();
        m_jData = ConvertJSon(bBuffer);
        return true;
    }
    #endregion


    public JToken GetJson(string szParam)
    {
        try
        {
            return m_jData[szParam];
        }
        catch (ArgumentNullException e)
        {
            CWUnityLib.DebugX.Log(e.ToString());
        }
        return null;

    }
    public int GetInt(string szParam)
    {
        if (m_jData==null)
        {
            return 0;
        }
        if (m_jData[szParam] == null) return 0;

        return CWLib.ConvertInt(m_jData[szParam].ToString());
    }
    public float GetFloat(string szParam)
    {


        JToken jj = m_jData[szParam];
        if (jj == null) return 0;
        if (m_jData == null)
        {
            return 0;
        }


        return CWLib.ConvertFloat(m_jData[szParam].ToString());

    }
    public byte [] GetBytes(string szParam)
    {
        if (m_jData[szParam] == null) return null;

        try
        {
            return m_jData[szParam].Value<byte[]>();
        }
        catch (ArgumentNullException e)
        {
            Debug.LogWarning(e.ToString());
        }
        return null;
    }
    public DateTime GetTime(string szParam)
    {
        return m_jData[szParam].Value<DateTime>();
    }
    public string GetString(string szParam)
    {
        if (m_jData[szParam] == null) return "";
        try
        {
            return m_jData[szParam].ToString();//m_jData[szParam].Value<string>();
        }
        catch (ArgumentNullException e)
        {
            //Debug.LogWarning(e.ToString());
        }
        return "";
    }
    public void Add(string propertyName, JToken value)
    {
        JToken js = m_jData.GetValue(propertyName);
        if(js!=null)
        {
            m_jData[propertyName] = value;
        }
        else
        {
            m_jData.Add(propertyName, value);
        }
        
    }

    public byte [] ToArray()
    {
        return ConvertBin(m_jData);
    }
    public JObject GetObject()
    {
        return m_jData;
    }
    public override string ToString()
    {
        return m_jData.ToString();
    }
    public void SetData(byte [] bBuffer)
    {
        m_jData = ConvertJSon(bBuffer);
    }

    public bool LoadString(string Jstring)
    {
        if (!CWLib.IsString(Jstring)) return false;
        m_jData = JObject.Parse(Jstring);
        return true;
    }

    #region 정적함수
    public static  JToken GetJson(JToken _Data, string szParam)
    {
        try
        {
            if (_Data == null) return null;
            if (_Data[szParam] == null) return null;
            if (_Data[szParam].ToString()=="") return null;

            return _Data[szParam];
        }
        catch (ArgumentNullException e)
        {
            CWUnityLib.DebugX.Log(e.ToString());
        }
        return null;

    }

    public static string GetString(JToken _Data, string szVal)
    {
        if (_Data[szVal] == null) return "";
        try
        {
            return _Data[szVal].ToString();
        }
        catch (ArgumentNullException e)
        {
            Debug.LogWarning(e.ToString());
        }
        return "";
    }
    public static JArray GetArray(JToken _Data, string szVal)
    {
        if (_Data[szVal] == null) return null;
        try
        {
            return (JArray)_Data[szVal];
        }
        catch (ArgumentNullException e)
        {
            Debug.LogWarning(e.ToString());
        }
        return null;

    }
    public static int GetInt(JToken _Data,string szVal)
    {
        if(_Data==null)
        {
            return 0;
        }
        if (_Data.ToString() == "") return 0;
        if (_Data[szVal] == null) return 0;
        string szval = _Data[szVal].ToString();
        return CWLib.ConvertInt(szval);
    }
    public static float GetFloat(JToken _Data, string szVal)
    {
        if (_Data == null) return 0;
        if (_Data[szVal] == null) return 0;
        string szval = _Data[szVal].ToString();
        return CWLib.ConvertFloat(szval);
    }
    public static DateTime GetTime(JToken _Data, string szParam)
    {
        if (_Data == null) return DateTime.MinValue;
        if (_Data.ToString() == "") return DateTime.MinValue;
        if (_Data[szParam] == null) return DateTime.MinValue;
        if (_Data[szParam].ToString() == "") return DateTime.MinValue;
        return _Data[szParam].Value<DateTime>();
    }

    #endregion


}
