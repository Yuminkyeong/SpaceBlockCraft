using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWEditAirObject : CWAirObject
{
    protected override GameObject CharblockAttach()
    {
        if (CWHeroManager.Instance.m_nCharNumber == 0) CWHeroManager.Instance.m_nCharNumber = 1;
         GameObject gg = new GameObject();
         CharBody cb = CWResourceManager.Instance.GetCharBody(CWHeroManager.Instance.m_nCharNumber);
         cb.transform.parent = gg.transform;
         cb.transform.localPosition = new Vector3(0, -1.3f, 0);
         cb.transform.localRotation = new Quaternion();

         //cb.MakeMeshCollider();

         BoxCollider bb = gg.AddComponent<BoxCollider>();
         bb.size = Vector3.one;
         return gg;
    }
    protected override void OnLoadEnd()
    {
        if (m_gHitDummy)
        {
            Destroy(m_gHitDummy);
        }

    }

}
