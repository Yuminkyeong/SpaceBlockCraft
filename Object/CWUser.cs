using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using CWUnityLib;
using Newtonsoft.Json.Linq;
using CWEnum;
using CWStruct;
     
/*
 
 * */
public class CWUser : CWAirObject
{


    public bool m_bReceiveEnd;

    public int m_nRanking;
    public int m_nNHP;
    public int m_nHP;
    public int m_nDamage;
    public int m_nKillcount;

    public int m_nFakeUser;
    public int m_nCharNumber;
    bool m_bReceived = false;
    public bool m_bDont3DUIInfo=false; // 3DUI 
    public bool IsReceiveEndTask()
    {
        return m_bReceived;
    }


    public bool m_bConnectuserflag;

    public override bool IsDrone()
    {
        return true;
    }

    protected virtual void ReceiveEnd()
    {
        m_bReceiveEnd = true;
        if (UserType == CWEnum.USERTYPE.USER)
        {
            string szstr = CWLocalization.Instance.GetLanguage("{0}님이 접속하였습니다");
            UserAppearDlg.Instance.Show(string.Format(szstr, name));
        }
        SetTeam();
       
    }
    protected override void SetObjectType()
    {
        m_ObjectType = CWOBJECTTYPE.USER;
    }
    List<WEAPONSLOT> m_kWeaponSlot = new List<WEAPONSLOT>();
    public void UpdateWeaponSlot(JArray ja)
    {
        m_kWeaponSlot.Clear();

        for (int i = 0; i < ja.Count; i++)
        {
            CWJSon jj = new CWJSon((JObject)ja[i]);

            WEAPONSLOT ws = new WEAPONSLOT();
            ws.DamageLv = jj.GetInt("Damage");
            ws.SpeedLv = jj.GetInt("Speed");
            ws.RangeLv = jj.GetInt("Range");

            m_kWeaponSlot.Add(ws);
        }
        PowerSetting();
    }

    public int GetWeaponDamageLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot - 1;
        return m_kWeaponSlot[num].DamageLv;
    }
    public int GetWeaponRangeLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot - 1;
        return m_kWeaponSlot[num].RangeLv;
    }

    public int GetWeaponSpeedLevel(int nSlot)// 1부터 시작 
    {
        if (nSlot <= 0) return 0;
        if (nSlot > m_kWeaponSlot.Count) return 0;
        int num = nSlot - 1;
        return m_kWeaponSlot[num].SpeedLv;
    }
    public override void WeaponSetting()
    {
        base.WeaponSetting();
        PowerSetting();
    }



    protected virtual void PowerSetting()
    {
        float frate = UnityEngine.Random.Range(0.5f, 2.5f);

        KPower.m_nDamage = m_nDamage;
        KPower.m_nHp = m_nHP;
        for (int i = 0; i < m_kWeapon.Count; i++)
        {
            m_kWeapon[i].m_nSlot = i + 1;
            m_kWeapon[i].m_nDamage = KPower.m_nDamage / m_kWeapon.Count;

            int nLevel = GetWeaponDamageLevel(m_kWeapon[i].m_nSlot) ;
            int nRLevel = GetWeaponRangeLevel(m_kWeapon[i].m_nSlot);
            int nSpeedLevel = GetWeaponSpeedLevel(m_kWeapon[i].m_nSlot);


            m_kWeapon[i].m_fSpeed = 60 + 15 * frate;
           
        }


    }

    void ReceiveUserData(JObject jData)
    {
        
       
        if(jData["Result"].ToString()=="ok")
        {

            CWJSon jSon = new CWJSon(jData);
            int nID = jSon.GetInt("id");
            JToken jt =  jData["BlockData"];
            if (jData["Name"] == null) return;
            CWUser kUser = (CWUser)CWUserManager.Instance.GetUser(nID);
            if (kUser == null)
            {
                return;
            }
            kUser.name = jData["Name"].ToString();

            if(jt==null)
            {
                TextAsset aa= CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                kUser.SetBuffer(aa.bytes);
            }
            else
            {
                byte [] buffer= jt.ToObject<byte[]>();
                if(buffer!=null)
                {
                    if (buffer.Length < 10)
                    {
                        TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                        kUser.SetBuffer(aa.bytes);
                    }
                    else
                    {
                        kUser.SetBuffer(buffer);
                    }

                }
                else
                {
                    TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
                    kUser.SetBuffer(aa.bytes);

                }

            }
                

            kUser.m_nGrade = jSon.GetInt("Grade");// jData["Ranking"].Value<int>();
            kUser.m_nRankPoint = jSon.GetInt("RankPoint");
            kUser.m_nRanking = jSon.GetInt("LastRank");
            if(kUser.m_nRanking==0)
            {
                kUser.m_nRanking = CWLib.Random(100,300);
            }

            kUser.m_nFakeUser = jSon.GetInt("FakeUser");
            kUser.m_nCharNumber = jSon.GetInt("CharNumber");

            UpdateWeaponSlot((JArray)jData["WeaponSlot"]);

            kUser.gameObject.SetActive(true);
            kUser.BaseCreate();


        }
        else
        {
            CWUser kUser = (CWUser)CWUserManager.Instance.GetUser(m_nID);
            if (kUser == null)
            {
                return;
            }
            TextAsset aa = CWResourceManager.Instance.GetAirObject(CWGlobal.StartAir());
            kUser.SetBuffer(aa.bytes);
            kUser.gameObject.SetActive(true);
            kUser.BaseCreate();
        }
    }



    public void BaseCreate()
    {
        base.Create(m_nID);
        CWUnityLib.DebugX.Log("유저 생성" + m_nID.ToString());
        ReceiveEnd();
        m_bReceived = true;

    }

    public override void Create(int nID)
    {
        UserType = CWEnum.USERTYPE.USER;
        m_nID = nID;
        if (!m_kJSon.IsLoad())
        {
            if(CWSocketManager.Instance)
                CWSocketManager.Instance.AskUserData(nID, ReceiveUserData, "ReceiveUserData");
        }
        m_nHeightInfo = 10;
    }

    public override void SetEnemyBar(bool bEnemy)
    {
        m_bTeamSetting = true;

        SetTeamColorBar(!bEnemy);


    }

  

    void Resenuser()
    {
        gameObject.SetActive(true);
    }
    
    // 휴식 알림!
    public override void SetRest(int nmode)
    {

        base.SetRest(nmode);
    }
   
 
    public override void SetDie()
    {
        CWUserManager.Instance.CloseUser(m_nID);
        CWEffectManager.Instance.GetEffect(GetHitPos(), "Ef_ExplosionParticle_01");
    }

    protected override string GetUI()
    {
        return "UserUI";
    }
    public override void CalPower()
    {
        //base.CalPower();
        //if(m_nHP>0)
        //{
        //    KPower.FhpRate = (float)m_nNHP / m_nHP;
        //    KPower.m_nHp = m_nHP;
        //}

    }
    public override void PVPStart()
    {
        
        BaseUnitUI ui = GetComponentInChildren<BaseUnitUI>();
        if (ui)
        {
            ui.SetWarmode();
        }

    }
    public override void SetEmoticon(Emoticon eType)
    {
        Vector3 vPos = transform.position;
        vPos.y += 6;
        CWPoolManager.Instance.GetParticle(vPos, eType.ToString());
    }
    public override void SetMusukBar(int state)
    {
        BaseUnitUI ui = GetComponentInChildren<BaseUnitUI>();
        if (ui)
        {
            ui.SetMusukBar(state);
        }


    }


    // 절대로 깜박이지 않는 데이타 작업 
    #region 유저 움직임 

    float m_fMoveSpeed = 1f;
    float m_fMoveYaw = 0;
    Vector3 m_vMovePos = Vector3.zero;

    public override void CopyTransPos(Vector3 vPos, float fYaw)
    {
        //vPos.y = ;

        float fdist = Vector3.Distance(transform.position, vPos);
        if ( fdist > 80f)
        {
            transform.position = vPos;
            m_vMovePos = vPos;
            return;
        }
        m_vMovePos = vPos;
        m_fMoveYaw = fYaw;
    }

    private void Update()
    {
        Vector3 vPos = transform.position;

        if (m_vMovePos == Vector3.zero)
        {
            return;
        }
            
        
        
        vPos = Vector3.Lerp(transform.position, m_vMovePos, 1.0f - Mathf.Exp(-m_fMoveSpeed*Time.deltaTime));
        SetPos(vPos);
        float fyaw = Mathf.LerpAngle(transform.eulerAngles.y, m_fMoveYaw,1.0f - Mathf.Exp(-m_fMoveSpeed * Time.deltaTime));
        SetYaw(fyaw);
    }




    #endregion

    #region 가상 맟춤

    public virtual void SetFakePower(int hp,int damage)
    {
    }
    // HP에 맞게 블록을 맟추어 준다!
    protected virtual void MakeFakeHP()
    {
    }
    #endregion

    #region 올드 파일 바꿈
    public enum GITEM_OLD
    {
        NONE = 0,
        tree = 1,
        CoreEngine = 2,
        bricks = 3,
        stone1 = 4,
        plastic = 5,
        wood = 6,
        brick = 7,
        concrete = 8,
        asphalt = 9,
        stone2 = 10,
        limestone = 11,
        granite = 12,
        marble = 13,
        crystal = 14,
        tempglass = 15,
        Copper_1 = 16,
        Copper_2 = 17,
        Copper_3 = 74,
        Copper_4 = 75,
        Iron_1 = 18,
        Iron_2 = 19,
        Iron_3 = 20,
        Iron_4 = 23,
        glass = 21,
        nikel_1 = 24,
        nikel_2 = 76,
        nikel_3 = 77,
        nikel_4 = 25,
        titanium_1 = 22,
        titanium_2 = 78,
        titanium_3 = 79,
        titanium_4 = 80,
        valyrian = 26,
        Drone_w_1 = 27,
        Drone_w_2 = 28,
        Drone_w_3 = 29,
        Vibranium = 30,
        Gundarium = 31,
        Turret_w_1 = 32,
        Turret_w_2 = 33,
        Turret_w_3 = 34,
        EmptyBlock = 35,
        sapphire = 36,
        Gold = 37,
        Ruby = 38,
        Emerald = 39,
        Diamond = 40,
        Red = 41,
        Blue = 42,
        Orange = 43,
        Yellow = 44,
        Green = 45,
        indigo = 46,
        Purple = 47,
        white = 48,
        Black = 49,
        Skyblue = 50,
        LightGreen = 51,
        Gray = 52,
        DarkGray = 53,
        LightGray = 54,
        Pink = 55,
        scarlet = 56,
        Lightbluegreen = 83,
        DarkBlue = 95,
        Darkyellowgreen = 84,
        Bluepurple = 85,
        magenta = 86,
        Cyanblue = 87,
        Lightgreencyan = 88,
        Blackgreen = 89,
        DarkBrown = 90,
        Brown = 91,
        amber = 92,
        Deeppink = 93,
        Darkgreen = 94,
        Darkpurple = 96,
        Buster = 61,
        weapon = 62,
        Bomb1 = 63,
        Bomb2 = 64,
        Bomb3 = 65,
        Bomb4 = 66,
        Bomb5 = 67,
        grass = 70,
        dirt = 71,
        sand = 72,
        stone = 73,
        Amethyst = 81,
        Topaz = 82,
        DarkRed = 97,
        LightRed = 98,
        DarkYellow = 99,
        LightSkyblue = 100,
        DarkSkyblue = 101,
        glassGreen = 102,
        Lightblue = 103,
        LightYellowGreen = 104,
        LightYellow = 105,
        Ticket = 106,
        Copper_5 = 107,
        Copper_6,
        Iron_5,
        Iron_6,
        nikel_5,
        nikel_6,
        titanium_5,
        titanium_6,
        valyrian_2,
        valyrian_3,
        valyrian_4,
        valyrian_5,
        valyrian_6,
        Gundarium_2,
        Gundarium_3,
        Gundarium_4,
        Gundarium_5,
        Gundarium_6,
        Vibranium_2,
        Vibranium_3,
        Vibranium_4,
        Vibranium_5,
        Vibranium_6,
        MAX = 4096,
    }
    void FindOldfile()
    {

        List<int> kTemp = new List<int>();

        foreach (var v in m_kData)
        {
            if (v.Key == 61) continue;
            if (v.Key == 62) continue;
            if (v.Key == 216) continue;
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            if (nData.type != "shipblock")
            {
                kTemp.Add(v.Key);
            }
        }
        foreach (var v in kTemp)
        {

            BlockData bb = m_kData[v];
            bb.nBlock= (int)GITEM.solidinium;
            m_kData[v] = bb;
            //m_kData[v].nBlock = (int)GITEM.solidinium;
        }


    }
    #endregion
    protected void ChangeBlock()
    {
        if (m_kData.Count == 0) return;

        FindOldfile();
        int nChangeBlock = (int)GITEM.stone;
    
        List<int> kTemp = new List<int>();
        
        int ahp = 0;
        foreach (var v in m_kData)
        {
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            if (nData.type == "shipblock")
            {
                kTemp.Add(v.Key);
                ahp += nData.hp;
            }
        }
        if(ahp >= KPower.m_nHp)
        {
            for(int i=0;i< kTemp.Count; i++)
            {
                int num = kTemp[i];
                UpdateBlock(num, (int)GITEM.tree);
            }
            LoadMeshFunc();
            return;
        }
        if (nChangeBlock == 0) return;
        int allhp = 0;
        GITEMDATA nData2= CWArrayManager.Instance.GetItemData(nChangeBlock);
        foreach (var v in kTemp)
        {
            UpdateBlock(v, nChangeBlock);
            allhp += nData2.hp;
            if (allhp > KPower.m_nHp) break;
        }
        if(allhp< KPower.m_nHp)// 모자란다!!
        {
            nChangeBlock= CWGlobal.g_BestIron;
            GITEMDATA nData3 = CWArrayManager.Instance.GetItemData(nChangeBlock);
            int hh = nData3.hp - nData2.hp;
            foreach (var v in kTemp)
            {
                UpdateBlock(v, nChangeBlock);
                allhp += hh;// 교체하는 차이값
                if (allhp > KPower.m_nHp) break;
            }

        }


        LoadMeshFunc();
    }

    public override void CheckCharAddBlock()
    {
        if (!CheckCharBlock())
        {

            for (int i = 0; i < 30; i++)
            {
                int y = 31 - i;
                if (GetBlock(16, y, 16) > 0)
                {
                    int yy = y + 1;
                    AddBlock(16, yy, 16, (int)GITEM.charblock);
                    m_nCharNumber = CWLib.Random(1, 5);
                    break;
                }
            }

        }
    }

    protected override GameObject CharblockAttach()
    {
        if (m_nCharNumber == 0) m_nCharNumber = 1;
         GameObject gg = new GameObject();
        CharBody cb = CWResourceManager.Instance.GetCharBody(m_nCharNumber);
        cb.transform.parent = gg.transform;
        cb.transform.localPosition = new Vector3(0, -1.3f, 0);
        cb.transform.localRotation = new Quaternion();
        BoxCollider bb = gg.AddComponent<BoxCollider>();
        bb.size = Vector3.one;
        bb.isTrigger = true;
        return gg;
    }

}
