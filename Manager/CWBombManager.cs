using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using UnityEngine.UI;
using DG.Tweening;
using CWUnityLib;
public class CWBombManager : CWManager<CWBombManager>
{

    public Vector2 TempDelta;
    public float m_fSpeed = 7f;
    public Texture2D m_Texture;
    public GameObject m_gBombBox;
    public RectTransform m_gUIIcon;
    public RectTransform m_tUIDir;
    int COUNT = 200;



    List<ParticleSystem> m_kPSystemList = new List<ParticleSystem>();
    List<ParticleSystem.Particle[]> m_kPartlcesList = new List<ParticleSystem.Particle[]>();
    

//    ParticleSystem.Particle[] m_kParticles;
  //  public ParticleSystem m_kParticleSystem;


    class BLOCKPARTICLE
    {
        public GameObject m_gObject;
        public RectTransform m_gUIIcon;
        public int m_nBock;
    }


    List<BLOCKPARTICLE> m_kList = new List<BLOCKPARTICLE>();

    bool m_bPlay = false;
    void ResetUI()
    {
        foreach (var v in m_kList)
        {
            v.m_gUIIcon.gameObject.SetActive(false);
        }
        if(EquipInvenList.Instance)
            EquipInvenList.Instance.UpdateData();
    }
    void Timer()
    {
        if (!EquipInvenList.Instance) return;

        m_bPlay = false;
        foreach (var v in m_kList)
        {
            
            RawImage rr = v.m_gUIIcon.GetComponent<RawImage>();
            if(rr!=null)
            {
                if(v.m_nBock>0)
                {
                    int nItem = CWArrayManager.Instance.GetItemFromBlock(v.m_nBock);

                    if (EquipInvenList.Instance.IsShow())
                    {
                        Vector2 vv2 = EquipInvenList.Instance.GetNewBlock(nItem);
                        if (vv2 != Vector2.zero)
                        {
                            rr.texture = CWResourceManager.Instance.GetItemIcon(nItem);

                            v.m_gUIIcon.DOScale(0.2f, 0f).OnComplete(()=> {
                                v.m_gUIIcon.DOAnchorPos(vv2, 0.6f);
                                v.m_gUIIcon.DOScale(1, 0.4f);
                                v.m_gUIIcon.gameObject.SetActive(true);
                            });
                                
                            //v.m_gUIIcon.anchoredPosition = CWLib.ConvertUIPosition(v.m_gObject);
                        }

                    }

                }

            }
            v.m_gObject.SetActive(false);
            v.m_nBock = 0;


        }

        Invoke("ResetUI", 0.6f);
        // 2D 오브젝트 변환 


    }
    bool BPlay
    {
        get
        {
            return m_bPlay;
        }
        set
        {
            m_bPlay = value;
            Invoke("Timer", 1.5f);
        }
    }
    int m_nCount = 0;
    void GetUV(ref Vector2 vUV, byte nBlock)
    {
        int x = CWArrayManager.Instance.m_kBlock[nBlock].x;
        int y = CWArrayManager.Instance.m_kBlock[nBlock].y;

        float sx = (float)x;
        float sy = (float)y;
        vUV.x = sx / 64f;
        vUV.y = sy / 64f;

    }

    void UpdateUV(GameObject gg, Texture2D tex, int nBlock,Color kcolor)
    {


        Renderer rr = gg.GetComponent<Renderer>();
        Vector2 vv = new Vector2();
        GetUV(ref vv, (byte)nBlock);
        rr.material = new Material(Shader.Find("CWShader"));
        rr.material.mainTextureOffset = vv;
        rr.material.mainTextureScale = new Vector2(1f / 64f, 1f / 64f);
        rr.material.mainTexture = tex;
        rr.material.color = kcolor;
    }


    public override void Create()
    {
        if (m_gBombBox == null) return;

        COUNT = 200;
        if (CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.BAD)
        {
            COUNT = 100;
        }

        for (int i = 0; i < COUNT; i++)
        {

            BLOCKPARTICLE bb = new BLOCKPARTICLE();

            RectTransform gUI = Instantiate(m_gUIIcon);
            gUI.SetParent(m_tUIDir);
            gUI.anchoredPosition = Vector2.zero;
            gUI.localScale = Vector3.one;
            gUI.gameObject.SetActive(false);
            GameObject gg = Instantiate(m_gBombBox);
            gg.transform.parent = transform;
            gg.SetActive(false);
            bb.m_gObject = gg;
            bb.m_nBock = 0;
            bb.m_gUIIcon = gUI;
            m_kList.Add(bb);
        }


        var systems = GetComponentsInChildren<ParticleSystem>();
        foreach(var v in systems)
        {
            m_kPSystemList.Add(v);
        }
        for (int i=0;i< m_kPSystemList.Count;i++)
        {
            ParticleSystem.Particle [] kParticles = new ParticleSystem.Particle[COUNT];
            m_kPartlcesList.Add(kParticles);
        }
        


        base.Create();
    }

    void SetParticle(ParticleSystem ksystem, ParticleSystem.Particle[] kParticles, CWAirObject kObject,int nCount,int offset)
    {

        ParticleSystem.MainModule main = ksystem.main;
        main.maxParticles = nCount;
        ksystem.Emit(nCount);
        ksystem.GetParticles(kParticles);
        int i = 0;
        Dictionary<int, BlockData> kData = kObject.GetData();

        foreach (var v in kData)
        {
            float x = kObject.transform.position.x + kObject.GetSellX(v.Key) - kObject.SELLWIDTH / 2;
            float y = kObject.transform.position.y + kObject.GetSellY(v.Key);
            float z = kObject.transform.position.z + kObject.GetSellZ(v.Key) - kObject.SELLWIDTH / 2;

            kParticles[i].position = new Vector3(x, y, z);
            m_kList[i+ offset].m_gObject.SetActive(true);
            int nBlock = CWArrayManager.Instance.GetBlockFromItem(v.Value.nBlock);
            Color kColor= CWGlobal.GetColor((COLORNUMBER)v.Value.nColor);
            UpdateUV(m_kList[i + offset].m_gObject, m_Texture, nBlock,kColor);


            i++;
            if (i >= nCount) break;
        }

        ksystem.SetParticles(kParticles, nCount);

    }

    public void PlayObject(CWAirObject kObject)
    {
        if (m_bPlay) return;

        m_nCount = kObject.GetBlockCount();
        if (m_nCount >= COUNT)
        {
            m_nCount = COUNT;
        }
        int offset = 0;
        for(int i=0;i< m_kPSystemList.Count; i++)
        {
            SetParticle(m_kPSystemList[i], m_kPartlcesList[i], kObject, m_nCount/ m_kPSystemList.Count, offset);
            offset += (m_nCount / m_kPSystemList.Count);
        }
        


        BPlay = true;

    }

    bool m_bBeginMap = false;
    public void Begin_Map()
    {
        if (m_bPlay) return;
        m_bBeginMap = true;
        m_nCount = 0;

        int nRange = COUNT / m_kPSystemList.Count;
        for (int j = 0; j < m_kPSystemList.Count; j++)
        {
            ParticleSystem.MainModule main = m_kPSystemList[j].main;
            main.maxParticles = nRange;
            m_kPSystemList[j].Emit(nRange);
            m_kPSystemList[j].GetParticles(m_kPartlcesList[j]);
        }



    }
    public void Reg_Map(int x, int y, int z, int nBlock,bool bTakeout)
    {
        if (!m_bBeginMap) return;

        if (m_nCount >= COUNT) return;

        int tRange = COUNT / m_kPSystemList.Count;

        Vector3 vPos = CWMapManager.ConvertPosFloat(new Vector3Int(x, y, z));

        int offset = m_nCount/tRange;
        ParticleSystem.Particle[] kParticle = m_kPartlcesList[offset];

        kParticle[m_nCount%tRange].position = vPos;

        if(bTakeout)
        {
            m_kList[m_nCount].m_gObject.SetActive(true);
            m_kList[m_nCount].m_nBock = nBlock;
        }
        else
        {
            m_kList[m_nCount].m_gObject.SetActive(false);
            m_kList[m_nCount].m_nBock = 0;
        }


        UpdateUV(m_kList[m_nCount].m_gObject, m_Texture, nBlock,Color.white);
        m_nCount++;

    }
    public void End_Map()
    {
        if (!m_bBeginMap) return;



        int nRange = m_nCount / m_kPSystemList.Count;
        for (int j = 0; j < m_kPSystemList.Count; j++)
        {

          
            m_kPSystemList[j].SetParticles(m_kPartlcesList[j], nRange);
        }
        BPlay = true;
        m_bBeginMap = false;
    }
    public void ResetObject()
    {
        foreach(var v in m_kList)
        {
            v.m_gObject.SetActive(false);
        }
    }
    void Run()
    {
        Vector2 vTemp = new Vector2(205.7f,-148.8f)+ TempDelta;

        int k = 0;
        int tcnt = m_nCount / m_kPSystemList.Count;
        for (int j=0;j< m_kPSystemList.Count;j++)
        {
            m_kPSystemList[j].GetParticles(m_kPartlcesList[j]);

            Vector3 rr = Random.insideUnitSphere;
            for (int i = 0; i < tcnt; i++)
            {
                m_kList[k].m_gObject.transform.position = m_kPartlcesList[j][i].position;//;new Vector3(m_kParticles[i].position.x, m_kParticles[i].position.z, m_kParticles[i].position.y);


                Vector3 v = m_kList[k].m_gObject.transform.eulerAngles;
                v += rr * Time.deltaTime * m_fSpeed;
                m_kList[k].m_gObject.transform.eulerAngles = v;


                
                m_kList[k].m_gUIIcon.anchoredPosition = CWLib.ConvertUIPosition(m_kList[k].m_gObject) + vTemp;



                k++;
            }


        }


    }
    private void Update()
    {
        if (!m_bPlay) return;

        Run();

    }


}
