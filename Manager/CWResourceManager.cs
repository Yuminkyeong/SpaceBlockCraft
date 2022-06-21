using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWStruct;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using CWUnityLib;
using DG.Tweening;
using CWEnum;
public class CWResourceManager : CWManager<CWResourceManager>
{

    struct PATHDATA
    {
        public DOTweenPath m_kDOTween;
        public float m_fDuration;
    }
    

    public GameObject m_gProductiondir;
    

    private Dictionary<string, GameObject> m_Production = new Dictionary<string, GameObject>();

    private Dictionary<string, AudioClip> m_SoundEffect = new Dictionary<string, AudioClip>();


    private Dictionary<string, GameObject> m_CharBody = new Dictionary<string, GameObject>();

    private Dictionary<string, GameObject> m_Prefab = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> m_Effect = new Dictionary<string, GameObject>();


    private Dictionary<string, Sprite> m_Sprite = new Dictionary<string, Sprite>();

    private Dictionary<string, Texture2D> m_Textures = new Dictionary<string, Texture2D>();

    private Dictionary<string, Texture2D> m_Tile = new Dictionary<string, Texture2D>();

    private Dictionary<string, TextAsset> m_BuildObject = new Dictionary<string, TextAsset>();
    private Dictionary<string, TextAsset> m_AirObject = new Dictionary<string, TextAsset>();
    private Dictionary<string, Material> m_Material = new Dictionary<string, Material>();
    

    private Dictionary<string, GameObject> m_Missile = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> m_Particle = new Dictionary<string, GameObject>();

    private Dictionary<string, PATHDATA> m_Path = new Dictionary<string, PATHDATA>();



    private Dictionary<int, GameObject> m_ItemObject = new Dictionary<int, GameObject>();

    private Dictionary<int, GameObject> m_CoreEngineObject = new Dictionary<int, GameObject>();

    private Dictionary<int, Texture2D> m_ItemIcon = new Dictionary<int, Texture2D>();
    private Dictionary<int, Texture2D> m_CoreEngineIcon = new Dictionary<int, Texture2D>();

    private Dictionary<string, byte[]> m_AirData = new Dictionary<string, byte[]>();

    private Dictionary<int, Texture> m_FaceImage = new Dictionary<int, Texture>();

    private Dictionary<string, CWJSon> m_kJSon = new Dictionary<string, CWJSon>();

    private Dictionary<string, GameObject> m_kLaser = new Dictionary<string, GameObject>();

    private Dictionary<string, Mesh> m_Mesh = new Dictionary<string, Mesh>();

    



    GameObject m_gTempDir =null;

    public void CloseTempDir()
    {
        Destroy(m_gTempDir);
    }

    public Mesh GetMeshAsset(string szname)
    {
        if (szname == null) return null;
        if (!m_Mesh.ContainsKey(szname.ToUpper()))
        {
            m_Mesh.Add(szname.ToUpper(), Resources.Load("MeshAsset/" + szname) as Mesh);
        }
        return m_Mesh[szname.ToUpper()];

        
    }



    public GameObject GetLaser(string szname)
    {
        if (szname == null) return null;
        if (!m_kLaser.ContainsKey(szname.ToUpper()))
        {
            m_kLaser.Add(szname.ToUpper(), Resources.Load("Laser/" + szname) as GameObject);
        }
        return Instantiate(m_kLaser[szname.ToUpper()]);
    }

    public CWProductionRoot RunProduction(string szname)
    {
        CWProductionRoot kRoot = GetProduction(szname);
        kRoot.Begin();
        return kRoot;
    }
    public CWProductionRoot GetProduction(string szname)//1부터 시작
    {

        if (!m_Production.ContainsKey(szname))
        {
            m_Production.Add(szname, Resources.Load("Production/" + szname) as GameObject);
        }

        GameObject gg = Instantiate(m_Production[szname]);
        if (gg == null)
        {
            Debug.Log("Production error! "  + szname);
            return null;
        }
        CWProductionRoot kRoot = gg.GetComponent<CWProductionRoot>();

        kRoot.transform.SetParent(m_gProductiondir.transform);

        RectTransform tt=kRoot.gameObject.GetComponent<RectTransform>();
        tt.anchoredPosition = Vector2.zero;
        tt.sizeDelta = Vector2.zero;
        kRoot.transform.localScale = Vector3.one;
        kRoot.transform.localRotation = new Quaternion();
        return kRoot;
    }

 

    


    public Material GetMaterial(string szname)
    {
        if (!m_Material.ContainsKey(szname))
        {
            m_Material.Add(szname, Resources.Load("Materials/" + szname) as Material);
        }
        return m_Material[szname];
    }

    public CharBody GetCharBody(int num)
    {
        string szname = num.ToString();
        if (!m_CharBody.ContainsKey(szname))
        {
            m_CharBody.Add(szname, Resources.Load("CharBody/" + szname) as GameObject);
        }
        GameObject gg= Instantiate(m_CharBody[szname]);

        return gg.GetComponent<CharBody>();

    }

    public CharBody GetCharBody(string szfile)
    {
        string szname = szfile;
        if (!m_CharBody.ContainsKey(szname))
        {
            m_CharBody.Add(szname, Resources.Load("CharBody/" + szname) as GameObject);
        }
        GameObject gg = Instantiate(m_CharBody[szname]);

        return gg.GetComponent<CharBody>();

    }

    public TextAsset GetBuildObject(string szname)
    {
        if (!m_BuildObject.ContainsKey(szname))
        {
            m_BuildObject.Add(szname, Resources.Load("BuildObject/" + szname) as TextAsset);
        }
        return m_BuildObject[szname];
    }
    public CWJSon GetJSon(string szname)
    {
        if (!m_kJSon.ContainsKey(szname))
        {
            CWJSon jSon = new CWJSon();
            jSon.LoadGamedata("Gamedata/"+ szname);
            m_kJSon.Add(szname, jSon);
        }
        if (m_kJSon[szname] == null)
        {
            return null;
        }
        return m_kJSon[szname];
    }

    public GameObject GetPrefab(string szname)
    {
        if (!CWLib.IsString(szname)) return null;
        if (!m_Prefab.ContainsKey(szname))
        {
            m_Prefab.Add(szname, Resources.Load("Prefabs/" + szname) as GameObject);
        }
        if(m_Prefab[szname]==null)
        {
            return null;
        }
        return Instantiate(m_Prefab[szname]);


    }
    public GameObject GetEffect(string szname)
    {
        if (!m_Effect.ContainsKey(szname))
        {
            GameObject gg= Resources.Load("Effect/" + szname) as GameObject;
            if(gg==null)
            {
                Debug.Log("effect null "+szname);
            }
            m_Effect.Add(szname, gg);
        }
        if (m_Effect[szname] == null) return null;

        return Instantiate(m_Effect[szname]);
    }

    public GameObject GetParticle(string szname)
    {
        if (szname == null) return null;
        if (!m_Particle.ContainsKey(szname.ToUpper()))
        {
            GameObject aa=Resources.Load("Particle/" + szname) as GameObject;
            if(aa==null)
            {
                Debug.LogError(string.Format("파일 없다!!  {0}",szname));
                return null;
            }
            m_Particle.Add(szname.ToUpper(), aa);
        }
        return Instantiate(m_Particle[szname.ToUpper()]);
    }

    public DOTweenPath GetPVPPath(string szname)
    {
        if (szname == null) return null;
        if (!m_Path.ContainsKey(szname.ToUpper()))
        {
            GameObject aa = Resources.Load("Path/" + szname) as GameObject;
            if (aa == null)
            {
                Debug.LogError(string.Format("파일 없다!!  {0}", szname));
                return null;
            }

            PATHDATA kData = new PATHDATA();
            kData.m_kDOTween = aa.GetComponent<DOTweenPath>();
            kData.m_fDuration = kData.m_kDOTween.duration;

            m_Path.Add(szname.ToUpper(), kData);
        }

        m_Path[szname.ToUpper()].m_kDOTween.duration = m_Path[szname.ToUpper()].m_fDuration*4 ;
        DOTweenPath ret = Instantiate(m_Path[szname.ToUpper()].m_kDOTween);

        m_Path[szname.ToUpper()].m_kDOTween.duration = m_Path[szname.ToUpper()].m_fDuration ;
        return ret;

    }


    public DOTweenPath GetPath(string szname,float fSpeed)
    {
        if (szname == null) return null;
        if (!m_Path.ContainsKey(szname.ToUpper()))
        {
            GameObject aa = Resources.Load("Path/" + szname) as GameObject;
            if (aa == null)
            {
                Debug.LogError(string.Format("파일 없다!!  {0}", szname));
                return null;
            }

            PATHDATA kData = new PATHDATA();
            kData.m_kDOTween = aa.GetComponent<DOTweenPath>();
            kData.m_fDuration = kData.m_kDOTween.duration;

            m_Path.Add(szname.ToUpper(), kData);
        }
        if (m_gTempDir == null)
        {
            m_gTempDir = new GameObject();
            m_gTempDir.transform.parent = transform;
            m_gTempDir.transform.localPosition = Vector3.zero;
        }

        m_Path[szname.ToUpper()].m_kDOTween.duration = m_Path[szname.ToUpper()].m_fDuration / fSpeed;
        DOTweenPath ret= Instantiate(m_Path[szname.ToUpper()].m_kDOTween);

        m_Path[szname.ToUpper()].m_kDOTween.duration = m_Path[szname.ToUpper()].m_fDuration;

        ret.transform.parent = m_gTempDir.transform;

        return ret;

    }

    public GameObject GetMissile(string szname)
    {
        if (szname == null) return null;
        if (!m_Missile.ContainsKey(szname.ToUpper()))
        {
            m_Missile.Add(szname.ToUpper(), Resources.Load("Weapon/" + szname) as GameObject);
        }
        return Instantiate(m_Missile[szname.ToUpper()]);
    }

    public GameObject GetCoreEngineObject(int nLevel)
    {

        int nID = nLevel / 10+1;
        
        if (nID == 0) return null;
        if (!m_CoreEngineObject.ContainsKey(nID))
        {
            string szname = string.Format("CoreEngine{0}", nID);
            if (CWLib.IsString(szname))
            {
                m_CoreEngineObject.Add(nID, Resources.Load("ItemObject/" + szname) as GameObject);
            }
            else return null;

        }
        if (m_CoreEngineObject[nID] == null)
        {
            CWUnityLib.DebugX.Log("");
        }


        return Instantiate(m_CoreEngineObject[nID]);
    }


    public GameObject GetItemObject(int nID)
    {
        if (nID == 0) return null;
        if (!m_ItemObject.ContainsKey(nID))
        {
            string szname = CWArrayManager.Instance.GetItemFileName(nID);
            if (CWLib.IsString(szname))
            {
                m_ItemObject.Add(nID, Resources.Load("ItemObject/" + szname) as GameObject);
            }
            else return null;
            
        }
        //if(m_ItemObject[nID]==null)
        //{
        //    CWUnityLib.DebugX.Log("");
        //}
      

        return Instantiate(m_ItemObject[nID]);
    }

    //m_CoreEngineIcon
    public Texture2D GetCoreEngineIcon(int nLevel)
    {
        int nID = nLevel / 10 + 1;

        if (nID == 0)
        {
            return GetTexture("Empty");
        }
        if (!m_CoreEngineIcon.ContainsKey(nID))
        {
            string szname = "CoreEngine" + nID.ToString();
            m_CoreEngineIcon.Add(nID, Resources.Load("ItemIcon/" + szname) as Texture2D);
        }
        return m_CoreEngineIcon[nID];
    }

    public Texture2D GetItemIcon(int nID)
    {

        
        if (nID == 0)
        {
            return GetTexture("Empty");
        }
        if (!m_ItemIcon.ContainsKey(nID))
        {


            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nID);

            m_ItemIcon.Add(nID, Resources.Load("ItemIcon/" + gData.sziconname) as Texture2D);
        }
        return m_ItemIcon[nID];
    }
    #region 페이스 관련
    
    public void FaceReceiveData(JObject jData)
    {
        if (jData["Result"].ToString() == "ok")
        {
            // 메모리 어태치를 한다
            CWJSon jSon = new CWJSon(jData);
            int nID = jSon.GetInt("_id");
            byte[] fileData = jSon.GetBytes("Filedata");
            Texture2D Tx = new Texture2D(CWGlobal.FACEIMAGESIZE, CWGlobal.FACEIMAGESIZE);
            Tx.LoadImage(fileData);

            CWLib.SaveImage(Tx, nID.ToString());
            if (!m_FaceImage.ContainsKey(nID))
            {
                m_FaceImage.Add(nID, Tx);
            }


        }
        else
        {
            

            CWJSon jSon = new CWJSon(jData);
            int nID = jSon.GetInt("_id");


            if (!m_FaceImage.ContainsKey(nID))
            {
                m_FaceImage.Add(nID, GetTexture("chobo_1"));
            }

            print(string.Format("faill!! {0}", nID));
        }

    }

    public Texture GetFaceTexture(int nID)
    {
        if (m_FaceImage.ContainsKey(nID))
        {
            return m_FaceImage[nID];
        }
        Texture2D kTex= CWLib.LoadImage(nID.ToString(), CWGlobal.FACEIMAGESIZE, CWGlobal.FACEIMAGESIZE);
        if(kTex)
        {
            m_FaceImage.Add(nID, kTex);
            return kTex;
        }

        m_kFaceReceiveList.Add(nID);

        return null;

    }
    List<int> m_kFaceReceiveList = new List<int>();

    
    IEnumerator FaceDownLoadRun()
    {
        

        while(true)
        {
            if (m_kFaceReceiveList.Count > 0)
            {
                
                {
                    CWSocketManager.Instance.AskServerFile(m_kFaceReceiveList[0], FaceReceiveData, "FaceReceiveData");//
                    
                    
                   // Debug.Log("SendData "  + m_kFaceReceiveList[0]);

                    m_kFaceReceiveList.RemoveAt(0);

                }
            }
            yield return new WaitForSeconds(0.1f);

        }

    }

    void LoadParticle()
    {
        //if (!m_Particle.ContainsKey(szname.ToUpper()))
        //{
        //    m_Particle.Add(szname.ToUpper(), Resources.Load("Particle/" + szname) as GameObject);
        //}

        GameObject[] aa = Resources.LoadAll<GameObject>("Particle");

        foreach(var v in aa)
        {
            m_Particle.Add(v.name.ToUpper(), v);
        }


    }

    public override void Create()
    {

        AudioClip [] aa= Resources.LoadAll<AudioClip>("EffectSound");

        foreach(var v in aa)
        {
            m_SoundEffect.Add(v.name, v);
        }
        
        if (!CWGlobal.g_bEditmode)
        {
            StartCoroutine("FaceDownLoadRun");
        }
        LoadParticle();



        base.Create();
    }
    #endregion

    #region  비행기 관련 데이타 

    public TextAsset GetAirObject(string szname)
    {
        if (!m_AirObject.ContainsKey(szname))
        {
            m_AirObject.Add(szname, Resources.Load("AirCraft/" + szname) as TextAsset);
        }

        return m_AirObject[szname];
    }



    #endregion

    public Sprite GetSprite(string szname)
    {
        if (szname == null) return null;
        if (szname.Length == 0) return null;
        if (!m_Sprite.ContainsKey(szname))
        {
            m_Sprite.Add(szname, Resources.Load<Sprite>("Sprite/" + szname) as Sprite);
        }
        return m_Sprite[szname];
    }
    public Texture2D GetTexture(string szname)
    {
       
        if (szname == null) return GetTexture("Empty");
        if (szname.Length == 0) return GetTexture("Empty");

        if (!m_Textures.ContainsKey(szname))
        {
            Texture2D ktex = Resources.Load("Texture/" + szname) as Texture2D;
            m_Textures.Add(szname, ktex);
        }
        return m_Textures[szname];
    }

    public Texture2D GetTile(string szname)
    {
        if (szname == null) return null;
        if (szname.Length == 0) return null;
        if (!m_Tile.ContainsKey(szname))
        {
            m_Tile.Add(szname, Resources.Load("tile/" + szname) as Texture2D);
        }
        return m_Tile[szname];
    }
    public MoveCoinUI MoveCoin(COIN nCoin,int Count,Transform m_gStartTrans )
    {
        GameObject kCoin = null;
        if (nCoin==COIN.GOLD)
        {
            kCoin = GetPrefab("MoveGold");//Resources.Load("Prefabs/MoveGold") as MoveCoinUI;
        }
        if (nCoin == COIN.GEM)
        {
            kCoin = GetPrefab("MoveGem");
        }
        if (nCoin == COIN.TICKET)
        {
            kCoin = GetPrefab("MoveTicket");
        }

        if (kCoin==null)
        {
            return null;
        }

        MoveCoinUI kk = kCoin.GetComponent<MoveCoinUI>();

        kk.m_nCount = Count;
        kk.transform.parent = m_gStartTrans;
        kk.transform.localPosition = Vector3.zero;
        kk.transform.localScale = Vector3.one;
        kk.transform.localRotation = new Quaternion();

        return kk;
    }

    public MoveCoinUI MoveObjectStar(GameObject gObject, int Count, Transform m_gStartTrans)
    {
        GameObject kCoin = GetPrefab("MoveStar");

        MoveCoinUI kk = kCoin.GetComponent<MoveCoinUI>();
        kk.m_gTarget = gObject;
        kk.m_nCount = Count;
        kk.transform.parent = m_gStartTrans;
        kk.transform.localPosition = Vector3.zero;
        kk.transform.localScale = Vector3.one;
        kk.transform.localRotation = new Quaternion();

        return kk;
    }

    #region 사운드관련

    public void PlaySound(string szname, GameObject gTarget = null, float fVolumnRate = 1f)
    {
        if (!CWLib.IsString(szname)) return;
        if (!m_SoundEffect.ContainsKey(szname)) return;

//        Debug.Log(string.Format("sound {0}  ", szname));

        CWLib.PlaySound(m_SoundEffect[szname], gTarget, fVolumnRate);
    }


    #endregion

}
