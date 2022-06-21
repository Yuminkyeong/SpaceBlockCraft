using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWUnityLib;
public class CWTweenColor : MonoBehaviour
{
    public enum Style
    {
        Once,
        Loop,
        PingPong,
    }
    public Style Play_Type=Style.Once;
    public float LifeTime = 1;
    public Color From = Color.white;
    public Color To = Color.white;
    public bool PlayOnAwke=false; // 시작하자 마자
    float m_Timer;

    bool m_bflag = false;
    bool m_bPingflag = false;

    private void Start()
    {
        if (PlayOnAwke)
        {
            Begin();
        }
    }

    public void Begin()
    {
        m_Timer = Time.time;
        m_bflag = true;
    }
    void Play()
    {
        float ff = Time.time - m_Timer;
        if(ff>=LifeTime)
        {
            if(Play_Type==Style.Once)
            {
                m_bflag = false;
                return;// 종료
            }
            if(Play_Type==Style.Loop)
            {
                Begin();
            }
            if (Play_Type == Style.PingPong)
            {
                m_bPingflag = !m_bPingflag;
                Begin();
            }

        }
        float fAlpa = ff/LifeTime;
        if (Play_Type == Style.PingPong)
        {
            if (m_bPingflag)
            {
                fAlpa = 1 - ff / LifeTime;
            }
        }
            
        
        Graphic kImage = GetComponent<Graphic>();
        if (kImage != null)
        {
            kImage.color = Color.Lerp(From, To, fAlpa);
        }
    }
    void Update()
    {
        if(m_bflag)
            Play();
    }

}
