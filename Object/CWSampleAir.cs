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

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

public class CWSampleAir : CWAirObject
{
    #region 오버로드
    protected override void BodyCreate()
    {
        
    }
    public override void CalPower()
    {
        
    }
    protected override bool LoadMeshFunc()
    {
        return true;
    }

    protected override void CreatePower()
    {
        
    }



    public override void Load(string szName)
    {
        base.Load(szName);
        MakeOrderList();
    }
    #endregion

    Vector3Int m_Start = new Vector3Int(15, 6, 13); // 시작좌표

    public List<int> m_kOrder = new List<int>();
    
    void SubRegData(int sx,int sy,int sz)
    {
        int Index = (sz ) * SELLWIDTH * SELLWIDTH + (sy ) * SELLWIDTH + sx ;
        if(!m_kOrder.Exists(x=>x==Index))
        {
            m_kOrder.Add(Index);
        }
        for (int z=-1;z<=1;z++)
        {
            for (int y = -1; y <= 1; y++)
            {
                for (int x = -1; x <= 1; x++)
                {
                    if (x == 0 && y == 0 && z == 0) continue;

                    int tx = sx + x;
                    int ty = sy + y;
                    int tz = sz + z;
                    int num = tz * SELLWIDTH * SELLWIDTH + ty * SELLWIDTH + tx;
                    if (m_kData.ContainsKey(num))
                    {
                        SubRegData(tx, ty, tz);
                    }
                }
            }
        }
    }

    void MakeOrderList()
    {

          
    }
}
