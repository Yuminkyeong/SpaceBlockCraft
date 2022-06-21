using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshDebug : CWSingleton<MeshDebug>
{

    public Dictionary<GameObject, string> m_gData = new Dictionary<GameObject, string>();

    private void Start()
    {
        
    }
    public void AddData(GameObject gg,string szname)
    {
        if(m_gData.ContainsKey(gg))
        {
            
            return;
        }

        m_gData.Add(gg,szname);
    }


}
