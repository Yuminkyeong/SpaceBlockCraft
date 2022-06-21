using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * 애니메이션 관리 
 * */
public class CharBody : MonoBehaviour
{

    enum ANIENUM { IDLE, ATTACK, WALK, DANCE1, DANCE2, DANCE3 };
    public Animation m_kAnimation;
    List<AnimationClip> m_kClipList = new List<AnimationClip>();
    // 무기 

    public GameObject m_gWeapon;
    public GameObject m_gBlock;

    public GameObject m_gDummy;
    public GameObject m_gPlayPos;
    const float GRIDE = 32f;
    const float FRate = 1 / 32f;

    void GetUV(ref Vector2 vUV, byte nBlock)
    {
        int x = CWArrayManager.Instance.m_kBlock[nBlock].x;
        int y = CWArrayManager.Instance.m_kBlock[nBlock].y;

        float sx = (float)x;
        float sy = (float)y;
        vUV.x = sx / GRIDE;
        vUV.y = sy / GRIDE;

    }
    void UpdateUV(GameObject gg, int nBlock)
    {


        Renderer rr = gg.GetComponent<Renderer>();
        Vector2 vv = new Vector2();
        GetUV(ref vv, (byte)nBlock);
 //       rr.material = new Material(Shader.Find("CWShader"));
        rr.material.mainTextureOffset = vv;
        rr.material.mainTextureScale = new Vector2(FRate, FRate);
    }

    public void SelectBlock(int nBlock)
    {
        if(nBlock == 0)
        {
            m_gWeapon.SetActive(true);
            m_gBlock.SetActive(false);
            return;
        }
        m_gWeapon.SetActive(false);
        m_gBlock.SetActive(true);
        
        UpdateUV(m_gBlock, nBlock);


    }

    void Start()
    {
        foreach (AnimationState v in m_kAnimation)
        {
            m_kClipList.Add(v.clip);
        }

    }

    IEnumerator IAniRun(int num,bool bLooping)
    {
        if (m_kClipList.Count > 0)
        {
            float ftime =Mathf.Min(m_kClipList[num].length,2f); 

            if (bLooping)
            {
                while (true)
                {
                    m_kAnimation.Play(m_kClipList[num].name);
                    yield return new WaitForSeconds(ftime);
                }

            }
            else
            {
                m_kAnimation.Play(m_kClipList[num].name);
                yield return new WaitForSeconds(ftime);
                //   m_kAnimation.CrossFade(m_kClipList[(int)ANIENUM.IDLE].name, 0.3f, PlayMode.StopSameLayer);
                StartCoroutine(IAniRun((int)ANIENUM.ATTACK, true));

            }

        }
        

        yield return null;
    }
    ANIENUM[] AA = { ANIENUM.IDLE, ANIENUM.WALK, ANIENUM.DANCE1, ANIENUM.DANCE2};

    float IIdleRunFnc()
    {
        

        if (m_kClipList.Count == 0)
        {
            return 0;
        }
        int num = (int)ANIENUM.IDLE;
        int RR = Random.Range(0, AA.Length);
        num =(int) AA[RR];

        float ftime = m_kClipList[num].length;
        m_kAnimation.Play(m_kClipList[num].name);

  ///      Debug.Log(string.Format(" {0} {1}  {2}", m_kClipList[num].name,num, AA[RR]));


        return ftime;
    }
    IEnumerator IIdleRun()
    {

        while (true)
        {
            float ftime = IIdleRunFnc();
            Vector3 vv= transform.localPosition;
            vv.y = -1.3f;
            transform.localPosition = vv;

            yield return new WaitForSeconds(ftime);

        }


    }
    public void SetDie()
    {

    }
    public void SetRandomDance()
    {
        StopAllCoroutines();
        StartCoroutine("IIdleRun");
    }
    public void SetIdle()
    {
        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.IDLE,true));

    }
    public void SetAttack()
    {

        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.ATTACK,false));
    }
    public void SetWalk()
    {
        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.WALK,true));
    }
    public void SetDance(int num)//1부터 시작
    {
        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.DANCE1 + num - 1,false));
    }


}
