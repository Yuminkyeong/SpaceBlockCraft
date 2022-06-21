using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWStruct;
using CWEnum;
using CWUnityLib;
public class ProductionTest : MonoBehaviour
{


    public float m_fTimeSale = 1f;
    public CWProductionRoot m_kProduction;

    private void Awake()
    {
    }
    void PlayMap()
    {
    }
    private void Start()
    {
        
        
        StartCoroutine("IRun");
    }

    // 상황에 맞게 여기를 고칠 것!!!
    IEnumerator IRun()
    {
        yield return new WaitForSeconds(1f);

        bool bexit = false;
        while(!bexit)
        {
            if(Space_Map.Instance.IsShow())
            {
                bexit = true;
            }
            yield return null;
        }
        m_kProduction.Begin();
    }




}
