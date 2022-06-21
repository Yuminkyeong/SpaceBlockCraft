using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;

public class CWCharacter : MonoBehaviour
{

    public GameObject m_gObject;

    Animator m_kAnimator;

    private void Awake()
    {
        m_gObject.SetActive(false);

        m_kAnimator = GetComponentInChildren<Animator>(true);
    }
    public void SetIdle()
    {
        m_kAnimator.SetBool("Walking", false);
        m_kAnimator.SetBool("LeftTurn", false);
        m_kAnimator.SetBool("SadIdle", false);

    }

    public void Show(TALKTYPE kType)
    {
        m_gObject.SetActive(true);
        if (kType==TALKTYPE.NORMAL)
        {
            SetIdle();
        }
        if (kType == TALKTYPE.HAPPY)
        {
            m_kAnimator.SetBool("Walking", true);
            ///Invoke("SetIdle", 1.3f);

        }
        if (kType == TALKTYPE.HERE)
        {
            m_kAnimator.SetBool("LeftTurn", true);
            Invoke("SetIdle", 1.3f);
        }

        if (kType == TALKTYPE.SADNESS)
        {
            m_kAnimator.SetBool("SadIdle", true);
            Invoke("SetIdle", 1.3f);
        }



    }
    public void Close()
    {
        m_gObject.SetActive(false);
    }
    public void SetPos(Vector3 vPos)
    {
        transform.position = vPos;
    }


}
