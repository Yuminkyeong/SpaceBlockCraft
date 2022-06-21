using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


class ProceduralMesh
{
    
    [MenuItem("Assets/Create Procedural Mesh")]
    static void Create()
    {
        string filePath =
            EditorUtility.SaveFilePanelInProject("Save Procedural Mesh", "Procedural Mesh", "asset", "");
        if (filePath == "") return;

        //Selection.gameObjects[0].name

        ////Selection.gameObjects
        //foreach(var v in Selection.gameObjects)
        //{
        //    MeshFilter[] kMeshf = v.GetComponentsInChildren<MeshFilter>(); // GetComponent<MeshFilter>().sharedMesh.vertexCount;
        //    foreach(var k in kMeshf)
        //    {
        //        AssetDatabase.CreateAsset(k.sharedMesh, filePath);
        //    }
            

        //}

        
    }


}