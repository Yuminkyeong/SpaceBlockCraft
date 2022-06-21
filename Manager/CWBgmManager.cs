using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Runtime.InteropServices;
using System.Net;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using CWUnityLib;
using CWStruct;
using Random = UnityEngine.Random;

public class CWBgmManager : CWManager<CWBgmManager>
{

    

    public AudioClip[] m_gStage; //로비
    public AudioClip[] m_gWar; // 전투시
    public AudioClip[] m_gEdit; //에디터
    public AudioClip[] m_gDig; //인게임에서 블록캘때


    AudioSource m_gSource;
    AudioClip m_gSelect;

    List<AudioClip> m_gBgmList=new List<AudioClip>();

    public float m_fMaxvolume = 1f;
    float m_fSpeed = 0.5f;

    bool _BgmOn =false;
    bool _bCreated = false;

    public void SetBGM(bool bflag)
    {
        _BgmOn = bflag;
        if(!_BgmOn)
        {
            PlayStop();
        }
    }

    public bool m_bBgmOn
    {
        get
        {
            return _BgmOn;
        }
        set
        {
            if (!_bCreated) return;
            _BgmOn = value;

            if(_BgmOn)
            {
                PlayStart();
            }
            else
            {
                PlayStop();
            }

        }
    }


    IEnumerator SoundStartCoroutine()
    {
        if (m_gSelect != null)
        {

            float t = m_gSource.volume;
            while (t > 0.0f)
            {
                t -= Time.deltaTime * m_fSpeed;
                m_gSource.volume = t;
                yield return new WaitForSeconds(0.05f);
            }
        }
        m_gSource.clip = m_gSelect;
        m_gSource.Play();
        _BgmOn = true;

        float tt = 0.0f;
        while (tt < m_fMaxvolume)
        {
            tt += Time.deltaTime * m_fSpeed;
            m_gSource.volume = tt;
            yield return new WaitForSeconds(0.05f);

        }
        StartCoroutine("RunCoroutine");
    }
    IEnumerator RunCoroutine()
    {

        while (true)
        {
            if (!m_gSource.isPlaying)
            {
                for (int i = 0; i < m_gBgmList.Count; i++)
                {
                    if (m_gBgmList[i] == m_gSelect)
                    {
                        int num = i + 1;
                        num = num % (m_gBgmList.Count);
                        m_gSelect = m_gBgmList[num];
                        m_gSource.clip = m_gSelect;
                        m_gSource.Play();
                        _BgmOn = true;
                        break;
                    }
                }
            }
            yield return new WaitForSeconds(1f);
        }
    }
    IEnumerator SoundStopCoroutine()
    {
        float t = m_gSource.volume;
        while (t > 0.2f)
        {
            t -= Time.deltaTime * 20;
            m_gSource.volume = t;
            yield return new WaitForSeconds(0.05f);
        }
        yield return new WaitForSeconds(5f);
        float tt = m_gSource.volume;
        while (tt < m_fMaxvolume)
        {
            tt += Time.deltaTime * 20;
            m_gSource.volume = tt;
            yield return new WaitForSeconds(0.05f);

        }
        m_gSource.Stop();
        _BgmOn = false;

    }
    public void PlayStart()
    {
        PlayStage();
        StartCoroutine("RunCoroutine");
    }
    public void PlayStop()
    {


        StopAllCoroutines();
        m_gSource.Stop();
        _BgmOn = false;
    }

    public void PlayStage()
    {
        if (!m_bBgmOn) return;
        int tcnt= m_gStage.Length;
        int rr = Random.Range(0, tcnt);
        m_gBgmList.Clear();
        foreach (var v in m_gStage)
        {
            m_gBgmList.Add(v);
        }
        PlayBgm(m_gStage[rr]);
    }
    public void PlayLobby()
    {
        if (!m_bBgmOn) return;
        //  int tcnt = m_gLobby.Length;
        // int rr = Random.Range(0, tcnt);
        //    PlayBgm(m_gLobby[rr]);
    }
    public void PlayEdit()
    {
        if (!m_bBgmOn) return;
        int tcnt = m_gEdit.Length;
        int rr = Random.Range(0, tcnt);

        m_gBgmList.Clear();
        foreach (var v in m_gEdit)
        {
            m_gBgmList.Add(v);
        }
        PlayBgm(m_gEdit[rr]);

    }
    public void PlayDigg()
    {
        if (!m_bBgmOn) return;
        int tcnt = m_gDig.Length;
        int rr = Random.Range(0, tcnt);

        m_gBgmList.Clear();
        foreach (var v in m_gDig)
        {
            m_gBgmList.Add(v);
        }
        PlayBgm(m_gDig[rr]);

    }

    public void PlayWar()
    {
        if (!m_bBgmOn) return;
        int tcnt = m_gWar.Length;
        int rr = Random.Range(0, tcnt);

        m_gBgmList.Clear();
        foreach (var v in m_gWar)
        {
            m_gBgmList.Add(v);
        }
        PlayBgm(m_gWar[rr]);

         
        
    }
    public void PlayTuto()
    {
        PlayWar();
    }
    public void PlayBgm(AudioClip gSelect)
    {
        if (!m_bBgmOn) return;

        m_gSelect = gSelect;
        StopAllCoroutines();
        StartCoroutine("SoundStartCoroutine");
    }

    public override void Create()
    {
        m_gSource = GetComponent<AudioSource>();

        
        m_gSelect = m_gDig[0];
        m_gSource.clip = m_gSelect;
        m_gSource.Play();
        _BgmOn = true;
        StartCoroutine("RunCoroutine");

        _bCreated = true;
        base.Create();
    }
    

}
