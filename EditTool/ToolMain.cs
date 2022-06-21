using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
    using UnityEditor;
#endif
using UnityEngine;
using CWEnum;
public class ToolMain : MonoBehaviour {

	// Use this for initialization
	void Start () {

        CWGlobal.g_bEditmode = true;
#if UNITY_EDITOR

        AssetDatabase.Refresh();
#endif

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform tChild = transform.GetChild(i);
            if (tChild)
            {
                tChild.SendMessage("Create");
            }
        }

        CWGlobal.G_bGameStart = true;

        CWTableManager.Instance.ToolLoad();
        CWArrayManager.Instance.InitData();

        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
