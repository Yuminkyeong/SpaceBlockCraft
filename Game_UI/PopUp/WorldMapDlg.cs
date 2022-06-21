using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
using CWStruct;
using CWEnum;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
public class WorldMapDlg : WindowUI<WorldMapDlg>
{
    public GameObject[] PlanetMaps;
    public GameObject StarMap;
    public GameObject []m_gPos;
    protected override void _Open()
    {

        foreach (var v in m_gPos) v.SetActive(false);
        
        
        for(int i=0;i<6;i++)
        {
            int num =( CWHeroManager.Instance.m_nPlanetID - 1)%6;
            if(num==i)
            {
                m_gPos[i].SetActive(true);
            }
            else
            {
                m_gPos[i].SetActive(false);
            }
            
        }



        m_nGroupType = 1;
        FirstOpenPlanetSystem();
        MakePlanetSystem();
        base._Open();
    }
    public override void Close()
    {
        base.Close();
    }
    public void EnterStarSystem(int starIndex)
    {
        for(int i = 0; i<PlanetMaps.Length; i++)
        {
            bool condition = (starIndex == i) ? true : false;
            PlanetMaps[i].SetActive(condition);
        }      
        StarMap.SetActive(false);
    }
    private void MakePlanetSystem()
    {
        for (int i = 0; i < PlanetMaps.Length; i++)
        {
            for(int j = 0; j <6; j++)
            {
                GameObject PlanetName = CWLib.FindChild(PlanetMaps[i].gameObject, string.Format("Planet_{0}", j + 1));

                string tempString = CWTableManager.Instance.GetTable("블록이름,스토리 - 행성", "stage", i * 6 + j + 1) +" "+ CWTableManager.Instance.GetTable("블록이름,스토리 - 행성", "name", i * 6 + j + 1);
                PlanetName.GetComponentInChildren<CWText>().text = tempString;
                WorldMapBtn ww=PlanetName.GetComponent<WorldMapBtn>();
                if(ww==null)
                {
                    ww=PlanetName.AddComponent<WorldMapBtn>();
                }
                ww.m_nPlanet = i * 6 + j + 1;



            }         
        }
    }
    private void FirstOpenPlanetSystem()
    {
        InactiveAllPlanetSystem();
        GameObject nowPlanetSystem = CWLib.FindChild(this.gameObject, string.Format("PlanetMap{0}", Space_Map.Instance.m_nSolaID - 1));
        nowPlanetSystem.SetActive(true);
    }
    private void InactiveAllPlanetSystem()
    {
        GameObject visible = this.transform.GetChild(0).gameObject;
        for (int i = 0; i<5; i++)
        {
            visible.transform.GetChild(i).gameObject.SetActive(false);
        }      
    }

    public void ShowNextUpdateMessage()
    {
        NoticeMessage.Instance.Show("다음 업데이트를 기대해주세요!");
    }
}
