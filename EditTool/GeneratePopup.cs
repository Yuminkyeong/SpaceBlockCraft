using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GeneratePopup : MonoBehaviour
{



    public void MakePopup()
    {
#if UNITY_EDITOR
        SamplePopup ss =GetComponentInChildren<SamplePopup>();
        if(ss==null)
        {
            GameObject obj = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/SpaceBlock/Prefab/EditPrefab/DialogBox.prefab", typeof(GameObject));
            GameObject gg = Instantiate(obj, transform);
            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.transform.rotation = new Quaternion();
        }
        else
        {

        }

#endif
    }

}
