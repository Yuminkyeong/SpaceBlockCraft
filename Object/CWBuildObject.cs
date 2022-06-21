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

/*
 * 건축,터렛등 
 * */

public class CWBuildObject : CWObject
{
    #region INIT
    public string m_szName;
    protected bool m_bLoadEnd = false;


    public static BlockData Zero = new BlockData();

    const int __OLD_BLOCKLISTVERSION_ = 203;
    const int __OLD_BLOCKLISTVERSION2_ = 204;
    const int __BLOCKLISTVERSION_ = 205;


    protected Vector3 m_vInfo=Vector3.zero;

    protected Dictionary<int, BlockData> m_kData = new Dictionary<int, BlockData>();

    public Dictionary<int, BlockData> GetData()
    {
        return m_kData;
    }

    public long m_nBuildNumber=0;

    public int m_nLevel;
    public int m_nPattenID;// maplist에서 가져오는 아이디 maplist = patten

    public float m_HeightDelta=0;

    //SphereCollider
    [Header("충돌구 수동조작")]
    public bool m_bSaveSphereCollider = false;//  충돌 수동으로 저장

    bool _bUpdated = false;

    public bool m_bUpdated
    {
        get
        {
            return _bUpdated;
        }

        set
        {
            _bUpdated = value;
        }
    }

    #endregion

    



    #region RUN // 구현

    public override void SetPos(Vector3 vPos)
    {

        transform.position = vPos;
    }
    virtual public void UpdateObject()
    {

        LoadMeshFunc();
    }



    virtual protected  bool Run()
    {
        if (m_bUpdated)
        {
            m_bUpdated = false;
            //LoadMeshFunc();
            //LoadObjectFunc();
            UpdateObject();

        }
        
        return true;

    }
    public void UpdateBlock()
    {
        Run();
    }

    public override void Close()
    {
        DeleteFile();
        base.Close();
    }
    
    public override void Create(int nID)
    {
        m_kData.Clear();
        base.Create(nID);
      

    }

  
    public void AddCollideBox()
    {
        
        if (m_gBody.GetComponent<MeshCollider>()==null)
        {
            m_gBody.AddComponent<MeshCollider>();
        }
        {
            Mesh mf=m_gBody.GetComponent<MeshFilter>().sharedMesh;
            m_gBody.GetComponent<MeshCollider>().sharedMesh = null;
            m_gBody.GetComponent<MeshCollider>().sharedMesh = mf;

        }


    }

    protected override void DeleteFile()
    {
        if (m_gBody != null)
        {
            if (m_gBody.GetComponent<MeshFilter>().sharedMesh != null)
            {
                GameObject.DestroyImmediate(m_gBody.GetComponent<MeshFilter>().sharedMesh);
                CWMakeMesh.G_MeshCount--;
            }
            
            Destroy(m_gBody);
            m_gBody = null;
        }
        if(m_gItemBody)
        {
            GameObject.Destroy(m_gItemBody);
            m_gItemBody = null;
        }
      
        m_kData.Clear();

        Destroy(m_gTempdir);
        m_gTempdir = null;
        Destroy(m_gMeshDir);
        m_gMeshDir = null;
        base.DeleteFile();

    }


    protected override bool LoadMeshFunc()
    {
        
        if(m_gBody.GetComponent<MeshFilter>().sharedMesh!=null)
        {
            GameObject.DestroyImmediate(m_gBody.GetComponent<MeshFilter>().sharedMesh);
            CWMakeMesh.G_MeshCount--;
        }
        CWMakeBlock kMakeBlock = new CWMakeBlock();// 블록을 메쉬로 만들 수 있게 머지하는 작업


        m_gBody.GetComponent<MeshFilter>().sharedMesh = kMakeBlock.Create(ConvertBlock, _GetShape, _GetColor, SELLWIDTH, SELLWIDTH, SELLWIDTH, true); // 블록을 가공한다  
        
//에디터 일때만




        return true;

    }
    protected override bool LoadObjectFunc()
    {
        //GameObject.Destroy(m_gItemBody);
        //m_gItemBody = new GameObject();
        //m_gItemBody.name = "ItemBody";
        //m_gItemBody.tag = gameObject.tag;
        //m_gItemBody.layer = gameObject.layer;
        //m_gItemBody.transform.parent = transform;
        //m_gItemBody.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
        //m_gItemBody.transform.localScale = Vector3.one;
        //m_gItemBody.transform.localEulerAngles = Vector3.zero;


        //MakeProgressBar();
        //if (!base.LoadObjectFunc()) return false;

        //MakeDamageShow();
        //CWLib.SetGameObjectLayer(gameObject, (int)m_nLayer);

        //OnLoadEnd();

        return true;
    }
    // 로드가 끝난 시점
    protected virtual void OnLoadEnd()
    {
        HpEvent();
        m_bLoadEnd = true;
    }
    public bool IsLoadEnd()
    {
        return m_bLoadEnd;
    }


    float m_fHittimer = 0;
    

    protected virtual void OnHit(int nDamage)
    {
        if (KPower == null) return;
        if (IsDie())
        {
            return;// 이미 사망했다면
        }
        if (GetHP() <= 0) return ;

        
        KPower.UpdateHP(-nDamage);
        if(KPower.GetHP()<=0)
        {
            SetDie();

        }
        m_fHittimer = Time.time;
        
        

        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if(ai)
        {
            ai.SetEnemy(m_gKiller);
        }


    }


    // 파괴된 다음 블록을 준다. 

    // Hit 리액션
    protected virtual void HitReAction()
    {


    }
    public virtual string GetHitEffect()
    {
        return "pf_smoke";
    }

    
    public virtual  bool Hit(CWObject kKiller,int nDamage)//블록 폭파 개수  fBlockCount
    {


        m_gKiller = kKiller;
        HitReAction();
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if(ai!=null)
        {
            //ai.m_gEnemy = m_gKiller;
            ai.SetEnemy(m_gKiller);
        }
        
        OnHit(nDamage);
        return true;
    }
    //맵에 데이타를 복사한다 
    public void CopyBlockMap(int sx, int sy, int sz, dlSetblock _Setblock)
    {

        sx = sx - SELLWIDTH / 2;
        sz = sz - SELLWIDTH / 2;
        for (int z = 0; z < SELLWIDTH; z++)
        {
            for (int y = 0; y < SELLWIDTH; y++)
            {
                for (int x = 0; x < SELLWIDTH; x++)
                {
                    int nblock = _GetBlock(x, y, z);
                    if (nblock == 0) continue;
                    if (y > 60) continue;
                    int tx = x + sx;
                    int ty = y + sy;
                    int tz = z + sz;
                    _Setblock(tx, ty, tz, nblock);
                }

            }

        }

    }

    #endregion RUN

    #region BLOCK

    public int GetSellZ(int num)
    {
        return num / (SELLWIDTH * SELLWIDTH);
    }
    public int GetSellX(int num)
    {
        return num % SELLWIDTH;
    }
    public int GetSellY(int num)
    {
        return (num / SELLWIDTH) % (SELLWIDTH);
    }


    protected void _DelBlock(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        m_kData.Remove(num);
        m_bUpdated = true;
    }


   
    protected int GetRandomResBlock()
    {
        return CWGlobal.GetResBlock(CWTableManager.Instance.GetTable("maplist", "ResBlock1", m_nPattenID));

    }
  
    protected int GetRandomResBlock2()
    {
        return CWGlobal.GetResBlock(CWTableManager.Instance.GetTable("maplist", "ResBlock2", m_nPattenID));

    }


    protected virtual int ChangeBlock(int x,int y,int z, int nBlock)
    {
        return nBlock;
    }

    public virtual  void AddBlock(int x,int y,int z,int nBlock,int nShape=0,int nColor=0)
    {
        if(x<0)
        {
            return;
        }
        if (x >=SELLWIDTH)
        {
            return;
        }
        if(y<0)
        {
            return;
        }
        if (y >=SELLWIDTH)
        {
            return;
        }
        if(z<0)
        {
            return;
        }
        if (z >=SELLWIDTH)
        {
            return;
        }

        _AddBlock(x, y, z, nBlock%256, nShape, nBlock/256, nColor);
    }
    protected void _AddBlock(int x, int y, int z, int nBlock, int nShape, int nLevel, int nColor)
    {
        // 디버깅
    ///    print(string.Format(" {0} {1} {2} {3} ", nBlock,nShape,nColor,nLevel));
        //GITEMDATA gData = CWArrayManager.Instance.GetItemData(nBlock);
        //if(gData.nID==0)
        //{
        //    Debug.Log(string.Format("없다! {0}",nBlock));

        //    return;

        //}
 

        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if(m_kData.ContainsKey(num))
        {
            int nBlockItem =  nBlock;
            BlockData nData = m_kData[num];
#if UNITY_EDITOR
            nData.ItemID = (byte)ChangeBlock(x,y,z, nBlockItem);
            nData.Level = (byte)nLevel;
#else
            nData.ItemID = (byte)nBlockItem;
            nData.Level = (byte)nLevel;
#endif
            nData.nColor = (byte)nColor;
            nData.nShape = (byte)nShape;
            m_kData[num] = nData;

        }
        else
        {
            BlockData nData = new BlockData();
            int nBlockItem =  nBlock;
#if UNITY_EDITOR
            nData.ItemID = (byte)ChangeBlock(x, y, z, nBlockItem);
            nData.Level = (byte)nLevel;

#else
            nData.ItemID = (byte)nBlockItem;
            nData.Level = (byte)nLevel;

#endif
            nData.nColor = (byte)nColor;
            nData.nShape = (byte)nShape;
            m_kData.Add(num, nData);

        }
        m_bUpdated = true;

    }
    public BlockData GetBlockData(int num)
    {
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num];
        }
        return Zero;

    }
    public BlockData GetBlockData(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num];
        }
        return Zero;

    }
    protected int _GetBlock(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nBlock;
        }
        return 0;

    }
    protected Color _GetColor(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
           return CWGlobal.GetColor((COLORNUMBER) m_kData[num].nColor);
             
        }
        return Color.white;
    }

    protected virtual int ConvertBlock(int x,int y,int z)
    {
        return _GetBlock(x, y, z);
    }

    protected virtual BLOCKSHAPE _GetShape(int x, int y, int z, int nBlock)
    {
        
        return CWArrayManager.Instance.m_kBlock[nBlock].nShape;
    }


    public void DelBlock(int x, int y, int z)
    {
        

        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        m_kData.Remove(num);

        m_bUpdated = true;
    }
    

    public Vector3 SelectPos(Vector3 vPos, Vector3 vNormal)
    {
        Vector3 v = vPos - vNormal;

        float fx = v.x + (float)SELLWIDTH / 2;
        if (vNormal.x < 0)
        {
            fx--;
        }

        int nx = (int)fx - SELLWIDTH / 2;
        float fz = v.z + (float)SELLWIDTH / 2;
        if (vNormal.z < 0)
        {
            fz--;
        }

        int nz = (int)fz - SELLWIDTH / 2;
        int ny = (int)v.y;

        return new Vector3(nx, ny, nz);
    }

    public void UpdateBlock(int x, int y, int z, int nItem,int nLevel)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            BlockData nData = m_kData[num];
            nData.ItemID = (byte)nItem;
            nData.Level = (byte)nLevel;
            m_kData[num] = nData;

        }
        else
        {
            BlockData nData = new BlockData();
            nData.ItemID = (byte)nItem;
            nData.Level = (byte)nLevel;
            m_kData.Add(num, nData);

        }
        m_bUpdated = true;

    }
    public void UpdateBlock(int x,int y,int z,int nBlock)
    {
        
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if(m_kData.ContainsKey(num))
        {
            BlockData nData = m_kData[num];
            nData.nBlock = (ushort)nBlock;
            m_kData[num] = nData;

        }
        else
        {

            BlockData nData = new BlockData();
            nData.nBlock = (ushort)nBlock;
            m_kData.Add(num, nData);

        }
        m_bUpdated = true;

    }
    public void UpdateColor(int x, int y, int z, int nColor)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if(m_kData.ContainsKey(num))
        {
            BlockData nData = m_kData[num];
            nData.nColor =(byte) nColor;
            m_kData[num] = nData;
        }
        m_bUpdated = true;

    }
    public void UpdateShape(int x, int y, int z, int nShape)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            BlockData nData = m_kData[num];
            nData.nShape = (byte)nShape;
            m_kData[num] = nData;
        }
        m_bUpdated = true;

    }



    public void UpdateBlock(int nKey,int nBlock)
    {
        int x = GetSellX(nKey);
        int y = GetSellY(nKey);
        int z = GetSellZ(nKey);
        UpdateBlock( x,  y, z,  nBlock);

    }


    public void UpdateBlockShape(int x, int y, int z,int nBlock, int nShape)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            BlockData nData = m_kData[num];
            nData.nBlock =(ushort) nBlock;
            nData.nShape = (byte)nShape;
            m_kData[num] = nData;
        }
        m_bUpdated = true;

    }


    public void SetBlock(int x, int y, int z, int nBlock, int nShape,int nColor)
    {
        
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        BlockData nData = new BlockData();
        nData.nBlock = (ushort)nBlock;
        nData.nShape = (byte)nShape;
        nData.nColor = (byte)nColor;
        m_kData[num] = nData;
        m_bUpdated = true;

    }
    public int GetBlock(Vector3 vPos)
    {
        vPos.x += SELLWIDTH / 2;
        vPos.z += SELLWIDTH / 2;
        return GetBlock((int)vPos.x, (int)vPos.y, (int)vPos.z);
    }
    public int GetBlock(int x, int y, int z)
    {
        
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nBlock;
        }
        return 0;

    }
    public int GetShape(int x, int y, int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nShape;
        }
        return 0;
    }
    public int GetColor(int x,int y,int z)
    {
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if (m_kData.ContainsKey(num))
        {
            return m_kData[num].nColor;
        }
        return 0;

    }
    public int GetBlockCount()
    {
        return m_kData.Count;
    }


#endregion



}
