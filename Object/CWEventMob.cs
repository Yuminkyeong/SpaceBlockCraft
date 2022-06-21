using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
using DG.Tweening;

public class CWEventMob : CWMobPath
{


    int m_nItem;
    public void Begin()
    {
/*
        string szFile="Event_1";

        int RR = Random.Range(1, 100);

        if (RR < 10)
        {
            m_nItem = (int)GITEM.Bomb1;
            szFile = "Event_1";
        }
        if (RR >= 10 && RR < 30)
        {
            m_nItem = (int)GITEM.Bomb2;
            szFile = "Event_2";
        }
        if (RR >= 30 && RR < 70)
        {
            m_nItem = (int)GITEM.Bomb3;
            szFile = "Event_3";
        }

        if (RR >= 70 && RR < 90)
        {
            m_nItem = (int)GITEM.Bomb4;
            szFile = "Event_4";
        }

        if (RR >= 90)
        {
            m_nItem = (int)GITEM.Bomb5;
            szFile = "Event_5";
        }

        Load(szFile);
        
        m_gPath = CWResourceManager.Instance.GetPath(szFile, 1);
        m_gPath.DOPlay();
        NTeam = 1;//NPC 
        SetTag("AI");
        m_ObjectType = CWOBJECTTYPE.EVENT;
*/

    }
    public override void CalPower()
    {
        

        base.CalPower();
        int RR = Random.Range(1, 10);

        KPower.m_nHp =(int)((float)CWHero.Instance.GetDamage()* RR);
    }
    public override void SetDie()
    {
        base.SetDie();

        RewardKillDlg.Instance.Show(m_nItem);

    }
}
