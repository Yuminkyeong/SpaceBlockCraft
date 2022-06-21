using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveTweenRun : MonoBehaviour {

    bool bflag = false;
    public Vector3 m_vStart;
    public Vector3 m_vEnd;
    private void OnEnable()
    {

        TweenPosition tw = GetComponent<TweenPosition>();
        if(tw)
        {
            if(!bflag)
            {
                m_vStart = tw.from;
                m_vEnd = tw.to;
            }
            tw.to = m_vEnd;
            tw.from = m_vStart;
            tw.duration = 1;
            tw.ResetToBeginning();
            tw.PlayForward();
            
        }
    }
    private void OnDisable()
    {
        TweenPosition tw = GetComponent<TweenPosition>();
        if (tw)
        {

            tw.to = m_vStart;
            tw.from = m_vEnd; 
            tw.duration = 1;
            tw.ResetToBeginning();
            tw.PlayReverse();
        }

    }
}
