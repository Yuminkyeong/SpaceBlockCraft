using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Char_Digg : MonoBehaviour
{

    enum ANIENUM { IDLE, ATTACK, WALK, DANCE1, DANCE2, DANCE3 };
    public Animation m_kAnimation;
    public List<AnimationClip> m_kClipList = new List<AnimationClip>();

    public GameObject m_gWeapon;
    public GameObject m_gBlock;


    // Start is called before the first frame update
    void Start()
    {
        //m_kClipList.Clear();
        //foreach (AnimationState v in m_kAnimation)
        //{
        //    m_kClipList.Add(v.clip);
        //}

    }

    IEnumerator IAniRun(int num, bool bLooping)
    {
        if (m_kClipList.Count > 0)
        {
            float ftime = Mathf.Min(m_kClipList[num].length, 2f);

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
                StartCoroutine(IAniRun((int)ANIENUM.IDLE, true));

            }

        }


        yield return null;
    }
    public void SetIdle()
    {
        if (!gameObject.activeSelf) return;
        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.IDLE, true));

    }
    public void SetAttack()
    {

        StopAllCoroutines();
        StartCoroutine(IAniRun((int)ANIENUM.ATTACK, false));
        CWResourceManager.Instance.PlaySound("digup"); //블록 캐는 소리
    }


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
        if (nBlock == 0)
        {
            m_gWeapon.SetActive(true);
            m_gBlock.SetActive(false);
            return;
        }
        m_gWeapon.SetActive(false);
        m_gBlock.SetActive(true);

        UpdateUV(m_gBlock, nBlock);


    }


}
