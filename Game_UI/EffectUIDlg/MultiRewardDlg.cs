using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWEnum;
using CWStruct;
public class MultiRewardDlg : EffectUIDlg<MultiRewardDlg>
{

    public GameObject [] m_gStar;

    public GameObject [] m_gStarEndPos;
    
    public RawImage m_kItem;
    Vector3[] m_vStartPos=new Vector3[4];

    public GameObject m_gStarImage;
    
    int m_nCount = 1;
    public Image m_bkImage;
    public Text m_kResultText;
    public Text m_kResultText2;

    bool m_bBomb = false;
    public void Show( CBClose Function = null)
    {

        
        int nItem =(int) GITEM.Ticket;
        int RR = CWLib.Random(0,100);//0, 1,2
        Debug.Log("Random = " + RR.ToString());
/*
        if (RR < CWGlobal.MULTI_TICKETRANDOM)
        {
            m_bBomb = false;
            nItem = (int)GITEM.Ticket;
            string szstr = CWLocalization.Instance.GetLanguage("{0}개의 랭킹포인트와 입장교환권 획득!");
            m_kResultText.text = string.Format(szstr, m_nCount);

            m_kResultText2.text = "입장권으로 교환해서 사용하세요!";

            //m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
            m_kItem.gameObject.SetActive(false);

        }
        else
        {

            // 폭탄
            nItem = (int)GITEM.Bomb1;
            RR = Random.Range(1, 100);
            if (RR < 80)
            {
                nItem = (int)GITEM.Bomb1;
            }
            if (RR >= 80 && RR < 90)
            {
                nItem = (int)GITEM.Bomb2;
            }
            if (RR >= 90 && RR < 95)
            {
                nItem = (int)GITEM.Bomb3;
            }

            if (RR >= 95 && RR < 98)
            {
                nItem = (int)GITEM.Bomb4;
            }

            string szstr = CWLocalization.Instance.GetLanguage("{0}개의 랭킹포인트와 폭탄 획득!");
            m_kResultText.text = string.Format(szstr, m_nCount);
            m_kResultText2.text = "";
            m_kItem.gameObject.SetActive(true);
            m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
            m_bBomb = true;



        }
        */

        m_bBomb = false;
        nItem = (int)GITEM.Ticket;
        string szstr = CWLocalization.Instance.GetLanguage("{0}개의 랭킹포인트와 입장교환권 획득!");
        m_kResultText.text = string.Format(szstr, m_nCount);

        m_kResultText2.text = "입장권으로 교환해서 사용하세요!";

        //m_kItem.texture = CWResourceManager.Instance.GetItemIcon(nItem);
        m_kItem.gameObject.SetActive(false);

        m_nCount = 1;

        NoticeMessage.Instance.Close();
        

        foreach(var v in m_gStar) { v.SetActive(false); }
        CloseFuction = Function;

        
        Open();
        StartCoroutine("IRun");


        m_bkImage.DOFade(1, 0).OnComplete(()=> {
            m_bkImage.DOFade(0, 4);
        });

    }
    

    IEnumerator IMoveStar(int num)
    {

        ParticleSystem ps = m_gStar[num].GetComponentInChildren<ParticleSystem>();
        ps.Play();
        m_gStar[num].transform.DOScale(1, 0);
        m_gStar[num].transform.position = m_vStartPos[num];
        m_gStar[num].SetActive(true);
        yield return new WaitForSeconds(1f);

        m_gStar[num].transform.DOScale(0.5f, 0.6f);
        while(true)
        {

            Vector3 vPos = m_gStar[num].transform.position;
            vPos = Vector3.Lerp(vPos, m_gStarEndPos[num].transform.position,Time.deltaTime*10f);
            float fdist = Vector3.Distance(vPos, m_gStarEndPos[num].transform.position);
            if(fdist<=0.1f)
            {
                break;
            }
            m_gStar[num].transform.position = vPos;
            yield return null;
        }

        yield return new WaitForSeconds(1f);
        m_gStar[num].SetActive(false);
        yield return null;
        

    }

    IEnumerator IRun()
    {
        yield return null;

        //StartCoroutine(IMoveStar(0));
        CWResourceManager.Instance.MoveObjectStar(m_gStarEndPos[0], 1, m_gStarImage.transform);
        yield return new WaitForSeconds(0.3f);
        if(!m_bBomb)
            StartCoroutine(IMoveStar(1));

        yield return new WaitForSeconds(2f);

        Close();

    }
    
}
