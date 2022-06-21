using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
public class ChangeMeshValue : MonoBehaviour
{

    public void ChangeMesh()
    {
        MeshRenderer[] mm = gameObject.GetComponentsInChildren<MeshRenderer>();
        foreach(var v in mm)
        {
            //v.castShadows = false;
            v.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            v.receiveShadows = false;
            v.lightProbeUsage = LightProbeUsage.Off;
            v.reflectionProbeUsage = ReflectionProbeUsage.Off;

            MeshCollider g1 = v.GetComponent<MeshCollider>();
            if(g1!=null)
                DestroyImmediate(g1);

            BoxCollider g2 = v.GetComponent<BoxCollider>();
            if (g2 != null)
                DestroyImmediate(g2);


        }




    }
}
