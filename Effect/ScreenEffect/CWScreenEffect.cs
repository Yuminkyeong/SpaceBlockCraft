using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;


public class CWScreenEffect : CWSingleton<CWScreenEffect>
{


    public GameObject m_visible;

    // 그대로 보이기, 한점으로 모이게 하기, 원래 위치로, 
    // 한점으로 모인다, 팽창한다
    public enum MOVETYPE {NONE, ZERO, Expansion,Scale_Plus,Scale_minus, Scale_None };
    

    public int m_nColumn = 10;// 몇개로 나누나

    

    int m_nWidth;
    int m_nHeight;
    public GameObject m_gPrefab;


    class SObject
    {
        public GameObject m_gObject;
        public Vector3 m_vStart;
        

    }
    

    Dictionary<int, List<SObject>> m_kScreenArray = new Dictionary<int, List<SObject>>();

    void Clear()
    {
        foreach(var v in m_kScreenArray)
        {
            foreach(var vv in v.Value)
            {
                GameObject.Destroy(vv.m_gObject);
                
            }

            v.Value.Clear();
        }
        m_kScreenArray.Clear();

    }

  
    IEnumerator IRun()
    {
        //Clear();
        yield return new WaitForEndOfFrame();

        CaptrueScreen(0);
        
        Play(0, MType, fTime);
    }
    // 화면을 캡쳐한다 : 나중에 파일로 저장도 고려 
    Texture2D m_kCapture;
    public void CaptrueScreen(int num)
    {
        m_nWidth = Screen.width / m_nColumn;
        m_nHeight = Screen.height / m_nColumn;

        if(!m_kScreenArray.ContainsKey(num))
        {
            if(m_kCapture==null)
            {
                m_kCapture = ScreenCapture.CaptureScreenshotAsTexture();
                m_kImage.texture = m_kCapture;
            }
            

            //m_kImage.texture = kTex;
            
            MakeTile(num, m_kCapture);
        }
    }
    // 타일로 만들기 
    void CopyPixel(Texture2D kTarget, int sx, int sy, int dx, int dy, Texture2D pSource)
    {

        for (int y = 0; y < dy; y++)
        {
            for (int x = 0; x < dx; x++)
            {
                Color kColor = pSource.GetPixel(sx+x,sy+y);
                kTarget.SetPixel(x,y, kColor);
            }

        }

        kTarget.Apply();
    }
    // 텍스쳐 좌표를 스크린 좌표로 일치 시킨다 
    Vector2 GetPos( int x,int y)
    {
        int tx = x* m_nWidth;
        int ty = y*m_nHeight;

        Vector2 vRet = new Vector2();
        vRet.x = tx - (Screen.width / 2f)+m_nWidth/2; 
        vRet.y = ty - (Screen.height / 2f)+m_nHeight/2;
        return vRet;

    }
    void MakeTile(int num, Texture2D kTex)
    {

        float ff= Game_App.Instance.Canvas_Window.transform.localScale.x;


        float frate= 1f/ff;

        List<SObject> kList = new List<SObject>();
        for (int y=0;y<m_nColumn;y++)
        {
            for (int x = 0; x < m_nColumn; x++)
            {
                Texture2D tile = new Texture2D(m_nWidth,m_nHeight);



                //  Graphics.CopyTexture(kTex, 0, 0, x * m_nWidth, y * m_nHeight, m_nWidth, m_nHeight, tile, 0, 0, 0, 0);
                CopyPixel(tile, x * m_nWidth, y * m_nHeight, m_nWidth, m_nHeight, kTex);


                  GameObject gTemp = GameObject.Instantiate(m_gPrefab);
                gTemp.name = string.Format("{0}_{1}",x,y);
                gTemp.transform.SetParent(transform);
                RectTransform rt =gTemp.GetComponent<RectTransform>();

                rt.anchoredPosition =GetPos(x, y) * frate;

                rt.sizeDelta = new Vector2(m_nWidth*frate, m_nHeight * frate);
                rt.localScale = Vector3.one;

                RawImage rr = gTemp.GetComponent<RawImage>();
                rr.texture = tile;

                SObject kk = new SObject();
                kk.m_gObject = gTemp;
                kk.m_vStart = rt.anchoredPosition;
                kList.Add(kk);
            }

        }
        m_kScreenArray.Add(num, kList);
    }
    // 점으로 뭉치기

    // 펴져나가기 

    public void Play(int num, MOVETYPE kType,float fTime)
    {

        
        foreach(var v in m_kScreenArray[num])
        {
            // 팽창
            if(kType== MOVETYPE.Expansion)
            {
                m_fSpeed = 0.25f;
                Vector3 vdir =v.m_vStart -  Vector3.zero;
                vdir.Normalize();
                v.m_gObject.transform.CWMove(vdir * 2500f, m_fSpeed);
                v.m_gObject.transform.DOScale(1f, fTime);
            }
            // 수축
            if (kType == MOVETYPE.ZERO)
            {
                //v.m_gObject.transform.DOLocalMove(Vector3.zero, fTime);

                m_fSpeed = 0.25f;
                v.m_gObject.transform.CWMove(Vector3.zero, m_fSpeed);
                v.m_gObject.transform.DOScale(0.0f, 3);
            }
            if (kType == MOVETYPE.NONE)
            {
                //v.m_gObject.transform.DOLocalMove(v.m_vStart, fTime);
                m_fSpeed = 5f;
                v.m_gObject.transform.CWMove(v.m_vStart, m_fSpeed);
                v.m_gObject.transform.DOScale(1f, fTime);
            }
            if(kType == MOVETYPE.Scale_Plus)
            {
                v.m_gObject.transform.DOScale(2, fTime);
            }
            if (kType == MOVETYPE.Scale_minus)
            {
                v.m_gObject.transform.DOScale(0.0f, fTime);
            }
            if (kType == MOVETYPE.Scale_None)
            {
                v.m_gObject.transform.DOScale(1f, fTime);
            }

        }

        m_visible.SetActive(true);
        //  ParticleSystem;
    }

    #region 테스트 

    public RawImage m_kImage;
    public float fTime=2f;
    public float m_fSpeed = 10f;
    
    public MOVETYPE MType= MOVETYPE.ZERO;
    public void Test()
    {
        
        
        
    }
/*
    private void Update()
    {
        if(Input.GetKeyUp(KeyCode.A))
        {
            MType = MOVETYPE.ZERO;
            StartCoroutine("IRun");
        }
        if (Input.GetKeyUp(KeyCode.Z))
        {
            MType = MOVETYPE.Expansion;
            StartCoroutine("IRun");
        }

        if (Input.GetKeyUp(KeyCode.D))
        {
            MType = MOVETYPE.NONE;
            StartCoroutine("IRun");
        }

        if (Input.GetKeyUp(KeyCode.Q))
        {
            MType = MOVETYPE.Scale_minus;
            StartCoroutine("IRun");
        }
        if (Input.GetKeyUp(KeyCode.W))
        {
            MType = MOVETYPE.Scale_Plus;
            StartCoroutine("IRun");
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            MType = MOVETYPE.Scale_None;
            StartCoroutine("IRun");
        }


    }
*/
    #endregion

}
