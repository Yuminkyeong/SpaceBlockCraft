
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using CWEnum;
using CWUnityLib;
using CWStruct;
using System.IO;

public class MakeMeshFile : MonoBehaviour
{

    public void MakeMesh(string filename)
    {

        Mesh kMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
        string szPath = string.Format("Assets/Resources/MeshAsset/{0}.asset", filename);
#if UNITY_EDITOR    
        AssetDatabase.CreateAsset(kMesh, szPath);
#endif

    }
}
