using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWStruct;
using CWUnityLib;
public class RadarUI : CWSingleton<RadarUI>
{

    public GameObject m_BlockPysick;
    public float DeltaYaw = 0;

    public Image m_kSpot;

    public RectTransform m_kHero;
    public RectTransform m_tdir;
    public int UISIZE = 300;

    
    protected Dictionary<int, Image> m_kData = new Dictionary<int, Image>();

    void Start()
    {
        
    }

    Vector3 GetPosition()
    {
        if(GamePlay.Instance.CharMode)
        {
            return CWChHero.Instance.GetPosition();
        }
        return CWHero.Instance.GetPosition();

    }
    float GetYaw()
    {
        if (GamePlay.Instance.CharMode)
        {
            return CWChHero.Instance.GetYaw();
        }
        return CWHero.Instance.GetYaw();

    }

    void Update()
    {
        if (GamePlay.Instance == null) return;
        if (!GamePlay.Instance.IsShow()) return;
        Vector3 rr = m_kHero.eulerAngles;
        rr.z = GetDir();
        m_kHero.eulerAngles = rr;

        
        Vector3 vh = GetPosition();

        // 2D로 변환
        float hx = -UISIZE + (UISIZE- ConvertPOSY(vh.x));
        float hy = -UISIZE + (UISIZE - ConvertPOSX(vh.z));
        Vector2 vpos =  new Vector2(hx,hy);
        m_tdir.anchoredPosition = vpos;


    }
    Image CreateSpot(int x,int y,Vector3 vPos)
    {

        //GameObject gg = Instantiate(m_BlockPysick);
        //gg.transform.position = vPos;// 표식 


        int SELLWIDTH = CWMapManager.Instance.m_kSelectMap.WORLDSIZE;
        int xx = (int)vPos.x;
        int yy = (int)vPos.y;
        int zz = (int)vPos.z;
        int num = zz * SELLWIDTH * SELLWIDTH + yy * SELLWIDTH + xx;
        if (!m_kData.ContainsKey(num))
        {
            Image kk = Instantiate(m_kSpot);
            kk.gameObject.SetActive(true);
            RectTransform tt = kk.GetComponent<RectTransform>();
            tt.SetParent(m_tdir);
            tt.anchoredPosition = new Vector2(x, y);
            tt.localScale = Vector3.one;
            m_kData.Add(num, kk);
            return kk;
        }

        return m_kData[num];
    }
    float GetDir()
    {
        
        return (360 - (GetYaw() + DeltaYaw));
        //return 0;
    }
    int ConvertPOSX(float x)
    {
        // 전체 크기 
        int mapsize = CWMapManager.Instance.m_kSelectMap.WORLDSIZE;
        int uisize = UISIZE;
        float fRate = (float)uisize / mapsize;
        return (int)( (mapsize-x) * fRate);


    }
    int ConvertPOSY(float x)
    {
        // 전체 크기 
        int mapsize = CWMapManager.Instance.m_kSelectMap.WORLDSIZE;
        int uisize = UISIZE;
        float fRate = (float)uisize / mapsize;
        return (int)((mapsize - x) * fRate);
    }

    void Clear()
    {
        foreach(var v in m_kData)
        {
            Destroy(v.Value.gameObject, 0.1f);
        }
        m_kData.Clear();
    }
    void FindMine()
    {
        Clear();

        
        foreach (var v in CWMapManager.Instance.m_kSelectMap.m_vRes3block)
        {
            int x = ConvertPOSY(v.x);
            int y = ConvertPOSX(v.z);
            CreateSpot(x,y,v);
        }

    }
    private void OnEnable()
    {
        if (GamePlay.Instance == null) return;
        if (!GamePlay.Instance.IsShow()) return;

        FindMine();

    }
    private void OnDisable()
    {
        Clear();
    }
    public void UpdateBlock(int x,int y,int z)
    {
        int SELLWIDTH = CWMapManager.Instance.m_kSelectMap.WORLDSIZE;
        int num = z * SELLWIDTH * SELLWIDTH + y * SELLWIDTH + x;
        if(m_kData.ContainsKey(num))
        {
            Destroy(m_kData[num].gameObject);
            m_kData.Remove(num);
        }
    }
}
