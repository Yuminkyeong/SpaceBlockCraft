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
using DG.Tweening;

/*
 * 형체 
 * 움직임등을 가지는 객체 
 *  히로,유저,터렛,빌딩,미사일이 모두 포함된다  
 *  디텍팅 ,블록 
 *  
  * */
public class CWObject : CWBehaviour
{
    #region 전역 변수 

    public static CWObject g_kSelectObject;// 선택된 오브젝트 

    #endregion

    #region  오브젝트 

    public CWOBJECTTYPE m_ObjectType;


    public int SELLWIDTH = 64;
    public GameObject m_gCenterObject;
    public GameObject m_gBody;
    public GameObject m_gItemBody;
    public GameObject m_gMeshDir;// 일반 메시 

    protected GameObject m_gTempdir;

    

    protected virtual void BodyCreate()
    {
        if(m_gCenterObject==null)
        {
            m_gCenterObject = new GameObject();
            m_gCenterObject.name = "Center";
            m_gCenterObject.transform.parent = transform;
            m_gCenterObject.transform.localPosition = Vector3.zero;
            m_gCenterObject.transform.localScale = Vector3.one;
            m_gCenterObject.transform.localEulerAngles = Vector3.zero;

        }
        if (m_gBody == null)
        {
            m_gBody = new GameObject(); // gameObject;
            m_gBody.AddComponent<MeshFilter>();
            MeshRenderer rr = m_gBody.AddComponent<MeshRenderer>();
            rr.material = CWResourceManager.Instance.GetMaterial("Airterrain");

        }

        m_gBody.name = "Body";
        m_gBody.tag = gameObject.tag;
        m_gBody.layer = gameObject.layer;
        m_gBody.transform.parent = m_gCenterObject.transform;
        m_gBody.transform.localPosition = new Vector3(-SELLWIDTH / 2, 0, -SELLWIDTH / 2);
        m_gBody.transform.localRotation = new Quaternion();
        m_gBody.transform.localScale = Vector3.one;



        if (m_gItemBody!=null)
        {
            GameObject.Destroy(m_gItemBody);
            m_gItemBody = null;
        }
        if (m_gTempdir != null)
        {
            GameObject.Destroy(m_gTempdir);
            m_gTempdir = null;
        }
        if (m_gMeshDir != null)
        {
            GameObject.Destroy(m_gMeshDir);
            m_gMeshDir = null;
        }


        if (m_gItemBody == null)
        {
            m_gItemBody = new GameObject();
            m_gItemBody.name = "ItemBody";
            m_gItemBody.tag = gameObject.tag;
            m_gItemBody.layer = gameObject.layer;
            m_gItemBody.transform.parent = m_gCenterObject.transform;
            m_gItemBody.transform.localPosition = new Vector3(0.5f, 0.5f, 0.5f);
            m_gItemBody.transform.localScale = Vector3.one;
            m_gItemBody.transform.localEulerAngles = Vector3.zero;
        }
        if (m_gTempdir == null)
        {
            m_gTempdir = new GameObject();
            m_gTempdir.transform.parent = m_gCenterObject.transform;
            m_gTempdir.name = "Tempdir";

            m_gTempdir.transform.localPosition = Vector3.zero;
            m_gTempdir.transform.localScale = Vector3.one;
            m_gTempdir.transform.localEulerAngles = Vector3.zero;

        }
        if (m_gMeshDir == null)
        {
            m_gMeshDir = new GameObject();
            m_gMeshDir.transform.parent = m_gCenterObject.transform;
            m_gMeshDir.name = "MeshDir";

            m_gMeshDir.transform.localPosition = Vector3.zero;
            m_gMeshDir.transform.localScale = Vector3.one;
            m_gMeshDir.transform.localEulerAngles = Vector3.zero;

        }

    }
    protected virtual void FixCenter()
    {
        m_gCenterObject.transform.localPosition = Vector3.zero;  
    }
    // 블록의 절대값을 구한다
    public Vector3 GetBlockPosition(int x,int y,int z)
    {
        if (m_gBody == null) return Vector3.zero;
        Vector3 vPos = new Vector3(x, y, z);
        vPos += m_gBody.transform.position;
        return vPos;
        //m_gBody.transform.position;
    }

    #endregion

    // 
    public int m_nSelectWeaponType = 0;// 무기를 선택한다 
    public GameLayer m_nLayer = GameLayer.Detect;
    public AIOBJECTTYPE m_AIType;
    public string m_szFace;

    int _grade = 0;
    public virtual int m_nGrade
    {
        get
        {
            return _grade;
        }
        set
        {
            _grade = value;
        }
    }
    public int m_nPrice; // 비행기 가격

    public GameObject m_gInfoView;
    bool m_bLoad = false;

    public virtual int GetGrade()
    {
        return m_nGrade;
    }


    public bool IsLoad()
    {
        return m_bLoad;
    }
    public virtual bool IsDrone() //비행기 인가?
    {
        return false;
    }
    public virtual string GetName()
    {
        return name;
    }

    public void SetColor(Color cc)
    {
        CWLib.SetColorChild(transform, cc);
    }

    public virtual void TakeMapBlock(int x, int y, int z) { }



    

    public void AIStop()
    {
        CWAIEntity ai = gameObject.GetComponent<CWAIEntity>();
        if (ai != null)
        {
            ai.Stop();
        }

    }


    #region 상위객체 오버로드

    protected override bool OnceRun()
    {


        return base.OnceRun();
    }

    #endregion
    #region 사망관련 
    protected bool m_bDieFlag = false;// 사망 
    public bool IsDie()
    {
        return m_bDieFlag;
    }
    // 사망! 
    public virtual void MultiMapDie()
    {

    }
    public virtual void SetDie()
    {
        m_bDieFlag = true;
        CWPower cp = gameObject.GetComponent<CWPower>();
        cp.FEnRate = 0;
        cp.FhpRate = 0;
        
        if (CWAIOwnerManager.Instance != null && CWAIOwnerManager.Instance.IsMyOwner(m_nID))
        {
            CWAIOwnerManager.Instance.SendRemoveOwner(m_nID);
        }
        Destroy(gameObject);
    }


    #endregion


    #region UDP 무브 체크
    Vector3 m_vPrePos = Vector3.zero;
    float m_fPrevYaw;
    public bool IsMoved()
    {
        if (m_vPrePos == GetPosition() && m_fPrevYaw == GetYaw())
        {
            return false;
        }
        m_vPrePos = GetPosition();
        m_fPrevYaw = GetYaw();

        return true;
    }


    #endregion

    
    public virtual int GetRoomNumber()// 
    {
        return 0;
    }


    USERTYPE _UserType;

    protected CWJSon m_kJSon = new CWJSon();
    private CWPower _kPower;

    private int _nLevel = 1;
    public int m_nRankPoint;
    public int m_nID;

    public int m_nRest;//0 휴식아님 1 휴식 중 

    public CWObject m_gKiller;

    public bool m_bTeamSetting = false;


    public CWPower KPower
    {
        get
        {
            if (_kPower == null)
            {
                _kPower = gameObject.GetComponent<CWPower>();
            }
            return _kPower;
        }

        set
        {
            _kPower = value;
        }
    }

    public virtual int NLevel
    {
        get
        {
            return _nLevel;
        }
        set
        {
            _nLevel = value;
        }
    }

    public USERTYPE UserType
    {
        get
        {
            return _UserType;
        }

        set
        {
            _UserType = value;
        }
    }

    public int NTeam;
    public Vector3 VStartPos
    {
        get
        {
            if (vStartPos == Vector3.zero)
            {
                vStartPos = transform.position;
            }
            return vStartPos;
        }

    }


    private int nTeam = -1;

    public bool IsUser()
    {
        if (UserType == USERTYPE.USER) return true;
        if (UserType == USERTYPE.BATTLEAI) return true;
        if (UserType == USERTYPE.COLLECTUSER) return true;
        return false;
    }


    public Vector3 GetBodyPos()
    {
        return m_gBody.transform.position;
    }

    public virtual Vector3 GetHitPos()
    {
        return GetPosition();// 
    }

    private Vector3 vStartPos = Vector3.zero;




    public virtual void SetPos(Vector3 vPos)
    {
        if (GetHP() <= 0) return;
        transform.position = vPos;


    }
    public void SetRotate(Vector3 vRot)
    {
        transform.eulerAngles = vRot;
    }
    public float GetPosX()
    {
        return transform.position.x;

        
    }
    public float GetPosZ()
    {
        return transform.position.z;

        
    }
    public float GetPosY()
    {
        return transform.position.y;

        
    }
    public Vector3 GetPosition()
    {
        return transform.position;
    }
    public virtual void SetYaw(float fYaw)// 방향 설정 
    {
        Vector3 vRot = Vector3.zero;
        vRot.y = fYaw;
        transform.eulerAngles = vRot;
    }
    public virtual float GetYaw()
    {
        return transform.eulerAngles.y;
    }
    public virtual void CopyObject(CWObject kObject)
    {
        

    }
    public virtual void SetBuffer(byte[] buffer)
    {

        m_kJSon.SetData(buffer);

    }
    public void SetHPRate(float fRate)
    {
        if (KPower == null) return;
        KPower.FhpRate = fRate;
    }
    public float GetHpRate()
    {
        if (KPower == null) return 0;
        return KPower.FhpRate;

    }
    public float GetEnRate()
    {
        if (KPower == null) return 0;
        return KPower.FEnRate;

    }
    public  int GetMaxHP()
    {
        if (KPower == null) return 0;
        return KPower.m_nHp;
    }

    public  float GetHP()
    {
        if (KPower == null) return 0;
        return KPower.GetHP();
    }
    public  float GetSpeed()
    {
        if (KPower == null) return 0;
        return KPower.FSpeed;
    }
    public  int GetDamage()
    {
        if (KPower == null) return 0;
        return KPower.m_nDamage;
    }
    //- 개념 정의
    // 객체의 이름을 파일이름으로 한다. 
    // 목적 : 게임상 아무의미가 없다.
    // 작업적 편의 목적외는 아무런 의미가 없다
    // 작업적 편의 개념 
    // 바로 로딩가능, 툴에서 어떤파일인지 확인 가능!
    // 이름에 의미를 부여하는 순간 버그 가능성있음 잊으면 안됨 
    public virtual void Load(string szName)
    {
        name = szName;
        if(!CWLib.IsString(name))
        {
            name = "empty";
        }
        //DeleteFile();
        if (LoadFileFunc())
        {
            if (LoadMeshFunc())
            {
                LoadObjectFunc();

                
                SetTag(gameObject.tag);

            }
        }
        m_bLoad = true;
    }

    IEnumerator Loadfuc()
    {
        m_bLoad = false;
        yield return null;
        if (LoadFileFunc())
        {
            yield return null;
            if (LoadMeshFunc())
            {
                 yield return null;
                LoadObjectFunc();
                SetTag(gameObject.tag);
            }
        }
        m_bLoad = true;
        yield return null;
    }

    private void OnDestroy()
    {
        DeleteFile();
    }
    public void MsgText(string szvalue)
    {
        BaseUnitUI cp = gameObject.GetComponentInChildren<BaseUnitUI>();
        if (cp != null)
        {
            cp.Message(szvalue,5f);
            

        }

    }
    protected virtual void HpEvent()
    {
        if (KPower == null) return;
        BaseUnitUI cp = gameObject.GetComponentInChildren<BaseUnitUI>();
        if (cp != null)
        {
            cp.UpdateData();
        }

      //  ConvertHPBlock();

    }

    protected virtual void CreatePower()
    {
        KPower = gameObject.GetComponent<CWPower>();
        if (KPower == null)
        {
            KPower = gameObject.AddComponent<CWPower>();
        }


        KPower.hpEvent = HpEvent;

    }
    protected virtual void SetObjectType()
    {

    }
    public virtual void Create(int nID = 0)
    {

        if(gameObject.activeInHierarchy==false)
        {
            Debug.LogError("여긴 어디?");
            return;
        }
        SELLWIDTH = 32;
        SetObjectType();
        m_nID = nID;
        CreatePower();
        m_bLoad = false;
        StartCoroutine("Loadfuc");
        SetTeam();




    }
    protected virtual void DeleteFile()
    {
       
    }
    protected virtual bool LoadMeshFunc()
    {
        return true;
    }
    protected virtual bool LoadObjectFunc()
    {
        return true;
    }
    public virtual bool LoadJSon()
    {
        return true;
    }
    public virtual bool LoadOldFile(MemoryStream ms)
    {
        return true;
    }
    protected virtual string GetPath()
    {
        return "Gamedata/" + name;
    }
    protected virtual bool LoadFileFunc()
    {
        if (!m_kJSon.IsLoad())
        {
            if (m_kJSon.LoadGamedata(GetPath()) == null)
            {
                return false;// 로드 실패!
            }
        }
        return LoadJSon();

    }
    bool m_bCheckflag = false;
    public virtual void CopyTransPos(Vector3 vPos, float fYaw)
    {
        if (gameObject.activeSelf == false) return;

        float fdist = Vector3.Distance(transform.position, vPos);
        if (m_bCheckflag && fdist > 80f)
        {
            ////Debug.Log("데이타 튐!! " + fdist.ToString()+"   " + vPos.ToString());
            //string sz = string.Format("{0} - [{1} >< {2}]", fdist.ToString(),vPos, transform.position);
            //Debug.Log(sz);
            //return;
        }
        transform.position = vPos;
        SetYaw(fYaw);
        m_bCheckflag = true;
    }
    public virtual void BombShoot(Vector3 vTarget, int nBlockCount, string szBomb = "")
    {

    }
    public virtual void AIShoot(bool bDetected, CWObject gTarget)
    {

    }
    public virtual void AIShootPos(bool bDetected, Vector3 vPos)
    {

    }

    public virtual void SetDamage(int nHitter, int nDamage, float fhp)
    {

        m_gKiller = CWUserManager.Instance.GetUser(nHitter);

        CWEffectManager.Instance.GetEffect(GetHitPos(), "Explosion2");
        if (KPower)
        {
            KPower.FhpRate = fhp;
        }

        



    }
    public virtual void ShootPos(bool bDetected, Vector3 vTarget) { }

    protected virtual void ConvertHPBlock()
    {

    }

    public virtual  void SetTag(string sztag)
    {
        CWLib.SetGameObjectTag(gameObject, sztag);
        

    }
    public virtual void SetDirection(Vector3 vTarget)
    {
        float fYaw = CWMath.GetLookYaw(transform.position, vTarget);
        SetYaw(fYaw);

    }
    public virtual void SetEnemyBar(bool bEnemy) { }
    public void SetTeamColorBar(bool myteam)
    {
      


    }
    public virtual void SetTeam()
    {
        NTeam = m_nID;
    }

    public virtual void SetRest(int nmode)
    {
        m_nRest = nmode;
    }

    public virtual void RotateWeaponIdle()
    {

    }
    public virtual void Rotate(Vector3 vRot)
    {
        transform.eulerAngles = vRot;
    }

    public virtual void TurntoEnmey(Vector3 vTarget, EventDelegate.Callback fuc)
    {
        vTarget.y = transform.position.y;
        transform.DOLookAt(vTarget, 2f).OnComplete(() => {
            fuc?.Invoke();
        });

    }
    IEnumerator TurnFucntion(Vector3 vTarget, EventDelegate.Callback fuc)
    {

        Vector3 vRot = transform.eulerAngles;
        Vector3 _Way = vTarget - transform.position;
        float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;

        // 회전각도 증가는 어느방향이 가까운가?
        float fdist = CWMath.GetAngleDist(vRot.y, _Angle);
        float fdir = CWMath.GetAngleDir(vRot.y, _Angle);



        float fRet = 0;
        // 0,1,2,3,4,
        while (true)
        {
            fRet += (Time.deltaTime * 200);
            vRot.y += (Time.deltaTime * 200) * fdir;
            if (Mathf.Abs(fdist) < fRet)
            {
                fuc?.Invoke();
                break;
            }
            //            transform.eulerAngles = vRot;
            Rotate(vRot);
            yield return null;
        }
    }

    public virtual bool IsEnemy(CWObject kObject)
    {
        if (NTeam <= 0) return false;
        

        if (kObject.NTeam <= 0) return false;// 적아님

        if (kObject.NTeam != NTeam) return true; //  적
        return false;
    }

    public virtual  bool IsHeroTeam()
    {
      

        return false;
    }
    public virtual void MakeBuild()
    {

    }
    public virtual void OnClick()
    {

    }
    public void DontInfoView()
    {

        m_gInfoView.SetActive(false);
    }
    // 보석 캐시로 된 가격
    public int GetPrice()
    {
        return (int)((float)m_nPrice * 0.0025f);
        //return m_nPrice;// 비행기 가격
    }
    public float GetPricef()
    {
        return ((float)m_nPrice * 0.0025f);
    }

    public virtual void PVPStart()
    {

    }

    #region 스트링 정보

    public virtual string GetValue(string str)
    {
        if(str=="HP")
        {
            return KPower.GetHP().ToString();
        }
        if (str == "Energy")
        {
            return KPower.GetEnergy().ToString();
        }
        if (str == "Damage")
        {
            return KPower.m_nDamage.ToString();
        }

        return "";

    }
    public virtual float GetRate(string str)
    {
        if (str == "HP")
        {
            return KPower.FhpRate;
        }
        if (str == "Energy")
        {
            return KPower.FEnRate;
        }
        return 0;

    }
    public virtual string GetRateStr(string str)
    {
        if (str == "HP")
            return string.Format("{0}/{1}",KPower.GetHP(),KPower.m_nHp);
        if (str == "Energy")
            return string.Format("{0}/{1}",KPower.GetEnergy(),KPower.m_fEnergy);

        return "";
    }



    #endregion


    public virtual void SetEmoticon(Emoticon eType)
    {

    }

    public virtual void SetMusukBar(int state)
    {

    }

    #region 회전 움직임
    
    // 회전을 한다 
    public virtual void DoRotate(Vector3 vRot,float fTime)
    {
        transform.DORotate(vRot, fTime);
    }
    public virtual void DoLookAt(Vector3 vRot, float fTime)
    {
        transform.DOLookAt(vRot, fTime);
    }


    #endregion

    // 추가 데미지 배수 
    public virtual int BonusDamage()
    {
        return 1;
    }
}



