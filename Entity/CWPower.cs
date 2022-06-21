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
using CWEnum;

public class CWPower : MonoBehaviour
{
    int _MaxDamage;
    public int m_nDamage
    {
        get
        {
            if (GamePlay.Instance != null && GamePlay.Instance.m_nWType == GamePlay.WTYPE.MULTI)// // 멀티라면
            {
                return CWGlobal.MULTI_ATTACK;
            }

            return _MaxDamage;
        }
        set
        {
            _MaxDamage = value;
        }
    }
    int _Maxhp;
    public int m_nHp
    {
        get
        {
            if (GamePlay.Instance!=null&&  GamePlay.Instance.m_nWType == GamePlay.WTYPE.MULTI )// // 멀티라면
            {
                return CWGlobal.MULTI_HP;
            }
            return _Maxhp;
        }
        set
        {
          
            _Maxhp = value;
        }
    }
    public float m_fEnergy;
    public int m_nRange;
    private float fSpeed;// 속도 
    private float fSpeedR;//우 속도 
    private float fSpeedL;//좌 속도 
    private float fhpRate=1;
    float _fEnRate;// 에너지 률 
    // 순간속도



    public delegate void HPEVENT();

    public HPEVENT hpEvent;
    float m_fTubo = 0;

    public float FhpRate
    {
        get
        {
            return fhpRate;
        }

        set
        {
            fhpRate = value;
            if (fhpRate >= 1) fhpRate = 1;
            if (hpEvent!=null)
                hpEvent();
        }
    }
    public float FEnRate
    {
        get
        {
            return _fEnRate;
        }

        set
        {
            _fEnRate = value;
            if (_fEnRate >= 1) _fEnRate = 1;
        }
    }


    public float FSpeed
    {
        get
        {
            return (fSpeed+ m_fTubo);
        }

        set
        {
            fSpeed = value;
        }
    }

    public float FSpeedR
    {
        get
        {
            return (fSpeedR + m_fTubo);
        }

        set
        {
            fSpeedR = value;
        }
    }
    public float FSpeedL
    {
        get
        {
            return (fSpeedL + m_fTubo);
        }

        set
        {
            fSpeedL = value;
        }
    }

    public int GetHP()
    {
        return (int)(m_nHp* FhpRate);
    }
    public int GetEnergy()
    {
        return (int)(m_fEnergy * FEnRate);
    }
    public void SetHP(int nHP)
    {
        if (nHP > m_nHp) nHP = m_nHp;

         fhpRate = nHP/(float) m_nHp;
    }
    public void UpdateHP(int nDamage)
    {
        int nhp =GetHP();
        nhp += nDamage;
        if (nhp < 0) nhp = 0;
        if (nhp >= m_nHp) nhp = m_nHp;

        FhpRate = nhp / (float)m_nHp;

    }
    public void UpdateEnergy(float fVal)
    {

        if (m_fEnergy == 0) return;
        float fEnergy = m_fEnergy * FEnRate;

        fEnergy += fVal;
        if (fEnergy < 0) fEnergy = 0;
        if (fEnergy >= m_fEnergy) fEnergy = m_fEnergy;

        FEnRate = fEnergy / (float)m_fEnergy;

    }

    
    void EndTubo()
    {
        m_fTubo = 0;
        GameObject gg = CWLib.FindChild(gameObject, "nozzle_fire");
        if (gg)
        {
            Vector3 vPos = gg.transform.localScale;
            vPos.y /= 10f;
            gg.transform.localScale = vPos;
        }

    }
    //public void SetTubo()
    //{

    //    GameObject gg= CWLib.FindChild(gameObject, "nozzle_fire");
    //    if(gg)
    //    {
    //        Vector3 vPos = gg.transform.localScale;
    //        vPos.y *= 10f;
    //        gg.transform.localScale = vPos;
    //    }

    //    Invoke("EndTubo",CWGlobal.TURBOTIME);
    //}
}

