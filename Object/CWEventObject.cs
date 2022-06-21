using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWStruct;
using CWEnum;
using CWUnityLib;

public class CWEventObject : CWBuildObject
{

  
    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.EVENT;
    }

    public override void SetDie()
    {
        // 퀘스트 종료
       

        
        gameObject.SetActive(false);
    }
    protected override void OnHit(int nDamage)
    {
        SetDie();
    }
    protected override void CreatePower()
    {

        SphereCollider sc = gameObject.AddComponent<SphereCollider>();
        sc.radius = 6;
        CWLib.SetGameObjectLayer(gameObject, 10);

        base.CreatePower();
    }
}
