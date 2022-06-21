using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
using Newtonsoft.Json.Linq;

public class CWCaptureBox : CWManager<CWCaptureBox>
{
    GameObject m_gSelectBox;
    public GameObject m_gSelectDir;
    public GameObject m_gObjectBox;
    public CWShowAirObject m_gAirObject;
    public GameObject m_gAirDir;

    public Camera m_kAirCam;
    public Camera m_kItemCam;

    public Camera m_kSelectCam;

    public CWAirObject m_gCapture;

    public override void Create()
    {
        base.Create();
    }
    public void ZoomCameara(float ff)//0~ 1
    {
        Camera cc = gameObject.GetComponentInChildren<Camera>(); //Game_App.Instance.m_gCaptureCamera;
        if (cc!=null)
        {
            cc.fieldOfView = 10+100f*ff;
        }
    }
    public void ShowItem(int nItemID)
    {
        m_kSelectCam = m_kItemCam;
        m_gAirDir.SetActive(false);
        m_gObjectBox.SetActive(true);
        MakeSelectBox(nItemID);
        Camera cc = m_kItemCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }

    }
    public void ShowAirBuffer(byte[] bBuffer)
    {
        m_kSelectCam = m_kAirCam;

        m_gAirDir.gameObject.SetActive(true);
        m_gObjectBox.SetActive(false);

        m_gAirObject.m_nLayer = GameLayer.Edit;
        m_gAirObject.CopyBuffer(bBuffer);

        CWObject.g_kSelectObject = m_gAirObject;

        CWLib.SetGameObjectLayer(m_gAirObject.gameObject, m_gAirObject.gameObject.layer);
        Camera cc = m_kAirCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }

        m_gCapture = m_gAirObject;

    }

    public void ShowUserGoods(int nID)
    {
        m_kSelectCam = m_kAirCam;
        m_gAirDir.gameObject.SetActive(true);
        m_gObjectBox.SetActive(false);

        


        m_gAirObject.ShowUserGoods(nID);
        CWObject.g_kSelectObject = m_gAirObject;


        Camera cc = m_kAirCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }


        m_gCapture = m_gAirObject;
    }
    public void ShowAirPlane(string szname )
    {
        m_kSelectCam = m_kAirCam;
        m_gAirDir.gameObject.SetActive(true);
        m_gObjectBox.SetActive(false);

        m_gAirObject.m_nLayer = GameLayer.Edit;
        
        m_gAirObject.Load(szname);

        m_gAirObject.FixPos();// 사이즈를 맞춘다

        CWObject.g_kSelectObject = m_gAirObject;

        CWLib.SetGameObjectLayer(m_gAirObject.gameObject, m_gAirObject.gameObject.layer);
        Camera cc = m_kAirCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }
        CWHero.Instance.gameObject.SetActive(false);
        


        m_gCapture = m_gAirObject;
    }
    
    public void ShowUser(int nUserID)
    {
        m_kSelectCam = m_kAirCam;
        m_gAirDir.gameObject.SetActive(true);
        m_gObjectBox.SetActive(false);

        m_gAirObject.ShowUser(nUserID);

        CWObject.g_kSelectObject = m_gAirObject;

        Camera cc = m_kAirCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }

        CWHero.Instance.gameObject.SetActive(false);
        

        

    }

    public void RefreshAirPlane()
    {
        m_gObjectBox.SetActive(false);
        CWLib.SetGameObjectLayer(m_gAirObject.gameObject, m_gAirObject.gameObject.layer);

    }

    public void ShowHero()
    {
        m_kSelectCam = m_kAirCam;
        m_gAirDir.SetActive(true);
        m_gObjectBox.SetActive(false);

        m_gAirObject.m_nLayer = GameLayer.Edit;
        m_gAirObject.CopyBuffer(CWHero.Instance.GetBuffer());

        m_gAirObject.FixPos();// 사이즈를 맞춘다

        CWObject.g_kSelectObject = m_gAirObject;

        CWLib.SetGameObjectLayer(m_gAirObject.gameObject, m_gAirObject.gameObject.layer);
        Camera cc = m_kAirCam;
        if (cc != null)
        {
            cc.fieldOfView = 60;
        }
        CWHero.Instance.gameObject.SetActive(false);
        

        m_gCapture = m_gAirObject;

        

    }
    public void ClearAir()
    {
        m_gAirDir.gameObject.SetActive(false);
    }

    public void Close()
    {
        if(m_gAirObject)
            m_gAirObject.transform.localScale = Vector3.one;
        if (m_gSelectBox != null)
        {
            GameObject.Destroy(m_gSelectBox);
        }
        m_gObjectBox.SetActive(false);
        m_gAirDir.SetActive(false);
        CWHero.Instance.gameObject.SetActive(true);
        
    }
    void MakeSelectBox(int nItemID)
    {

        if (m_gSelectBox != null)
        {
            GameObject.Destroy(m_gSelectBox);
        }
        if (nItemID == 0) return;
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItemID);

        ValueUI.g_kSelectItemData = gData;// 선택된 

        if (gData.type == "shipblock"|| gData.type == "color" || gData.type == "보석")
        {
            m_gSelectBox = CWResourceManager.Instance.GetPrefab("SelectBlock");
            Texture2D tex = CWResourceManager.Instance.GetTexture("terrain");

            //int nBlock = CWArrayManager.Instance.GetBlockToItem(gData.nblock);

            //Color cc = CWGlobal.GetColor((COLORITEM)gData.Color);
            CWLib.UpdateUV(m_gSelectBox, tex, gData.nblock);
            //CWLib.SetColorChild(m_gSelectBox.transform, cc);

        }
        else
        {
            m_gSelectBox = CWResourceManager.Instance.GetItemObject(gData.nID);
        }
        if (m_gSelectBox == null) return;

        m_gSelectBox.transform.parent = m_gSelectDir.transform;
        m_gSelectBox.transform.localPosition = Vector3.zero;
        m_gSelectBox.transform.localScale = Vector3.one;
        m_gSelectBox.transform.localEulerAngles = Vector3.zero;

        CWLib.SetGameObjectLayer(m_gSelectDir, LayerMask.NameToLayer("ItemBox"));



    }

    public byte[] GetCaptureImage()
    {
        Camera cc = m_kSelectCam;
        return CWLib.MakeImageBuffer( cc, cc.backgroundColor);
    }
    public Texture2D GetCaptureTexture()
    {
        Camera cc = m_kSelectCam;
        return CWLib.MakeTexture( cc, cc.backgroundColor);

    }

    #region 회전 컨트롤

    Vector3 m_vPos;
    public float m_fRotSpeed = 10f;
    public void RotateBegin()
    {
        TweenRotation tt = m_gAirObject.gameObject.GetComponent<TweenRotation>();
        tt.enabled = true;

    }
    public void OnCMouseDown()
    {

        m_vPos = Input.mousePosition;
        TweenRotation tt=m_gAirObject.gameObject.GetComponent<TweenRotation>();
        tt.enabled = false;
        
    }
    public void OnCMouseDrag()
    {
        Vector3 vmouse = new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0);
        Vector3 vDir = vmouse - m_vPos;
        vDir.Normalize();
        RotateFuc(vDir);

    }
    void RotateYaw(float fAnlge)
    {
        if (fAnlge == 0) return;
        if (fAnlge > 90) fAnlge = 90;
        if (fAnlge < -90) fAnlge = -90;

        Vector3 vv = m_gAirObject.transform.localEulerAngles;
        vv.y += fAnlge * Time.deltaTime * m_fRotSpeed;
        m_gCapture.transform.localEulerAngles = vv;


    }

    void RotateFuc(Vector3 vDir)
    {
        RotateYaw(-vDir.x * m_fRotSpeed);
    }

    #endregion
    private void Update()
    {
        if (m_gAirDir.activeInHierarchy == false) return;
        if (!m_gAirObject) return;
        if(m_kAirCam!=null)
        {
            GameObject gg= m_gAirObject.GetHitObject();
            if (gg == null) return;

            m_kAirCam.transform.LookAt(gg.transform);
        }
        
    }
}
