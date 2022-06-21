using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class DotweenTest : MonoBehaviour
{
    public GameObject m_gTarget;

    public GameObject m_gShoot;

    public GameObject m_gPath;
    private void Start()
    {

        GameObject gg = Instantiate(m_gPath);
        gg.transform.position = Vector3.zero;
        PostionLink pp= m_gTarget.AddComponent<PostionLink>();
        pp.m_LinkPostion = gg;


    }
    void Shoot()
    {

    }
    private void OnGUI()
    {
        if(GUI.Button(new Rect(0,0,100,100),"click"))
        {
            GameObject gg = Instantiate(m_gShoot);
            gg.transform.position = m_gTarget.transform.position;
            Vector3 vPos = m_gTarget.transform.forward * 200;
            gg.transform.DOMove(vPos, 2f);

        }
    }

}
