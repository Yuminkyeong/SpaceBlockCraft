using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWUnityLib;
using CWEnum;
using CWStruct;
using UnityEngine.UI;
using DG.Tweening;

public class CWChHero : CWSingleton<CWChHero>
{

    public DETECTTYPE m_GroundState = DETECTTYPE.EXIT;

    public GameObject m_gShootDummy;
    public GameObject m_gParent;
    public GameObject m_gCameraDummy;
    public GameObject m_visible;
    public CharBody   m_gBody;
    public GameObject m_CameraBody;// 여기만 보이고 움직인다

    
    public Char_Digg m_gDigg;
    
    public bool m_bShow;

    public bool m_bLandingflag = false;// 낙하 하고 있다

    Rigidbody m_Rigidbody = null;
    public Rigidbody GetRigidbody()
    {
        if(m_Rigidbody==null)
        {
            m_Rigidbody = gameObject.GetComponent<Rigidbody>();
            if (m_Rigidbody == null)
            {
                m_Rigidbody = gameObject.AddComponent<Rigidbody>();

                m_Rigidbody.freezeRotation = true;
                m_Rigidbody.drag = 0.6f;
                m_Rigidbody.angularDrag = 0.8f;
                m_Rigidbody.mass = 1;


            }
                
        }
        return m_Rigidbody;
    }
    // 움직이고 있는가?
    public bool IsMove()
    {
        return GetComponent<CharicAction>().IsMove();
    }
    public  float GetYaw()
    {
        return Camera.main.transform.eulerAngles.y;
    }

    void DestroyRigi()
    {
        if(m_Rigidbody!=null)
        {
            Destroy(m_Rigidbody);
            m_Rigidbody = null;
        }
    }

    public void SettingChar(int num)
    {
        if(m_gBody!=null)
        {
            //Destroy(m_gBody);
            DestroyImmediate(m_gBody.gameObject);
        }
        m_gBody= CWResourceManager.Instance.GetCharBody(num);
        m_gBody.transform.parent = m_visible.transform;
        m_gBody.transform.localPosition = Vector3.zero;
        m_gBody.transform.localRotation = new Quaternion();
        m_gBody.transform.localScale = Vector3.one;
        m_gShootDummy = CWLib.FindChild(m_gBody.gameObject, "dummyshoot");

        m_gDigg.SelectBlock(0);
    }
  

    private void Start()
    {
        m_visible.SetActive(false);
        m_gDigg.gameObject.SetActive(false);


    }
    

    public void Close()
    {
        m_visible.SetActive(false);
        m_gDigg.gameObject.SetActive(false);
    }
    public void Show(bool bflag)
    {

        m_visible.SetActive(bflag);
        if(bflag)// 캐릭터 모드 
        {
            CWMapManager.SelectMap.SetMeshCollider(false);
            
            GetComponent<CharicAction>().m_bFlyMode = false;
            transform.DOKill(false);
            m_bShow = true;
            SetDance();
            ShowBody(true);
            Rigidbody rr = GetRigidbody();
            rr.isKinematic = false;
            rr.useGravity = true;
            transform.parent = CWHero.Instance.transform.parent;
            transform.localPosition = Vector3.zero;
            transform.localScale = Vector3.one;
            transform.localRotation = new Quaternion();
            GetComponent<CharicAction>().Play();
            Camera.main.fieldOfView = 43;

        }// 캐릭터 모드
        else
        {
            // 비행기 모드
            CWMapManager.SelectMap.SetMeshCollider(true);
        }
    }

   

    #region 위치 정보

    public Vector3 GetPosition()
    {
        return transform.position;
    }

    #endregion

    #region 캐릭터가 착석 
    // 현재 상태 
    //bflag=false 캐릭터가 내린 상황
    public void ShowBody(bool bflag)
    {

        if(bflag)
        {
            m_gDigg.gameObject.SetActive(false);
            m_gBody.gameObject.SetActive(true);
        }
        else
        {
            m_gBody.gameObject.SetActive(false);
            m_gDigg.gameObject.SetActive(true);

            m_gDigg.SetIdle();
        }
        //GameObject gg2 = CWLib.FindChild(m_gBody.gameObject, "mixamorig:Hips");
        //gg2.SetActive(bflag);
        //if (gg2 == null) return;
        //GameObject gg = CWLib.FindChild(m_gBody.gameObject, "right_arm_dummy");
        //if (gg == null) return;
        //if (!bflag)// 캐릭터 내림
        //{

        //   // gg.transform.localPosition = new Vector3(1.12f, 0, -0.9f);
        //    //gg.transform.localEulerAngles = new Vector3(58, 9, 52);
        //    gg.transform.parent = m_CameraBody.transform;
        //    gg.transform.localPosition = new Vector3(2.05f,1.46f,0.95f);
        //    gg.transform.localEulerAngles = new Vector3(56.26f,-11.17f,-9.32f);

        //}
        //else
        //{
        //      gg.transform.localPosition = Vector3.zero;
        //      gg.transform.localEulerAngles = Vector3.zero;
        //}




    }
    // 비행기에 타다 / 내리다
    // 캐릭터가 비행기에 타다
    public void Charic_IN()
    {


        Show(false);

        //CWBgmManager.Instance.PlayDigg();

    }
    // 캐릭터가 비행기에서 내리다 
    public void Charic_OUT(Vector3 vPos)
    {
        Show(true);
        if (vPos.x < 10) vPos.x = 10;
        if (vPos.x > 245) vPos.x = 245;

        if (vPos.z < 10) vPos.z = 10;
        if (vPos.z > 245) vPos.z = 245;


        SetDance();
        m_bLandingflag = true;
        Rigidbody rr = GetRigidbody();
        rr.isKinematic = false;

        Vector3 vvv = CWHero.Instance.GetChiricPos();
        vvv.x += 16;
        vvv.z += 16;

        Vector3 vv = CWHero.Instance.transform.position;// + vvv;

      //  vv.y += 2f;

        transform.DOMove(vv, 0.01f).OnComplete(() =>
        {
///            Vector3 vDir = CWHero.Instance.transform.forward;
   ///         vDir.y = 1;
      //      GetRigidbody().AddForce(vDir, ForceMode.Impulse);

        });


        

        transform.DORotateQuaternion(CWHero.Instance.transform.rotation, 1f);

        //CWProductionRoot Production = CWResourceManager.Instance.GetProduction("CharAction_OFF");
        //Production.Begin(() => {
            GetComponent<CharicAction>().Play();
            ShowBody(false);
            m_bLandingflag = false;
        //});


        StartCoroutine("IFitchRun");
        

    }
    IEnumerator IFitchRun()
    {
        while(true)
        {
            yield return null;
            CharicCamera cc = GamePlay.Instance.gameObject.GetComponentInChildren<CharicCamera>();//.g_CharicCamera.FitchPlay(3.2f);
            if(cc!=null)
            {
                cc.FitchPlay(1f);
                break;
            }
        }
    }


    #endregion

    private void OnMouseDown()
    {
       // SetDance(Random.Range(1,3));
    }
    public bool IsJuming()
    {
        return GetComponent<CharicAction>().IsJumping();
    }
    public void OnJump()
    {
        GetComponent<CharicAction>().OnJump();
        
    }

    public void SetDie()
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetDie();
    }
    public void SetDance()
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetDance(1);
    }
    public void SetIdle()
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetIdle();
    }
    public void SetAttack()
    {
        //m_gBody.SetAttack();
        m_gDigg.SetAttack();
    }
    public void SetWalk()
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetWalk();
    }
    public void SetDance(int num)
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetDance(num);
    }
    public void SetRandomDance()
    {
        if (m_gBody.gameObject.activeSelf == false) return;
        m_gBody.SetRandomDance();
    }



    #region 이펙트 

    public void LandingEffect()
    {
        CWPoolManager.Instance.GetParticle(transform.position, "Missffect", 1f);
    }

    #endregion

    #region 블록캐기
    // 레이저
    CWLaser m_kLaser = null;
    public void BlockBegin(Vector3 vTargetPos)
    {
        GameObject gg = CWPoolManager.Instance.GetLaser("DigLaser", 3f);
        m_kLaser = gg.GetComponent<CWLaser>();

        m_kLaser.BlockBegin(m_gShootDummy.transform, vTargetPos);

        BlockDamageUI.Instance.Begin();
    }

    public void BlockDigging(Vector3 vPos)
    {
        if (m_kLaser != null) 
            m_kLaser.Stop();

        m_kLaser = null;
        SetAttack();
        
        // 근처에 있는 블록만 캘 수 있다 
        //float fdist = Vector3.Distance(vPos, transform.position);
        //if(fdist<3f)
        {
            CWMapManager.SelectMap.Hit(true, vPos, 1);
        }
        //Debug.Log(string.Format("거리= {0}",fdist));

    }
    public void BlockDiggStop()
    {
        if (m_kLaser==null) return;
        m_kLaser.Stop();
        m_kLaser = null;
    }
    #endregion

    #region 블록꾸미기

    
    // 블록을 세팅한다
    public int m_nSelectBlock;
    public void SelectBlock(int nblock)
    {
        m_nSelectBlock = nblock;
        m_gDigg.SelectBlock(nblock);

    }
    // 삭제하거나, 칠해야 하는 블록인가?
    public bool IsUpdateBlock()
    {

        if(EquipInvenList.Instance.m_bDeletedFlag)
        {
            return true;
        }
        int nItem = CWArrayManager.Instance.GetItemFromBlock(m_nSelectBlock);
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
        if (gData.type == "color") return true;


        return false;
    }
    Vector3[] g_Varray =
    {
            new Vector3(1,1,0),
            new Vector3(1,0,0),
            new Vector3(0,1,1),
            new Vector3(0,0,1),
            new Vector3(1,1,1),
            new Vector3(1,0,1),
            new Vector3(0,1,0),
            new Vector3(0,0,0),

    };

    bool IsDetecting(int x,int y,int z)
    {
        Vector3 vPos= CWMapManager.SelectMap.GetCameraPos();
        if(x==vPos.x && y == vPos.y&& z == vPos.z)
        {
            return true;
        }
            
        //Collider[] cc = gameObject.GetComponentsInChildren<Collider>();
        //Vector3 vv = new Vector3(x, y, z);
        //foreach (var v in cc)
        //{
        //    //if (v.isTrigger) continue;
        //    for(int i=0;i<8;i++)
        //    {
        //        Vector3 val = vv + g_Varray[i];
        //        if (v.bounds.Contains(val))
        //        {
        //            return true;
        //        }

        //    }

        //}


        return false;
    }
    public void SetBlock(int x,int y,int z)
    {
        int nowblock = CWMapManager.SelectMap.GetBlock(x, y, z);
        int nItem = CWArrayManager.Instance.GetItemFromBlock(m_nSelectBlock);
        int nselect = m_nSelectBlock;
        if (EquipInvenList.Instance.m_bDeletedFlag)
        {

            
            if (nowblock>0)
            {
                // 컬러 블록이라면, 스톤과 컬러가 나눠서 가져간다
                CWMapManager.SelectMap.TakeBlock(x, y, z, nowblock);
            }
        }
        else
        {
            if (nItem == 0) return;
            GITEMDATA gData = CWArrayManager.Instance.GetItemData(nItem);
            if (gData.type == "color")
            {
                // 칠할 수 없는 블록이면 칠하면 안된다!!!
                if(CWMapManager.SelectMap.IsAllowColor(x,y,z))
                {
                    // 컬러를 칠한다
                    EquipInvenList.Instance.DelItem(nItem);
                    EquipInvenList.Instance.UpdateData();
                    CWMapManager.SelectMap.UpdateColor(x, y, z, nselect);
                }
                else
                {
                    NoticeMessage.Instance.Show("칠 할 수 없는 블록입니다!");
                }
            }
            else
            {
                // 블록이 캐릭터와 충돌하면 붙이면 안된다!!

                if(IsDetecting(x, y, z))// 충돌하는가?
                {
                    Debug.Log("캐릭터와 충돌!!");
                    return;
                }
                EquipInvenList.Instance.DelItem(nItem);
                EquipInvenList.Instance.UpdateData();
                CWMapManager.SelectMap.UpdateBlock(x, y, z, nselect);
            }
        }


        SetAttack();
    }



    #endregion
}
