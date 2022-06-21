using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class BlockDamageUI : CWSingleton<BlockDamageUI>
{
    public float m_MaxTime = 1f;
    public Renderer m_kRender;
    public void Begin()
    {
      //  StopAllCoroutines();
       // StartCoroutine("IRun");


    }
    Vector2[] offset =
    {
         new Vector2(0,0),new Vector2(0,0.5f),new Vector2(0.5f,0),new Vector2(0.5f,0.5f)
    };

    IEnumerator IRun()
    {


        int num = 0;
        float fStart = Time.time;
        while (true)
        {
           
            m_kRender.sharedMaterial.SetTextureOffset("_MainTex", offset[num]);
            num++;
            if (num >= offset.Length) break;
            yield return new WaitForSeconds(0.1f);
        }

        
    }

}
