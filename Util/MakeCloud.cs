using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class MakeCloud : MonoBehaviour
{

    public float m_fScale = 1f;
    public GameObject [] m_gCloud;
    GameObject m_gDir = null;
    private void Start()
    {
        Make();
    }
    public void Make()
    {
        if(m_gDir!=null)
        {
            DestroyImmediate(m_gDir);
        }
        m_gDir = new GameObject();
        m_gDir.transform.parent = transform;
        m_gDir.transform.localPosition = Vector3.zero;

        for (int x=-8;x<40;x++)
        {
            for (int z = -8; z < 40; z++)
            {
                int rr2 = Random.Range(0, 20);
                if (rr2 != 0) continue;

                int rr3 = Random.Range(0, m_gCloud.Length);
                GameObject gg= Instantiate(m_gCloud[rr3]);
                gg.transform.parent = m_gDir.transform;
                gg.transform.localPosition = new Vector3(x*8 * m_fScale, 0,z*8 * m_fScale);

                float f1= Random.Range(1f, 3.1f)* m_fScale;
                float f2= Random.Range(1f, 3.1f) * m_fScale;

                gg.transform.localScale = new Vector3(f1, 1, f2);
            }

        }
        int rr = Random.Range(50, 200);
        m_gDir.transform.DOLocalMoveZ(100, rr).SetLoops(-1, LoopType.Restart);
    }
}
