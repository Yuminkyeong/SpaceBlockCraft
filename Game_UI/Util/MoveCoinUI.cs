using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWEnum;
using CWUnityLib;

// 코인등 제화를 이동시키기 연출 
public class MoveCoinUI : MonoBehaviour
{
    public int m_nCount;
    public COIN m_nCoin;
    public GameObject m_gTarget;
    public float m_fTime = 0.4f;
    public GameObject m_gVisible;
    void Start()
    {
     
        if(m_gTarget==null)
        {
            m_gTarget = CoininfoDlg.Instance.m_gCoin[(int)m_nCoin];
        }
        StartCoroutine("IRun");
    }
    IEnumerator IRun()
    {
        yield return null;
        for (int i = 1; i < m_nCount; i++)
        {
            GameObject gg = Instantiate(m_gVisible);
            
            gg.transform.SetParent(transform);

            gg.transform.localPosition = Vector3.zero;
            gg.transform.localScale = Vector3.one;
            gg.transform.rotation = new Quaternion();
            gg.transform.DOMove(m_gTarget.transform.position, m_fTime);
            yield return new WaitForSeconds(0.2f);
        }
        m_gVisible.transform.DOMove(m_gTarget.transform.position, m_fTime);
        yield return null;
        Destroy(gameObject,0.1f);
    }
}
