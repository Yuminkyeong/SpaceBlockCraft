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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;

using CWUnityLib;
using CWStruct;
using CWEnum;
using DG.Tweening;
/*
 * 
 * 비행유닛을 만든다 
 * */

public class CWAirObject : CWBuildObject
{
    #region 변수


    public int m_nHeightInfo=4;

    public bool m_bMeshCollider;

    protected GameObject m_gHitDummy;
    protected GameObject m_gStageBox;
    public bool m_bDontProgresbar = false;
    public bool m_bUnit = false;

    public enum SHAPEORDER { _DELETEBOX, _BOX, _WEDGE, _WEDGE_2, _WEDGE_3, _WEDGE_4, _WEDGE_5, _WEDGE_6, _WEDGE2, _WEDGE2_2, _WEDGE2_3, _WEDGE2_4, _WEDGE2_5, _WEDGE2_6 };
    public struct SHIPDATA
    {
        public int x;
        public int y;
        public int z;
        public int nItemNumber;
        public int nShape;
    };

    const int __OLDSHIPMANAGERVERSION_ = 104;
    const int __SHIPMANAGERVERSION_ = 200;

    // 지워진 파일
    protected Dictionary<int, int> m_kDelFile = new Dictionary<int, int>();

    protected List<CWItemObject> m_kWeapon = new List<CWItemObject>();
    //
    public int m_nNumber;// 비행기 번호 
    public bool m_bLoading = false;

    

    public float m_fUseEnergy;// 초당 소모되는 에너지 
    public float m_fWeaponUseEnergy;// 발사 마다 소모되는 에너지 

    int m_nSpeedCount;
    int m_nWeaponCount;
    int m_nBlockCount;

    protected GameObject m_gTail;


    public Vector3 m_vStart;
    public Vector3 m_vEnd;

    public Vector3 m_vSize;
    public Vector3 m_vCenter;

    public int m_nADDDamage;// 추가 데미지 , 아이템에서 나옴

    public int NSpeedCount
    {
        get
        {
            return m_nSpeedCount;
        }

        set
        {
            m_nSpeedCount = value;
        }
    }

    public int NWeaponCount
    {
        get
        {
            return m_nWeaponCount;
        }

        set
        {
            m_nWeaponCount = value;
        }
    }
    public int NBlockCount
    {
        get
        {
            return m_nBlockCount;
        }

        set
        {
            m_nBlockCount = value;
        }
    }


    #endregion
    #region 초기작업

    public CWAirObject()
    {
        ///SELLWIDTH = 32;

    }
    public override void SetRest(int nmode)
    {
        base.SetRest(nmode);
       
        
    }

    protected virtual string GetUI()
    {
        return "InfoUI";
    }


    protected virtual void MakeProgressBar()
    {
        if (m_bDontProgresbar) return;
        if(m_gTempdir==null)
        {
            return;
        }
        if(m_gInfoView)
        {
            Vector3 vPos = m_vCenter;
            vPos.y += (m_vSize.y + m_nHeightInfo);
            m_gInfoView.transform.parent = m_gTempdir.transform;
            m_gInfoView.transform.localPosition = vPos;

            return;
        }
        GameObject gPress = CWResourceManager.Instance.GetPrefab(GetUI());
        if (gPress)
        {
            Vector3 vPos = m_vCenter;
            vPos.y += (m_vSize.y+ m_nHeightInfo);
            gPress.transform.SetParent(m_gTempdir.transform);
            gPress.transform.localPosition = vPos;
            gPress.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            gPress.tag = gameObject.tag;
            m_gInfoView = gPress;

        }
    }


    #endregion

    #region 블록만들기

    // 로컬 데이타 위치
    public int GetBlockLocal(int x,int y,int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nBlock;
        }
        return 0;
    }
    // 비행기 크기 구하기
    public void CallSize()
    {
        if(m_bSaveSphereCollider)
        {
            return;
        }
        if(m_kData.Count==0)
        {
            m_vCenter = Vector3.zero;
            m_vSize = Vector3.zero;
            return;
        }
        float fmaxX = 0;
        float fmaxY = 0;
        float fmaxZ = 0;

        float fminX = 100000;
        float fminY = 100000;
        float fminZ = 100000;
        foreach (var v in m_kData)
        {
            float x = GetSellX(v.Key) - SELLWIDTH / 2;
            float y = GetSellY(v.Key);
            float z = GetSellZ(v.Key) - SELLWIDTH / 2;
            if (x < fminX)
            {
                fminX = x;
            }
            if (y < fminY)
            {
                fminY = y;
            }
            if (z < fminZ)
            {
                fminZ = z;
            }
            if (x > fmaxX)
            {
                fmaxX = x;
            }
            if (y > fmaxY)
            {
                fmaxY = y;
            }
            if (z > fmaxZ)
            {
                fmaxZ = z;
            }
        }
        float fDelX = Mathf.Abs(fmaxX - fminX);
        float fDelZ = Mathf.Abs(fmaxZ - fminZ);
        float fDelY = Mathf.Abs(fmaxY - fminY);
        m_vCenter.x = fminX + fDelX / 2 + 0.5f;
        m_vCenter.y = fminY + (fmaxY - fminY) / 2;
        m_vCenter.z = fminZ + fDelZ / 2 + 0.5f;

        m_vStart = new Vector3(fminX, fminY, fminZ);
        m_vEnd = new Vector3(fmaxX, fmaxY, fmaxZ);

        m_vSize = new Vector3(fDelX, fDelY, fDelZ);

        FixCenter();
        



    }
    protected override void FixCenter()
    {
     //   if (CWGlobal.g_bEditmode) return;
      //  m_gCenterObject.transform.localPosition = new Vector3(-m_vCenter.x, -m_vCenter.y, -m_vCenter.z);
    }
    public virtual void CalPower()
    {
        int nHP = 0;
        int nDamage = 0;
        m_fUseEnergy = 0;

        
        
        NSpeedCount = 0;
        NWeaponCount = 0;
        NBlockCount = 0;
        m_fWeaponUseEnergy = 0;

        
        int nPrice = 0;
        m_nPrice = 0;
        m_nADDDamage = 0;
        foreach (var v in m_kData)
        {
            GITEMDATA nData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            m_nADDDamage += nData.Damage;


            if (nData.type == "Buster")
            {
                NSpeedCount++;
            }
                
            if (nData.type == "weapon")
            {
                int nLevel= v.Value.nBlock/256+1;
                int ds  = CWHeroManager.Instance.GetWeaponDamage(m_nSelectWeaponType, nLevel);
                nDamage += ds;
                NWeaponCount++;
                
            }

            if (nData.type == "shipblock")
            {
                NBlockCount++;
                
            }

            nPrice += nData.price; // 

            nHP += nData.hp;
            
            
        }


        m_nPrice = nPrice;// (int)((float)nPrice*0.0025f);

        CWPower cp = gameObject.GetComponent<CWPower>();
        if (cp == null)
        {
            cp= gameObject.AddComponent<CWPower>();
        }
        cp.m_nHp = nHP;
        cp.m_nDamage = nDamage;




    }
    // 캐릭터 포지션
    public Vector3 GetChiricPos() // local
    {
        foreach (var v in m_kData)
        {
            if (v.Value.ItemID == (int)GITEM.charblock)
            {
                float x = GetSellX(v.Key) ;
                float y = GetSellY(v.Key);
                float z = GetSellZ(v.Key) ;

                return new Vector3(x, y, z);
            }
        }

        return Vector3.zero;
    }



    public virtual bool IsDelFile(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kDelFile.ContainsKey(num))
        {
            return true;
        }
        return false;

    }
    protected override int ConvertBlock(int x, int y, int z)
    {
        int nItem = _GetBlock(x, y, z);
        if (IsDelFile(x, y, z)) return 0; // 지워진 파일이라면

        if (nItem > 0)
        {
            return CWArrayManager.Instance.GetBlockFromItem(nItem);
        }
        return 0;
    }

    
    public    delegate int CBChangeBlock(int x, int y, int z, int nBlock);
    public CBChangeBlock _ChangeBlock;

    protected override int ChangeBlock(int x,int y,int z, int nBlock)
    {
        //// 과거 파일 검사 
        //if (nBlock > 62) nBlock = (int)GITEM.stone1;

#if UNITY_EDITOR

        if (_ChangeBlock != null)
        {
            return _ChangeBlock(x,y,z, nBlock);
        }
#endif
        return nBlock;
    }
    int ConvertInt(uint uval, int nsize, int pos)
    {
        uint bit = (uint)Math.Pow(2f, (double)nsize);
        bit = bit - 1;
        return (int)((uval >> pos) & bit);
    }
    void ConvertData(uint dd, ref SHIPDATA ndata)
    {
        int p = 0;
        ndata.x = ConvertInt(dd, 5, p); p += 5;
        ndata.y = ConvertInt(dd, 4, p); p += 4;
        ndata.z = ConvertInt(dd, 5, p); p += 5;
        ndata.nItemNumber = ConvertInt(dd, 8, p); p += 8;
        ndata.nShape = ConvertInt(dd, 4, p); p += 4;
    }
    

    BLOCKSHAPE ConvetShape(int n)
    {
        SHAPEORDER ss = (SHAPEORDER)n;
        if (ss == SHAPEORDER._WEDGE)
        {
            return BLOCKSHAPE.SLANT_1;
        }
        if (ss == SHAPEORDER._WEDGE_2)
        {
            return BLOCKSHAPE.SLANT_3;
        }
        if (ss == SHAPEORDER._WEDGE_3)
        {
            return BLOCKSHAPE.SLANT_2;
        }
        if (ss == SHAPEORDER._WEDGE_4)
        {
            return BLOCKSHAPE.SLANT_4;
        }
        if (ss == SHAPEORDER._WEDGE_5)
        {
            return BLOCKSHAPE.SLANT_1;
        }

        return BLOCKSHAPE.NORMAL;
    }
    protected override BLOCKSHAPE _GetShape(int x, int y, int z, int nBlock)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return (BLOCKSHAPE)m_kData[num].nShape;
        }
        return 0;


    }




#endregion
#region 블록 접근관련

    // 블록이 하나라도 있는가?
    public bool  DeleteAllowBlock()
    {
        int nCount = 0;
        foreach(var v in m_kData)
        {
            int nItem= v.Value.nBlock;
            if(nItem==(int)GITEM.CoreEngine)
            {
                return true;// 코어 블록이 있으면 다 지워도 됨
            }
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            if(gData.type== "shipblock")
            {
                nCount++;
            }
            if(nCount>=2)
            {
                //2개 이상이어야 지울 수 있다
                return true;
            }
        }

        return false;

    }
    public int GetItem(int nIndex)
    {
        if (m_kData.ContainsKey(nIndex))
        {
            return m_kData[nIndex].nBlock;
        }

        return 0;
    }
    public Vector3Int FindCenterBlock(string sztype)
    {
        for (int i = 0; i < 100; i++)
        {
            int tx = 0, tz = 0;
            CWCircleData.GetData(i, ref tx, ref tz);
            int x = SELLWIDTH / 2 + tx;
            int z = SELLWIDTH / 2 + tz;

            for (int y = SELLWIDTH; y >= 0; y--)
            {
                int item = GetBlock(x + tx, y, z + tz);
                if (item > 0)
                {
                    GITEMDATA gData = CWArrayManager.Instance.GetItemData(item);
                    if (gData.type == sztype)
                    {
                        return new Vector3Int(x + tx, y, z + tz);
                    }

                }

            }
        }
        return Vector3Int.zero;

    }
    public override void AddBlock(int x, int y, int z, int nItem, int nShape=0,int nColor=0)
    {
        
        base.AddBlock(x, y, z, nItem, nShape, nColor);
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        
        ItemObjectAttach(nItem%256, num,nItem/256);
        CalPower();
        CallSize();

    }
    
   
    public int GetBlock_AirEdit(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nBlock;
        }

        return 0;
    }

    

    public virtual void DelHpBlock(int num)
    {

    }
    public   void DelBlock(int num)
    {
        m_bUpdated = true;
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        foreach (var v in array)
        {
            if (v.m_nPosNumber == num)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.m_ID);
                if (gData.type == "weapon")
                {
                    m_kWeapon.Remove(v);
                }
                GameObject.Destroy(v.gameObject);
                break;
            }
        }
        m_kData.Remove(num);
        LoadObjectFunc();
        


    }
    public bool DelBlock_AirEdit(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;

        if(!m_kData.ContainsKey(num))
        {
            Debug.Log("aaa");
            return false;
        }
        BlockData kData= m_kData[num];
        m_kData.Remove(num);
        float ft = Time.realtimeSinceStartup;
        if (!IsAllLinkBlock(x,y,z))
        {
            // 원상복귀
            m_kData.Add(num, kData);
            return false;
        }
        float ft2 = Time.realtimeSinceStartup;
        float fff = ft2 - ft;
        Debug.Log("delay " + fff.ToString());


        m_bUpdated = true;
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        foreach (var v in array)
        {
            if (v.m_nPosNumber == num)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.m_ID);
                if (gData.type == "weapon")
                {
                    m_kWeapon.Remove(v);
                }
                GameObject.Destroy(v.gameObject);
                break;
            }
        }
        
        CalPower();
        return true;
        
    }
#endregion
#region BLOCKEDIT

   
    // 자동 모양 바꿈
    public void AutoShape()
    {

    }
    // 모든 블록을 옮긴다.
    public void AllMoveInven()
    {


        foreach (var v in m_kData)
        {
            CWInvenManager.Instance.AddItem(v.Value.nBlock, 1);
        }

    }

#endregion
#region 파일로드저장

    public void CopyBuffer(byte[] buffer)
    {
        SetBuffer(buffer);
        Load(name);
    }

    protected int CampareSort(int a, int b)
    {
        int ax = GetSellX(a);
        int ay = GetSellY(a);
        int az = GetSellZ(a);
        int bx = GetSellX(b);
        int by = GetSellY(b);
        int bz = GetSellZ(b);

        Vector3 v1 = new Vector3(ax, ay, az);
        Vector3 v2 = new Vector3(bx, by, bz);

        float fdist1 = Vector3.Distance(v1, new Vector3(16, 4, 16));
        float fdist2 = Vector3.Distance(v2, new Vector3(16, 4, 16));

        return (int)(fdist1 - fdist2);


    }
    public override bool LoadOldFile(MemoryStream ms)
    {
        byte[] bb = new byte[4];
        ms.Read(bb, 0, 4);
        int nVersion = BitConverter.ToInt32(bb, 0);
        if (nVersion == __OLDSHIPMANAGERVERSION_)
        {
            ms.Read(bb, 0, 4);
            int tcnt = BitConverter.ToInt32(bb, 0);
            for (int i = 0; i < tcnt; i++)
            {
                ms.Read(bb, 0, 4);
                uint udata = BitConverter.ToUInt32(bb, 0);

                SHIPDATA nData = new SHIPDATA();
                ConvertData(udata, ref nData);
                int nBlock = OldItemConvert(nData.nItemNumber);
                int nShape = (int)ConvetShape((int)nData.nShape);
                if (nBlock > 0)
                {
                    _AddBlock(nData.x, nData.y, nData.z, nBlock, nShape,0,0);
                }

            }

        }
        else if (nVersion == __SHIPMANAGERVERSION_)
        {
            ms.Read(bb, 0, 4);
            int tcnt = BitConverter.ToInt32(bb, 0);

            for (int i = 0; i < tcnt; i++)
            {
                ms.Read(bb, 0, 4);
                uint udata = BitConverter.ToUInt32(bb, 0);

                SHIPDATA nData = new SHIPDATA();
                ConvertData(udata, ref nData);
                int nBlock = OldItemConvert(nData.nItemNumber);
                int nShape = (int)ConvetShape((int)nData.nShape);
                _AddBlock(nData.x, nData.y, nData.z, nBlock, nShape,0,0);
            }

        }

        return true;

    }
    protected virtual int ChangeItem(int nID)
    {
        return nID;
    }
    protected virtual void OnWeaponAttach(CWItemObject cs)
    {

    }
    protected virtual string FindMissile(int wtype, int nLevel)
    {
        //if (nLevel == 0) nLevel = 1;
        nLevel += 1;

        string sz=CWArrayManager.Instance.GetMissile(m_nSelectWeaponType, nLevel); 
        if(CWLib.IsString(sz))
        {
            return sz;
        }
        return "gun_1";
    }
    protected virtual GameObject CharblockAttach()
    {
        return null;
    }
    protected void ItemObjectAttach(int nItem,int num,int nLevel)
    {
        GITEMDATA nData = CWArrayManager.Instance.GetItemData(nItem);
        if (nData.type == "weapon" || nData.type == "코어엔진" || nData.type == "Buster" || nData.type == "charblock")
        {
            GameObject gg =null;
            if (nData.type== "코어엔진")
            {
                gg = CWResourceManager.Instance.GetCoreEngineObject(NLevel);
            }
            else
            {
                if(nData.type== "charblock")
                {
                    gg= CharblockAttach();

                }
                else
                {
                    gg = CWResourceManager.Instance.GetItemObject(nData.nID);
                }
                
            }
            if (gg == null) return;

            //MeshCollider cc = gg.GetComponent<MeshCollider>();
            //if (cc == null)
            //{
            //    cc = gg.AddComponent<MeshCollider>();
            //    cc.sharedMesh = gg.GetComponent<MeshFilter>().sharedMesh; ;
            //}


            gg.transform.parent = m_gItemBody.transform;
            float x = GetSellX(num) - SELLWIDTH / 2;
            float y = GetSellY(num);
            float z = GetSellZ(num) - SELLWIDTH / 2;

            gg.transform.localPosition = new Vector3(x, y, z);
            gg.transform.localScale = Vector3.one;
            gg.transform.localEulerAngles = Vector3.zero;

            gg.name = nData.szname;

            

            CWLib.SetGameObjectLayer(gg, gameObject.layer);
            CWLib.SetGameObjectTag(gg, gameObject.tag);

            CWItemObject cs = gg.AddComponent<CWItemObject>();
            cs.m_ID = nData.nID;
            cs.m_kItem = CWArrayManager.Instance.GetItemData(nData.nID);
            cs.m_nPosNumber = num;
            cs.m_nLevel = nLevel+1;

            if (cs.m_ID==(int)GITEM.Gun)
                m_nSelectWeaponType = 1;
            if (cs.m_ID == (int)GITEM.Missile)
                m_nSelectWeaponType = 2;
            if (cs.m_ID == (int)GITEM.Laser)
                m_nSelectWeaponType = 3;

            
            
            if (nData.type == "weapon")
            {
                cs.m_nWeaponType = m_nSelectWeaponType;
                cs.m_szMissile = FindMissile(m_nSelectWeaponType, nLevel);
                OnWeaponAttach(cs);
                m_kWeapon.Add(cs);
            }

        }
        
        

    }
    override public void UpdateObject()
    {

        GameObject.Destroy(m_gItemBody);
        m_gItemBody = new GameObject();
        m_gItemBody.name = "ItemBody";
        m_gItemBody.tag = gameObject.tag;
        

        if(m_gCenterObject)
        {
            m_gItemBody.transform.parent = m_gCenterObject.transform;
            m_gItemBody.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
            m_gItemBody.transform.localScale = Vector3.one;
            m_gItemBody.transform.localEulerAngles = Vector3.zero;
        }

        m_kWeapon.Clear();
        foreach (var v in m_kData)
        {
            int nItem = ChangeItem(v.Value.nBlock);
            ItemObjectAttach(nItem%256, v.Key, nItem /256);
        }

        LoadMeshFunc();
        WeaponSetting();
    }
    public virtual void WeaponSetting()
    {
     

    }
    public override void CopyObject(CWObject kObject)
    {
        CWAirObject kAir= (CWAirObject)kObject;
        CopyBuffer(kAir.GetBuffer());
    }

    public virtual void CheckCharAddBlock()
    {
        List<int> kTemp = new List<int>();
        
        foreach (var v in m_kData)
        {
            if (v.Value.ItemID == (int)GITEM.charblock)
            {
                kTemp.Add(v.Key);
            }
        }

        if(kTemp.Count==0)// 하나도 없을때
        {
            for (int i = 0; i < 30; i++)
            {
                int y = 31 - i;
                if (GetBlock(16, y, 16) > 0)
                {
                    int yy = y + 1;
                    AddBlock(16, yy, 16, (int)GITEM.charblock);
                    break;
                }
            }
        }
        if (kTemp.Count>1)// 2개 이상 있을 때,
        {
            for(int i=1;i<kTemp.Count;i++)
            {
                UpdateBlock(kTemp[i], 0);
            }
        }


    }
    // 캐릭터 블록이 있는지 검사
    public bool CheckCharBlock()
    {
        foreach(var v in m_kData)
        {
            if(v.Value.ItemID==(int)GITEM.charblock)
            {
                return true;
            }
        }

        return false;
    }

    protected override bool LoadObjectFunc()
    {


        GameObject.Destroy(m_gItemBody);
        m_gItemBody = new GameObject();
        m_gItemBody.name = "ItemBody";
        m_gItemBody.tag = gameObject.tag;
        
        m_gItemBody.transform.parent = m_gCenterObject.transform;
        m_gItemBody.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
        m_gItemBody.transform.localScale = Vector3.one;
        m_gItemBody.transform.localEulerAngles = Vector3.zero;

        m_kWeapon.Clear();
        foreach (var v in m_kData)
        {
            int nItem= ChangeItem(v.Value.nBlock);
            ItemObjectAttach(nItem%256, v.Key, nItem /256);
        }

        CalPower();
        CallSize();

        MakeProgressBar();
        WeaponSetting();
        // 체크 캐릭터 블록
        OnLoadEnd();
        

      //  StartCoroutine("IDamageShow");
        return true;
    }
    protected  virtual void SetHitDummy()
    {
        if (m_gHitDummy)
        {
            Destroy(m_gHitDummy);
        }
        m_gHitDummy = new GameObject();
        m_gHitDummy.transform.parent = m_gCenterObject.transform;
        m_gHitDummy.transform.localPosition = m_vCenter;
        m_gHitDummy.name = "HitDummy";
        SphereCollider ss = m_gHitDummy.GetComponent<SphereCollider>();
        if (ss == null)
        {
            ss = m_gHitDummy.AddComponent<SphereCollider>();
        }
        ss.radius = m_vSize.x+0.5f;
        ss.isTrigger = true;

    }
    protected override void OnLoadEnd()
    {
       // 에디터가 아니면
        //SphereCollider ss = gameObject.GetComponent<SphereCollider>();
        //if(ss==null)
        //{
        //    ss = gameObject.AddComponent<SphereCollider>();

        //}
//        ss.radius = m_vSize.x;
  //      ss.center = m_vCenter;
      //  ss.isTrigger = true;
        SetHitDummy();
        base.OnLoadEnd();
    }

    int OldItemConvert(int nItem)
    {
        string szItem = CWTableManager.Instance.GetTable("OldItem", "Item", nItem);
        if (szItem != null && szItem.Length > 1)
        {
            return CWArrayManager.Instance.GetItemNumber(szItem);
        }


        return 0;
    }
    protected override bool LoadMeshFunc()
    {

        if(m_gBody==null)
        {
            Debug.Log("");
            return false;
        }

        if (m_gBody.GetComponent<MeshFilter>().sharedMesh != null)
        {
            GameObject.DestroyImmediate(m_gBody.GetComponent<MeshFilter>().sharedMesh);
            CWMakeMesh.G_MeshCount--;
        }


        CWMakeBlock kMakeBlock = new CWMakeBlock();// 블록을 메쉬로 만들 수 있게 머지하는 작업


        m_gBody.GetComponent<MeshFilter>().sharedMesh = kMakeBlock.Create(ConvertBlock, _GetShape, _GetColor, SELLWIDTH, SELLWIDTH, SELLWIDTH, true); // 블록을 가공한다  

        if(m_bMeshCollider)
        {
            AddCollideBox();
        }
        

        return true;
    }
#endregion


#region 외부접근함수
    public  void ReceiveData(byte [] bBuffer)
    {
        SetBuffer(bBuffer);
        base.Create(m_nID);

    }

    protected override string GetPath()
    {
        return "AirCraft/" + name;
    }

    public Vector3 GetCenter()
    {
        return m_vCenter;
    }
    public override void Close()
    {
        
        base.Close();
    }
    public void ClearBlock()
    {
        m_bUpdated = true;
        m_kData.Clear();
        if (m_gItemBody == null) return;
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        foreach (var v in array)
        {
            GameObject.Destroy(v.gameObject);
        }
        
        
    }
    public void EmptyCreate()
    {
        SELLWIDTH = 32;

        BodyCreate();
    }
#endregion

#region 네트워 관련

    public override void ShootPos(bool bDetected,Vector3 vTarget)
    {

        Shoot(bDetected,vTarget);
    }

#endregion


#region die


    //
    protected virtual void DieClose()
    {
        base.SetDie();
    }
    public override void SetDie()//죽는 모습, 폭발 등
    {
        CWEffectManager.Instance.GetEffect(GetHitPos(), "Ef_ExplosionParticle_01");
        CWResourceManager.Instance.PlaySound("BuildDie_1", gameObject);
        CWBombManager.Instance.PlayObject(this);
        base.SetDie();

    }


#endregion

#region 전투관련


    // hp에 맞게 현재블록을 조정한다 
    // 초기 시작할 때만 적용 
    protected override void ConvertHPBlock()
    {
        m_kDelFile.Clear();
        m_bUpdated = true;
        // 전체를 조정
        int nDelHP = KPower.m_nHp - KPower.GetHP();
        if (nDelHP <= 0) return;

        // 가장자리부터 정렬
        List<int> kTemp = new List<int>();
        foreach (var v in m_kData)
        {
            kTemp.Add(v.Key);
        }

        kTemp.Sort(CampareSort);

        int nn = 0;
        foreach (var v in kTemp)
        {
            int nItem = m_kData[v].nBlock;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            nn += gData.hp;
            m_kDelFile.Add(v, nItem);
            if (nn >= nDelHP)
            {
                break;
            }
        }
        
    }
    public virtual void Shoot(bool bDeteted, Vector3 vTarget,GameObject gTarget=null)
    {
        if (CWGlobal.g_GameStop) return;
        if (CWGlobal.g_bStopAIAttack) return;
        foreach (var v in m_kWeapon)
        {
            v.Shoot(this, bDeteted, vTarget, gTarget);
            
        }
    }

    
#endregion


#region AI 액션 관련


    public override void AIShoot(bool bDetected, CWObject gTarget)
    {
        if (gTarget == null) return;
        Shoot(bDetected, gTarget.GetHitPos());
    }
    public override void AIShootPos(bool bDetected, Vector3 vPos)
    {
        Shoot(bDetected,vPos);
    }
#endregion

    /*
     * 건물 지음
     * */

#region  편집

//    public Vector3 FindCharBlock()
//    {
//rm
//    }
    public void AddBlock(CWAirObject kTargetObject)
    {
        foreach (var v in m_kData)
        {


            int x = GetSellX(v.Key);
            int y = GetSellY(v.Key);
            int z = GetSellZ(v.Key);
            kTargetObject.SetBlock(x, y, z, v.Value.nBlock, v.Value.nShape, v.Value.nColor);
        }

    }
    //protected Dictionary<int, BlockData> m_kData = new Dictionary<int, BlockData>();
    public void Flip()
    {
        Dictionary<int, BlockData> kTemp = new Dictionary<int, BlockData>();
        foreach(var v in m_kData)
        {
            kTemp.Add(v.Key, v.Value);
        }
        m_kData.Clear();

        for (int z = 0; z < SELLWIDTH; z++)
        {
            for (int y = 0; y < SELLWIDTH; y++)
            {
                int txx = SELLWIDTH - 1;
                for (int x = 0; x < SELLWIDTH; x++)
                {
                    int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
                    if (kTemp.ContainsKey(num))
                    {
                        int num2 = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + txx-x;
                        BlockData kdata = kTemp[num];

                        if (kTemp[num].nShape==(int)BLOCKSHAPE.SLANT_2)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.SLANT_4;
                        }
                        else if (kTemp[num].nShape==(int)BLOCKSHAPE.SLANT_4)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.SLANT_2;
                        }

                        if (kTemp[num].nShape == (int)BLOCKSHAPE.STAIRS_2)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.STAIRS_4;
                        }
                        else if (kTemp[num].nShape == (int)BLOCKSHAPE.STAIRS_4)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.STAIRS_2;
                        }


                        if (kTemp[num].nShape == (int)BLOCKSHAPE.STAIRS_2_B)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.STAIRS_4_B;
                        }
                        else if (kTemp[num].nShape == (int)BLOCKSHAPE.STAIRS_4_B)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.STAIRS_2_B;
                        }



                        if (kTemp[num].nShape == (int)BLOCKSHAPE.SLANT_6)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.SLANT_8;
                        }
                        else if (kTemp[num].nShape == (int)BLOCKSHAPE.SLANT_8)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.SLANT_6;
                        }


                        if (kTemp[num].nShape == (int)BLOCKSHAPE.HSLANT_2)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.HSLANT_4;
                        }
                        else if (kTemp[num].nShape == (int)BLOCKSHAPE.HSLANT_4)
                        {
                            kdata.nShape = (int)BLOCKSHAPE.HSLANT_2;
                        }




                        m_kData.Add(num2, kdata);



                    }

                }

            }

        }

        //foreach(var v in m_kData)
        //{
        //    //10 32-10 
        //    int x = GetSellX(v.Key);
        //    int y = GetSellY(v.Key);
        //    int z = GetSellZ(v.Key);
        //    int tx = (SELLWIDTH-1) - x;

        //    int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        //    int num2 = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + tx;
        //    m_kData[num] = kTemp[num];

        //}
        m_bUpdated = true;
    }



#endregion

   
    public List<SLOTITEM> GetList(string szType)
    {
        List<SLOTITEM> kTemp = new List<SLOTITEM>();
        foreach (var v in m_kData)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
            if (gData.type == szType)
            {
                SLOTITEM ss = new SLOTITEM();
                ss.m_nSlot = v.Key;
                ss.NItem = v.Value.nBlock;
                kTemp.Add(ss);
            }
        }

        return kTemp;
    }


    protected virtual void CheckDie()
    {

        if (GetHP()<=0)
        {
            SetDie();
        }

    }

    protected override void OnHit(int nDamage)
    {
        if (KPower == null) return;
        if(!CWGlobal.g_bSingleGame)
        {
            int nID = 0;
            if (m_gKiller != null)
            {
                nID=m_gKiller.m_nID;
            }

            CWSocketManager.Instance.SendUserDamage(nID, m_nID, nDamage,(jData)=> {

                if(CWJSon.GetInt(jData,"Musuk")==1)
                {
                    NoticeMessage.Instance.Show("상대는 현재 무적입니다!");
                }
                

            });
            
            return;
        }
        if (IsDie())
        {
            return;// 이미 사망했다면
        }
        KPower.UpdateHP(-nDamage);
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai)
        {
            ai.SetEnemy(m_gKiller);
        }
        CheckDie();
        


        
    }
#region 데미지 리액션 

    protected bool m_bHitReAction = false;
    protected void CloseReAction()
    {
        m_bHitReAction = false;
    }
    protected override void HitReAction()
    {
        if (m_bHitReAction) return;
        m_bHitReAction = true;
        if (m_gKiller == null) return;
        Vector3 v1 = m_gKiller.transform.position;// -10
        Vector3 v2 = transform.position;  // 5   
        Vector3 vdir = v2 - v1; // 때린자에서 맞은자 방향 
        vdir.Normalize();
        float fMaxdist = 0.02f; // 최대 밀리는 숫자 
        Vector3 vPos = v2 + fMaxdist * vdir;
        vPos.y = v2.y;

        transform.DOMove(vPos, 0.1f).SetLoops(2, LoopType.Yoyo);
        Invoke("CloseReAction", 2);

        



    }

#endregion
    /*
#region 데미지 쇼
    int m_nDamageShow = 0;
    protected virtual void BeginDamageText(int nDamage)
    {

        Color kColor = Color.red;
        if(IsHeroTeam())
        {
            kColor = Color.yellow;
        }
        
        GameObject gg = CWPoolManager.Instance.GetObject("DamageText", 20);
        if (gg)
        {
            gg.name = "DamageText";
            Vector3 vPos = m_vCenter;
            vPos.y += (m_vSize.y + 10);
            gg.transform.SetParent(m_gTempdir.transform);
            gg.transform.localPosition = vPos;
            gg.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
            gg.tag = gameObject.tag;

            DamageText dd = gg.GetComponentInChildren<DamageText>();
            dd.Begin(nDamage.ToString(), kColor);
        }

    }
    protected virtual void DamageRun()
    {
        if (m_nDamageShow > 0)
        {
            BeginDamageText(m_nDamageShow);
            m_nDamageShow = 0;
        }

    }
    IEnumerator IDamageShow()
    {
        while(true)
        {
            DamageRun();
            yield return new WaitForSeconds(0.3f);
        }
    }



#endregion

    */
    protected virtual void BeginHPBar()
    {

        UnitHPBar hpbar= gameObject.GetComponentInChildren<UnitHPBar>();
        if(hpbar==null)
        {
            GameObject gPress = CWResourceManager.Instance.GetPrefab("HPbar");
            if (gPress)
            {
                gPress.name = "HPbar";
                Vector3 vPos = m_vCenter;
                vPos.y += (m_vSize.y + 10);
                gPress.transform.SetParent(m_gTempdir.transform);
                gPress.transform.localPosition = vPos;
                gPress.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
                gPress.tag = gameObject.tag;
            }
        }
        else
        {
            hpbar.SetUpdate();
        }
        
    }

    public bool CheckShipblockItem()
    {
        foreach (var v in m_kData)
        {
            int nItem = v.Value.nBlock;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            if (gData.type == "shipblock")
            {
                return true;
            }
        }
        return false;
    }

    public bool CheckWeaponItem()
    {
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        int tcnt = 0;
        foreach(var v in array)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.m_ID);
            if(gData.type=="weapon")
            {
                tcnt++;
            }
            
        }
        if (tcnt ==0) return false;

        return true;
    }
    public bool CheckCharBlockItem()
    {
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        int tcnt = 0;
        foreach (var v in array)
        {
            if(v.m_ID==(int)GITEM.charblock)
            {
                tcnt++;
            }

        }
        if (tcnt == 0) return false;

        return true;
    }

    public bool CheckBusterItem()
    {
        CWItemObject[] array = m_gItemBody.GetComponentsInChildren<CWItemObject>();
        int tcnt = 0;
        foreach (var v in array)
        {
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.m_ID);
            if (gData.type == "Buster")
            {
                tcnt++;
            }

        }
        if (tcnt == 0) return false;

        return true;

    }
    public int GetWeaponItemCount()
    {
        int tcnt = 0;
        foreach (var v in m_kData)
        {
            int nItem = v.Value.nBlock;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            if (gData.type == "weapon")
            {
                tcnt++;
            }
        }
        return tcnt;


    }
    public int GetBusterItemCount()
    {

        int tcnt = 0;
        foreach (var v in m_kData)
        {
            int nItem = v.Value.nBlock;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            if (gData.type == "Buster")
            {
                tcnt++;
            }
        }
        return tcnt;


    }

    public GameObject GetHitObject()
    {
        return m_gHitDummy;
    }
    public override Vector3 GetHitPos()
    {
        if (m_gHitDummy == null) return transform.position;
        return m_gHitDummy.transform.position;
    }
#region 좌표 픽스 

    float DEFAULT_HY = 1;
    float DEFAULT_scale = 28;

    public float GetScaleRate()
    {
        float fx = m_vSize.x;
        float fz = m_vSize.z;
        float fdx = DEFAULT_scale / (fx + fz);
        if (fdx > 1) fdx = 1;
        return fdx;
    }

    public virtual  void FixPos()
    {

        
        float fy = m_vStart.y - DEFAULT_HY;
        float fx = m_vSize.x;
        float fz = m_vSize.z;
        float fdx = GetScaleRate();
        if (fdx > 1) fdx = 1;
        fy *= fdx;

        if (m_gCenterObject == null) return;
        m_gCenterObject.transform.localScale = new Vector3(fdx, fdx, fdx);
        m_gCenterObject.transform.localPosition = new Vector3(0, -fy, 0);

        SetHitDummy();
    }


#endregion
#region 롤 회전

    bool m_bRotateflag = false;
    public void RollRoate(float ndir)
    {

        GameObject gg = m_gCenterObject;
        if (gg == null)
        {
            gg = gameObject;
        }
        if (!m_bRotateflag)
        {
            m_bRotateflag = true;
            float fval = 35 * (ndir / ndir);

            Transform tForm = gg.transform;
            tForm.DOLocalRotate(new Vector3(0, 0, fval), 0.9f).OnComplete(() => {
                m_bRotateflag = false;

            });
        }
    }
    public void RollRoateStop()
    {

        GameObject gg = m_gCenterObject;
        if (gg == null)
        {
            gg = gameObject;
        }

        m_bRotateflag = false;
        Transform tForm = gg.transform;
        tForm.DOKill();
        tForm.DOLocalRotate(new Vector3(0, 0, 0), 0.3f);
    }


#endregion

#region 연결안된 파일 검사
    int[,] vNormal =
    {


        {-1,-1,-1},
    {0,-1,-1},
    {1,-1,-1},
    {-1,0,-1},
    {0,0,-1},
    {1,0,-1},
    {-1,1,-1},
    {0,1,-1},
    {1,1,-1},
    {-1,-1,0},
    {0,-1,0},
    {1,-1,0},
    {-1,0,0},
    {1,0,0},
    {-1,1,0},
    {0,1,0},
    {1,1,0},
    {-1,-1,1},
    {0,-1,1},
    {1,-1,1},
    {-1,0,1},
    {0,0,1},
    {1,0,1},
    {-1,1,1},
    {0,1,1},
    {1,1,1}


    };
    
    bool _RLinkBlock(int nFindNumber, List<Vector3Int> kUse, Vector3Int vv)
    {
        

        if(kUse.Exists(x =>(x==vv)))
        {
            return false;
        }
        kUse.Add(vv);
        int num = vv.z * SELLWIDTH * SELLWIDTH + vv.y * SELLWIDTH + vv.x;
        if (nFindNumber== num)
        {
            // 여기 찾으면 끝
            return true;
        }
      
        for (int i = 0; i < 26; i++)
        {
            
            int tx = vv.x + vNormal[i, 0];
            int ty = vv.y + vNormal[i, 1];
            int tz = vv.z + vNormal[i, 2];
            if (GetBlock(tx, ty, tz) == 0) continue;
            if (_RLinkBlock(nFindNumber,kUse, new Vector3Int(tx,ty,tz)))
            {
                return true;
            }
        }
        return false;
    }

    
    public bool IsAllLinkBlock(int sx,int sy,int sz)
    {

        if(m_kData.Count>100)
        {
            return true;
        }
        int MaxDist = 4;
        int BaseNumber = 0;
        foreach (var v in m_kData)
        {

            int x = GetSellX(v.Key);
            int y = GetSellY(v.Key);
            int z = GetSellZ(v.Key);

            if (x < sx - MaxDist) continue;
            if (y < sy - MaxDist) continue;
            if (z < sz - MaxDist) continue;

            if (x > sx + MaxDist) continue;
            if (x > sx + MaxDist) continue;
            if (x > sx + MaxDist) continue;

            if (GetBlock(x, y, z) == 0) continue;
            if (BaseNumber == 0)
            {
                BaseNumber = v.Key;
                continue;
            }

            Vector3Int tt = new Vector3Int(x,y,z);
            List<Vector3Int> kUse = new List<Vector3Int>();
            if (!_RLinkBlock(BaseNumber, kUse, tt))
            {
               
                return false;
            }
        }


        return true;
    }


    #endregion


    #region FILE_LOAD_SAVE //파일  저장/불러오기 

    public override bool LoadJSon()
    {
        if (!m_kJSon.IsLoad()) return false;
        m_kData.Clear();

        SELLWIDTH = m_kJSon.GetInt("SELLWIDTH");

        if (SELLWIDTH == 0)
        {
            SELLWIDTH = 32;
        }
        BodyCreate();

        m_HeightDelta = m_kJSon.GetFloat("HeightDelta");
        Vector3 vPos2 = transform.localPosition;
        vPos2.y -= m_HeightDelta;
        transform.localPosition = vPos2;

        if (m_kJSon.GetInt("Colorversion") == 1)
        {
            byte[] bArray = m_kJSon.GetBytes("block");
            for (int z = 0; z < SELLWIDTH; z++)
            {
                for (int y = 0; y < SELLWIDTH; y++)
                {
                    for (int x = 0; x < SELLWIDTH; x++)
                    {
                        int num = ((y * SELLWIDTH + x) * SELLWIDTH + z) * 4;
                        if (bArray[num] > 0)
                        {

                            _AddBlock(x, y, z, bArray[num], bArray[num + 1], bArray[num + 2], bArray[num + 3]);
                        }

                    }
                }
            }

        }
        else
        {
            byte[] bArray = m_kJSon.GetBytes("block");
            for (int z = 0; z < SELLWIDTH; z++)
            {
                for (int y = 0; y < SELLWIDTH; y++)
                {
                    for (int x = 0; x < SELLWIDTH; x++)
                    {
                        int num = ((y * SELLWIDTH + x) * SELLWIDTH + z) * 3;
                        if (bArray[num] > 0)
                        {

                            _AddBlock(x, y, z, bArray[num], bArray[num + 1],0, bArray[num + 2]);
                        }

                    }
                }
            }

        }
        Vector3 vPos = new Vector3();
        Vector3 vRot = new Vector3();
        Vector3 vScale = new Vector3();


        // (JObject)m_kJSon.GetJson("Info");

        float vx = CWJSon.GetFloat(m_kJSon.GetJson("Info"), "x");
        float vy = CWJSon.GetFloat(m_kJSon.GetJson("Info"), "y");
        float vz = CWJSon.GetFloat(m_kJSon.GetJson("Info"), "z");

        m_vInfo = new Vector3(vx, vy, vz);

        JArray Js = (JArray)m_kJSon.GetJson("MeshArray");
        if (Js != null)
        {
            foreach (var v in Js)
            {
                vPos.x = v["PosX"].Value<float>();
                vPos.y = v["PosY"].Value<float>();
                vPos.z = v["PosZ"].Value<float>();

                vRot.x = v["RotX"].Value<float>();
                vRot.y = v["RotY"].Value<float>();
                vRot.z = v["RotZ"].Value<float>();

                vScale.x = v["ScaleX"].Value<float>();
                vScale.y = v["ScaleY"].Value<float>();
                vScale.z = v["ScaleZ"].Value<float>();
                string szName = v["Name"].ToString();
                GameObject gg = CWResourceManager.Instance.GetPrefab(szName);
                if (gg == null) continue;
                gg.name = szName;
                gg.transform.parent = m_gMeshDir.transform;
                gg.transform.localPosition = vPos;
                gg.transform.localEulerAngles = vRot;
                gg.transform.localScale = vScale;

            }

        }

        if (m_kJSon.GetInt("SaveSphereCollider") == 1)
        {
            m_bSaveSphereCollider = true;

            m_vSize.x = CWJSon.GetFloat(m_kJSon.GetJson("Size"), "x");
            m_vSize.y = CWJSon.GetFloat(m_kJSon.GetJson("Size"), "y");
            m_vSize.z = CWJSon.GetFloat(m_kJSon.GetJson("Size"), "z");

            m_vCenter.x = CWJSon.GetFloat(m_kJSon.GetJson("Center"), "x");
            m_vCenter.y = CWJSon.GetFloat(m_kJSon.GetJson("Center"), "y");
            m_vCenter.z = CWJSon.GetFloat(m_kJSon.GetJson("Center"), "z");


        }
        else
        {
            m_bSaveSphereCollider = false;
        }
        CheckCharAddBlock();

        m_kJSon.Close();
        m_szName = name;


        return true;
    }

    public void Clear()
    {
        m_kData.Clear();
    }

    public void CopyBlock(CWAirObject kObject)
    {
        kObject.Clear();
        foreach (var v in m_kData)
        {
            int n = CWArrayManager.Instance.GetItemFromBlock(v.Value.nBlock);
            if (n == 0) n = 1;
            int x = GetSellX(v.Key);
            int y = GetSellY(v.Key);
            int z = GetSellZ(v.Key);
            kObject.SetBlock(x, y, z, n, v.Value.nShape, v.Value.nColor);
        }

    }
    // 블록을 덧붙인다 


    int ConvertOldblock(int nBlock)
    {
        int nRet = nBlock;
#if UNITY_EDITOR

        if (nBlock == 12) return 126;

        int nKey = CWTableManager.Instance.Find("block - convert", "num", nBlock.ToString());
        if (nKey > 0)
        {
            nRet = CWTableManager.Instance.GetTableInt("block - convert", "ID", nKey);
        }


#endif

        return nRet;
    }


    public byte[] GetBuffer()
    {
        return GetJSonByte();
    }
    public void UpGradeBlock(int AirSlotID, int nKey)
    {

        if (m_kData.ContainsKey(nKey))
        {

            BlockData nData = m_kData[nKey];
            nData.Level++;
            m_kData[nKey] = nData;
            //    m_bUpdated = true;
            byte[] buffer = GetJSonByte();
            CalPower();

            //public void UpdateAirObject(int AirSlotID,int maxhp,int Damage,int blockcount, byte[] bbb,Action func)
            CWSocketManager.Instance.UpdateAirObject(AirSlotID,GetMaxHP(),GetDamage(),NBlockCount, buffer, () => {

            });


        }


    }

    //32으로 모두 통일
    virtual public byte[] GetJSonByte()
    {

        int _SELLWIDTH = 32;


        byte[] bBuffer = new byte[_SELLWIDTH * _SELLWIDTH * _SELLWIDTH * 4];

        CWJSon jSon = new CWJSon();

        int delta = 0;
        if (SELLWIDTH == 64) delta = 16;

        foreach (var v in m_kData)
        {
            if (v.Value.nBlock > 0)
            {
                int x = GetSellX(v.Key) - delta;
                int y = GetSellY(v.Key);
                int z = GetSellZ(v.Key) - delta;
                int num = ((y * _SELLWIDTH + x) * _SELLWIDTH + z) * 4;
                if (x < 0) continue;
                if (y < 0) continue;
                if (z < 0) continue;

                if (x >= _SELLWIDTH) continue;
                if (y >= _SELLWIDTH) continue;
                if (z >= _SELLWIDTH) continue;


                BlockData bs = v.Value;
                bBuffer[num] = (byte)(bs.nBlock % 256);
                bBuffer[num + 1] = bs.nShape;
                bBuffer[num + 2] = (byte)(bs.nBlock / 256);
                bBuffer[num + 3] = (byte)bs.nColor;
            }


        }

        jSon.Add("SELLWIDTH", _SELLWIDTH);
        jSon.Add("Colorversion", 1);
        jSon.Add("block", bBuffer);
        jSon.Add("HeightDelta", m_HeightDelta);

        BaseUnitUI cp = gameObject.GetComponentInChildren<BaseUnitUI>();
        if (cp != null)
        {


            JObject jjj = new JObject();
            jjj.Add("x", cp.transform.localPosition.x);
            jjj.Add("y", cp.transform.localPosition.y);
            jjj.Add("z", cp.transform.localPosition.z);

            jSon.Add("Info", jjj);
        }

        {

            JArray jo = new JArray();
            for (int i = 0; i < m_gMeshDir.transform.childCount; i++)
            {
                Transform tChild = m_gMeshDir.transform.GetChild(i);

                Vector3 vPos = tChild.localPosition;
                Vector3 vAngle = tChild.localEulerAngles;
                Vector3 vScale = tChild.localScale;

                JObject jj = new JObject();
                jj.Add("Name", tChild.name);
                jj.Add("PosX", vPos.x);
                jj.Add("PosY", vPos.y);
                jj.Add("PosZ", vPos.z);

                jj.Add("RotX", vAngle.x);
                jj.Add("RotY", vAngle.y);
                jj.Add("RotZ", vAngle.z);

                jj.Add("ScaleX", vScale.x);
                jj.Add("ScaleY", vScale.y);
                jj.Add("ScaleZ", vScale.z);
                jo.Add(jj);

            }
            jSon.Add("MeshArray", jo);
        }
        //m_gMeshDir

        if (m_bSaveSphereCollider)
        {
            jSon.Add("SaveSphereCollider", 1);
            SphereCollider ss = gameObject.GetComponent<SphereCollider>();
            if (ss != null)
            {
                //                m_vSize.x = CWJSon.GetFloat(m_kJSon.GetJson("Size"), "x");
                m_vSize.x = ss.radius;
                m_vCenter = ss.center;

                {
                    JObject jjj = new JObject();
                    jjj.Add("x", m_vSize.x);
                    jjj.Add("y", m_vSize.y);
                    jjj.Add("z", m_vSize.z);
                    jSon.Add("Size", jjj);
                }
                {
                    JObject jjj = new JObject();
                    jjj.Add("x", m_vCenter.x);
                    jjj.Add("y", m_vCenter.y);
                    jjj.Add("z", m_vCenter.z);
                    jSon.Add("Center", jjj);

                }



            }
            


        }
        else
        {
            jSon.Add("SaveSphereCollider", 0);
        }



        return jSon.ToArray();

    }
    public bool Save(string szFile)
    {
        byte[] kByte = GetJSonByte();


#if UNITY_EDITOR
        try
        {
            using (FileStream fs = File.OpenWrite(szFile))
            {
                fs.Write(kByte, 0, kByte.Length);
                fs.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning("Failed to deserialize a file " + szFile + " (Reason: " + e.Message + ")");
        }
#endif
        return true;
    }



    #endregion FILE_LOAD_SAVE



    public virtual Vector3 GetMoveDir()
    {
        return transform.forward;
    }








}
