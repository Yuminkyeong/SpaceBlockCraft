using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CWUnityLib;
public class ConvertJsonFile : MonoBehaviour {

    /*
     * 모든 파일을 읽어 들어서 Json 으로 바꾼다 . 
     * 
     * */

    MemoryStream LoadPck(string szpath)
    {
        byte[] fileData = File.ReadAllBytes(szpath);
        MemoryStream fs = new MemoryStream(fileData);
        byte[] bPck = new byte[fs.Length];
        fs.Seek(4, SeekOrigin.Begin);
        fs.Read(bPck, 0, (int)fs.Length);
        fs.Close();
        return CWLib.Uncompress(bPck);

    }

    void MakeBuildFile()
    {

        string szPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        szPath = szPath + "oldfile/BuildObject";
        DirectoryInfo dirs = new DirectoryInfo(szPath);
        FileInfo[] files = dirs.GetFiles();
        foreach(var v  in files)
        {
            print("begin  " + v.Name);
            GameObject gg = new GameObject();
            CWAirObject bObject = gg.AddComponent<CWAirObject>();
            if (bObject.LoadOldFile(LoadPck(v.FullName)))
            {
                string szpath = string.Format("{0}/Resources/Gamedata/{1}.bytes", Application.dataPath, v.Name);
                bObject.Save(szpath);
            }
            GameObject.Destroy(gg);

            print("end  " + v.Name);
        }
    }
    void MakeAirFile()
    {
        string szPath = Application.dataPath.Substring(0, Application.dataPath.Length - 6);
        szPath = szPath + "oldfile/AirObject";
        DirectoryInfo dirs = new DirectoryInfo(szPath);
        FileInfo[] files = dirs.GetFiles();
        foreach (var v in files)
        {
            print("begin  " + v.Name);
            GameObject gg = new GameObject();
            CWAirObject bObject = gg.AddComponent<CWAirObject>();

            if (bObject.LoadOldFile(LoadPck(v.FullName)))
            {
                string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, v.Name);
                bObject.Save(szpath);

            }
            GameObject.Destroy(gg);
            print("end  " + v.Name);
        }

    }

    void MakeFile()
    {
        // 
     //   MakeBuildFile();
        MakeAirFile();
    }


    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,200,200),"Make"))
        {
            MakeFile();
        }
    }


    void Start () {
		
	}
	
	
	void Update () {
		
	}
}
