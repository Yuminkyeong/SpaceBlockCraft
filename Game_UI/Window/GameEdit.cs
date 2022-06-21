using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json.Linq;
using CWUnityLib;
using CWStruct;
using CWEnum;



public class GameEdit : WindowUI<GameEdit>
{

    public int m_nAirSlotID;
    public int m_nAirBlockCount;
    public Material m_kSkyMat;
    public GameObject m_gEnvironment;


    public bool m_bErase = false;



    protected override int GetUINumber()
    {
        return 3;
    }

    

    #region Undo

    public List<Dictionary<int, BlockData>> m_kUndo = new List<Dictionary<int, BlockData>>();

    void AddUndo()
    {
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();

        Dictionary<int, BlockData> nData = new Dictionary<int, BlockData>();

        foreach (var v in kData)
        {
            nData.Add(v.Key, v.Value);
        }
        m_kUndo.Add(nData);
        EditInvenDlg.Instance.AddUndo();


    }
    void Undo()
    {
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();

        int tcnt = m_kUndo.Count - 1;
        if (tcnt < 0) return;

        Dictionary<int, BlockData> nData = m_kUndo[tcnt];

        kData.Clear();
        foreach (var v in nData)
        {
            kData.Add(v.Key, v.Value);
        }
        m_kAirObject.m_bUpdated = true;

        EditInvenDlg.Instance.Undo();
        m_kUndo.Remove(nData);
        m_kAirObject.CalPower();
        m_kAirObject.CallSize();



    }


    #endregion

    Vector3 m_vCenterPos=Vector3.zero;

    UNITCLASS m_kUnitClass;

    public bool m_bUnit = false;
    int m_nSlot;// heor는 슬롯번호, 유닛은 사용안함

    bool _updated=false;
    public bool m_bUpdated
    {
        get
        {
            return _updated;
        }
        set
        {
            _updated = value;
            //UpdateCenterCount();
        }
    }


    Vector3 m_vPos;
    Vector3 m_vEuler = Vector3.zero;
    Vector3 m_vScale = new Vector3(0.5f, 0.5f, 0.5f);
    Vector3 m_vMovePos = Vector3.zero;
    public GameObject m_gEditPan;

    public GameObject m_gCamera;
    public GameObject m_gMainCamera;

    public CWAirObject m_kAirObject;
    public CWAirObject m_kSelectBlock;
    public GameObject m_gPos;

    public CWButton m_kShapeBtn;
    public Transform m_tHPText;

    BlockData m_kSelectBlockData = new BlockData();// 현재 선택된 블록
    
    private int _nSelectID;
    public int m_nSelectSlot;

    public Text m_tModeTextUI;
    public GameObject m_gColorUI;
    public GameObject m_gColor;
    public GameObject m_gEraser;

    public GameObject m_gEditCamPos;
    public Image m_kColor;

    
    int _nColor;
    

    bool _bDelete = false;
    void UpdateSelect()
    {
        
      

        m_kSelectBlock.gameObject.SetActive(true);
        //m_kSelectBlock.transform.DOScale(2, 0.5f);

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(_nSelectID);
        WEAPON ws = CWArrayManager.Instance.GetWeapon(_nSelectID);


        BLOCKSHAPE _shape = BLOCKSHAPE.NORMAL;
        if (SELECT_SHAPE==SHAPE.NORMAL)
        {
            _shape= BLOCKSHAPE.NORMAL;
        }
        if (SELECT_SHAPE == SHAPE.SLANT)
        {
            _shape = BLOCKSHAPE.SLANT_1;
        }
        if (SELECT_SHAPE == SHAPE.HALF)
        {
            _shape = BLOCKSHAPE.HALF;
        }
        if (SELECT_SHAPE == SHAPE.QURT)
        {
            _shape = BLOCKSHAPE.QURT;
        }
        if (SELECT_SHAPE == SHAPE.QURT3)
        {
            _shape = BLOCKSHAPE.QURT3;
        }
        if (SELECT_SHAPE == SHAPE.HSLANT)
        {
            _shape = BLOCKSHAPE.HSLANT_1;
        }
        if (SELECT_SHAPE == SHAPE.STAIR)
        {
            _shape = BLOCKSHAPE.STAIRS_1;
        }
        if (SELECT_SHAPE == SHAPE.STICK)
        {
            _shape = BLOCKSHAPE.STICK;
        }

        
        //블록 모양을 만들어야 한다 
        m_kSelectBlock.ClearBlock();
        m_kSelectBlock.SetBlock(16, 5, 16, _nSelectID, (int)_shape, 0);
        m_kSelectBlock.UpdateBlock();


    }
    public int NSelectID
    {
        get
        {
            return _nSelectID;
        }

        set
        {
            if(_nSelectID != value)
                SELECT_SHAPE = SHAPE.NORMAL;
            _nColor = 0;
            _nSelectID = value;


            if (_nSelectID > 0)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(_nSelectID);
                if (gData.type == "color")
                {
                    m_State = STATE.COLOR;
                    _nColor = (int)CWGlobal.GetColorItem((GITEM)NSelectID);

                    if (_nColor == (int)COLORNUMBER.WHITE)
                    {
                        if(m_gEraser)
                            m_gEraser.SetActive(true);
                        if(m_gColor)
                        m_gColor.SetActive(false);
                        
                    }
                    else
                    {
                        Color kcolor = CWGlobal.GetColor((COLORNUMBER)_nColor);
                        kcolor.a = 1;
                        if(m_kColor)
                            m_kColor.color = kcolor;
                        if (m_gEraser)
                            m_gEraser.SetActive(false);
                        if (m_gColor)
                            m_gColor.SetActive(true);


                    }


                }
                else
                {
                    m_State = STATE.BLOCK;
                    _nColor = 0;
                }
            }
            else
            {

                m_State = STATE.BLOCK;
            }
         
        }
    }

    public GameObject m_gDelete;
    public CWButton m_kDeleteBtn;
    public bool BDelete
    {
        get
        {
            return _bDelete;
        }

        set
        {

            SELECT_SHAPE = SHAPE.NORMAL;
            m_State = STATE.DELETE;
            _bDelete = value;


        }
    }

    protected override void OnceFunction()
    {
        m_kUIType = UITYPE.EDIT;
        base.OnceFunction();
    }


    public GameObject[] m_gHideObject;

    void __BeginGamePlay2()
    {
        CWGalaxy.Instance.OnPlayMap();
        
        Close();
        GamePlay.Instance.ShowWar();
        


        
    }
    void __BeginGamePlay()
    {
        
    }
    
    // 교환이 되면 무조건 나간다
    public void BeginerShow()
    {
        
        ChangeBlockDlg.Instance.CloseFuction = __BeginGamePlay;


        Open();
    }

    void __Help()
    {
        //TalkMessageDlg.Instance.Show("블록을 붙이면 HP가 올라갑니다!");
        TipMessageDlg.Instance.Show("블록을 붙이면 HP가 올라갑니다!");
    }

    public void Show(int nID,int blockcount, byte[] bBuffer)
    {
        //int nCharItem = (int)GITEM.charblock_1 + CWHeroManager.Instance.m_nCharNumber-1;


        Open();
        m_nAirBlockCount = blockcount;
        m_nAirSlotID = nID;
        m_kAirObject.CopyBuffer(bBuffer);
        CWObject.g_kSelectObject = m_kAirObject;
        Setting();
        CWLib.SetGameObjectLayer(m_kAirObject.m_gCenterObject, LayerMask.NameToLayer("Bound"));
        m_kAirObject.gameObject.layer = LayerMask.NameToLayer("Bound");

        m_kAirObject.m_gCenterObject.transform.localPosition = Vector3.zero;
        m_kAirObject.m_bMeshCollider = true;

        CoininfoDlg.Instance.Close();

    }

    public override void Open()
    {

        m_kUndo.Clear();
        foreach (var v in m_gHideObject)
        {
            if(v)
                v.SetActive(false);
        }
        base.Open();

        m_gCapture.gameObject.SetActive(true);

        

    }

    public override void Close()
    {

        foreach (var v in m_gHideObject)
        {
            v.SetActive(true);
        }

        m_kAirObject.transform.parent = Game_App.Instance.m_gHeroDir.transform;
        m_kAirObject.transform.localPosition = new Vector3();
        m_kAirObject.transform.rotation = new Quaternion();

        base.Close();

        

        m_gCapture.gameObject.SetActive(false);

        CoininfoDlg.Instance.Open();
    }
    void Setting()
    {


        CWBgmManager.Instance.PlayEdit();
        m_vPos = m_kAirObject.transform.position;
        m_kAirObject.transform.position = m_gPos.transform.position;

        m_kAirObject.transform.eulerAngles = Vector3.zero;
        m_kAirObject.transform.localScale = Vector3.one;

        m_vCenterPos = m_kAirObject.m_gCenterObject.transform.localPosition;
        RenderSettings.fog = false;
        

        m_gMainCamera.SetActive(false);
//        Camera.main.transform.position = m_gCamera.transform.position;
//      Camera.main.transform.rotation = m_gCamera.transform.rotation;

        //UpdateCenterCount();

        // 초기 블록 선택
    }

    protected override void _Open()
    {

        m_kAirObject.transform.parent = Game_App.Instance.transform;
        m_kAirObject.transform.localPosition = new Vector3();
        m_kAirObject.transform.rotation = new Quaternion();


        m_kAirObject.m_bDontProgresbar = true;
        CWObject.g_kSelectObject = m_kAirObject;
        EditInvenDlg.Instance.Open();
        //EditWeaponList.Instance.Open();
        m_gEditPan.SetActive(true);
        m_kAirObject.gameObject.SetActive(true);
        StartCoroutine("IRun");


        base._Open();
        if (m_gEnvironment)
        {
            Transform tParent = m_gEnvironment.transform.parent;
            for (int i = 0; i < tParent.childCount; i++)
            {
                Transform tChild = tParent.GetChild(i);
                tChild.gameObject.SetActive(false);
            }

            m_gEnvironment.SetActive(true);
        }

        EnvironmaenON();
        //SetShow(true);


    }
    public virtual void EnvironmaenON()
    {
        RenderSettings.skybox = m_kSkyMat;
        if (m_gEnvironment)
            m_gEnvironment.SetActive(true);

        //m_kSkyMat.color = Color.black;
        //StartCoroutine("IEnvRun");
    }

    protected override void _Close()
    {

        //CWHero.Instance.Show(false);
        m_gEditPan.SetActive(false);
        EditInvenDlg.Instance.Close();
        
     
        CWHeroManager.Instance.UpdatePrice();
        m_gMainCamera.SetActive(true);


        m_kAirObject.m_gCenterObject.transform.localPosition = m_vCenterPos;

    }
    #region 외부 호출

    public void ChangeItem(int nItem1, int nItem2)
    {
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();
        foreach (var v in kData)
        {
            if (v.Value.nBlock == nItem1)
            {
                m_kAirObject.UpdateBlock(v.Key, nItem2);
                return;
            }
        }

    }

    #endregion


    #region AIR_EDIT // 비행기 에디터 


    IEnumerator IRun()
    {
        while (true)
        {
            yield return null;
            if (m_kSelectBlock.gameObject.activeInHierarchy)
            {
                break;
            }

        }
        m_kSelectBlock.EmptyCreate();
        m_kSelectBlock.m_nLayer= GameLayer.UI;

        yield return new WaitForSeconds(1);
        EditInvenDlg.Instance.OnSelect(0);

        while (true)
        {
            RunUpdate();
            m_gCapture.transform.position = m_gCamera.transform.position;
            m_gCapture.transform.rotation = m_gCamera.transform.rotation;

            yield return null;
        }
    }

    private void RunUpdate()
    {
      

        m_kAirObject.UpdateBlock();

    }

    void _AddBlock(int x, int y, int z, int nFace)
    {
        if (NSelectID == 0) return;

        
        GITEMDATA gData = CWArrayManager.Instance.GetItemData(NSelectID);
        int nItem = NSelectID;

        

        AddUndo();
        int nShape = (int)CalShape(nFace);
        m_kAirObject.AddBlock(x, y, z, NSelectID, nShape, 0);
        if(EditInvenDlg.Instance.DelItem(NSelectID)==false)
        {
            // 다음 블록이 없다면
            NSelectID = EditInvenDlg.Instance.NextBlock(NSelectID);
        }

//        m_tHPText.DOShakePosition(0, 0);
  ///      m_tHPText.DOShakePosition(1f,7);

        CWDemageManager.Instance.ShowHpText("+"+gData.hp.ToString(),Color.yellow);

        m_bUpdated = true;

        CWQuestManager.Instance.CheckUpdateData(2, 1);//


    }
    bool IsBlockAdd(int x,int y,int z)
    {


        if (x < 0)
        {
            return false;
        }
        if (x >= m_kAirObject.SELLWIDTH)
        {
            return false;
        }
        if (y < 0)
        {
            return false;
        }
        if (y >= m_kAirObject.SELLWIDTH)
        {
            return false;
        }
        if (z < 0)
        {
            return false;
        }
        if (z >= m_kAirObject.SELLWIDTH)
        {
            return false;
        }
        if(m_kAirObject.GetBlock(x,y,z)>0)
        {
            return false;
        }

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(NSelectID);
        if(gData.type=="weapon")
        {
            if (!CheckWeapon(x, y, z))
            {
                return false;
            }
        }
        if (gData.type == "Buster")
        {
            if (!CheckBuster(x, y, z))
            {
                return false;
            }
        }




        return true;
    }

    bool _moveflag = false;
    public void AddBlock(int x, int y, int z,int nFace)
    {
        if (NSelectID == 0) return;
        if (_moveflag) return;

        m_vMovePos.y -= 5f;

        m_vMovePos.x -= 0.5f;
        m_vMovePos.y += 0.5f;
        m_vMovePos.z -= 0.5f;

        if(!IsBlockAdd(x,y,z))
        {
            NoticeMessage.Instance.Show("배치 할 수 없는 위치입니다");
            return;
        }

        Vector3 vPos = m_kSelectBlock.m_gCenterObject.transform.position;

        _moveflag = true;
        m_kSelectBlock.m_gCenterObject.transform.position= m_gEditCamPos.transform.position;
        m_kSelectBlock.m_gCenterObject.transform.DOMove(m_vMovePos, 0.3f).OnComplete(() => {
            _AddBlock(x, y, z, nFace);
            m_kSelectBlock.m_gCenterObject.transform.position = vPos;
            m_kSelectBlock.m_gCenterObject.transform.localPosition = Vector3.zero;
            _moveflag = false;
        });


    }
    void Delete(int x, int y, int z)
    {

        
        
        if (m_kAirObject.GetBlockCount() == 1)
        {
            MessageOneBoxDlg.Instance.Show("메세지", "삭제가 불가합니다!");
            return;
        }
        int nID = m_kAirObject.GetBlock(x, y, z);

        if(nID==(int)GITEM.CoreEngine)
        {
            MessageOneBoxDlg.Instance.Show("메세지", "삭제가 불가합니다!");
            return;
        }

        GITEMDATA gData = CWArrayManager.Instance.GetItemData(nID);

        // 무기 블록인가?
        if (gData.type == "shipblock")
        {
            if (!m_kAirObject.DeleteAllowBlock())
            {
                MessageOneBoxDlg.Instance.Show("메세지", "삭제가 불가합니다!");
                return;
            }

        }

        AddUndo();

        if (!m_kAirObject.DelBlock_AirEdit(x, y, z))
        {
            NoticeMessage.Instance.Show("블록의 연결이 끊어집니다!");
           return;
        }

        EditInvenDlg.Instance.AddItem(nID);

        CWDemageManager.Instance.ShowHpText("-" + gData.hp.ToString(), Color.red);

        m_bUpdated = true;

        SelectDelete();


        

    }
    void SelectDelete()
    {


        BDelete = true;

    }
    
    int GetNowShape()
    {
        BLOCKSHAPE _shape = BLOCKSHAPE.NORMAL;
        if (SELECT_SHAPE == SHAPE.NORMAL)
        {
            _shape = BLOCKSHAPE.NORMAL;
        }
        if (SELECT_SHAPE == SHAPE.SLANT)
        {
            _shape = BLOCKSHAPE.SLANT_1;
        }
        if (SELECT_SHAPE == SHAPE.HALF)
        {
            _shape = BLOCKSHAPE.HALF;
        }
        if (SELECT_SHAPE == SHAPE.QURT)
        {
            _shape = BLOCKSHAPE.QURT;
        }
        if (SELECT_SHAPE == SHAPE.QURT3)
        {
            _shape = BLOCKSHAPE.QURT3;
        }
        if (SELECT_SHAPE == SHAPE.HSLANT)
        {
            _shape = BLOCKSHAPE.HSLANT_1;
        }
        if (SELECT_SHAPE == SHAPE.STAIR)
        {
            _shape = BLOCKSHAPE.STAIRS_1;
        }
        if (SELECT_SHAPE == SHAPE.STICK)
        {
            _shape = BLOCKSHAPE.STICK;
        }

        return (int)_shape;
    }

    int CovnertNumber(float num)
    {
        return (int)(num + 32.0f)-32;
    }
    public Vector3 SelectPos(Vector3 vPos, Vector3 vNormal)
    {


        if(vNormal.y==1)
        {
            vPos.y = CovnertNumber(vPos.y + 0.95f);
        }
        if (vNormal.y == -1)
        {
            vPos.y = CovnertNumber(vPos.y);
        }

        if (vNormal.x == 1)
        {
            vPos.x = CovnertNumber(vPos.x + 0.95f);
        }
        if (vNormal.x == -1)
        {
            vPos.x = CovnertNumber(vPos.x);
        }

        if (vNormal.z == 1)
        {
            vPos.z = CovnertNumber(vPos.z + 0.95f);
        }
        if (vNormal.z == -1)
        {
            vPos.z = CovnertNumber(vPos.z);
        }



        if (vNormal.z>0&&vNormal.z<1)
        {
            vPos.z = (int)CovnertNumber(vPos.z + 0.95f);
            vNormal.z = 1;
        }
        if (vNormal.x > 0 && vNormal.x < 1)
        {
            vPos.x = CovnertNumber(vPos.x + 0.95f);
            vNormal.x = 1;
        }

        if (vNormal.z > -1 && vNormal.z < 0)
        {
            vPos.z = CovnertNumber(vPos.z );
            vNormal.z = 0;
        }
        if (vNormal.x > -1 && vNormal.x < 0)
        {
            vPos.x = CovnertNumber(vPos.x);
            vNormal.x = 0;
        }


        if (vNormal.y > 0 && vNormal.y < 1)
        {
            vNormal.y = 0;
        }

        if (vNormal.y > -1 && vNormal.y < 0)
        {
            vNormal.y = -1;
        }



        Vector3 v = vPos - vNormal;

        float fx = v.x + (float)m_kAirObject.SELLWIDTH / 2; // 양수로 만듦
        if (vNormal.x < 0)
        {
            fx--;
        }


        int nx = CovnertNumber(fx) - m_kAirObject.SELLWIDTH / 2;
        float fz = v.z + m_kAirObject.SELLWIDTH / 2;
        if (vNormal.z < 0)
        {
            fz--;
        }



        int nz = CovnertNumber(fz) - m_kAirObject.SELLWIDTH / 2;
        int ny = CovnertNumber(v.y+0.5f);
        if (vNormal.y < 0)
        {
            ny = CovnertNumber(v.y );
            ny--;
        }
        else
        {
            ny = CovnertNumber(v.y );
        }

        return new Vector3(nx + m_kAirObject.SELLWIDTH / 2, ny-1000, nz + m_kAirObject.SELLWIDTH / 2);

    }
    public void ClickEvent()
    {
        int nMask = (1 << LayerMask.NameToLayer("Bound"));
        RaycastHit hit;
        Camera cam= m_gCamera.GetComponent<Camera>();
        if (Physics.Raycast(cam.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
        {


            Quaternion qq = Quaternion.Inverse(GameEdit.Instance.m_kAirObject.transform.rotation);
            Vector3 vNormal = qq * hit.normal;
            int nx, ny, nz;
            Vector3 vv = SelectPos(hit.point, hit.normal);
            if(hit.collider.name=="charblock")
            {
                vv= m_kAirObject.GetChiricPos();
            }
            nx = (int)vv.x;
            ny = (int)vv.y;
            nz = (int)vv.z;

            ClickBlock(nx, ny, nz, vNormal, hit.point);



        }

    }
    bool CheckWeapon(int x,int y,int z)
    {
        // 장비를 부착할 수 있는 자리인가?
        // 앞에 무기가 존재한다
        // 뒤에 무기가 존재한다
        // 앞에 블록이 있다 
        int nblock = m_kAirObject.GetBlock(x, y, z + 1);
        if (nblock > 0) return false;
        return true;

    }
    bool CheckBuster(int x, int y, int z)
    {
        // 장비를 부착할 수 있는 자리인가?
        // 앞에 무기가 존재한다
        // 뒤에 무기가 존재한다
        // 앞에 블록이 있다 
        int nblock = m_kAirObject.GetBlock(x, y, z - 1);
        if (nblock > 0) return false;
        return true;

    }

    public void ClickBlock(int nx, int ny, int nz, Vector3 vNormal,Vector3 vMovePos)
    {

        m_vMovePos = vMovePos;
        int tx, ty, tz;
        tx = nx;
        ty = ny;
        tz = nz;
        m_kSelectBlockData = m_kAirObject.GetBlockData(tx, ty, tz);

        int nBlock = m_kSelectBlockData.nBlock;

        GITEMDATA nData = CWArrayManager.Instance.GetItemData(nBlock);
        

        if(m_bRotateBlock)
        {
            RotateBlock(nx, ny, nz);
            CWResourceManager.Instance.PlaySound("btt6");
            return;
        }


        if (BDelete)
        {
            Delete(nx, ny, nz);
            CWResourceManager.Instance.PlaySound("blockdel");
        }
        else
        {

          

            GITEMDATA cData = CWArrayManager.Instance.GetItemData(_nSelectID);

            if (cData.type=="color")
            {
                int nColor = (int)CWGlobal.GetColorItem((GITEM)_nSelectID);
                if(m_bErase)
                {
                    nColor = 0;
                }
                if(nData.type=="shipblock")
                {
                    int cc= m_kAirObject.GetColor(nx, ny, nz);
                    if(cc==nColor)
                    {
                        return;
                    }
                    AddUndo();
                    m_kAirObject.UpdateColor(nx, ny, nz, nColor);
                    if(!EditInvenDlg.Instance.DelItem(NSelectID))
                    {
                        NSelectID = EditInvenDlg.Instance.NextBlock(NSelectID);
                    }
                    m_bUpdated = true;

                }
            }
            else
            {
                if(cData.type=="shipblock")
                {
                    int nBlockMaxCount = m_nAirBlockCount;
                    if (m_kAirObject.NBlockCount >= nBlockMaxCount)
                    {

                      

                        NoticeMessage.Instance.Show("추가 하려면 업그레이드 해야합니다!");
                        return;
                    }

                }

                int ss = m_kAirObject.GetShape(nx, ny, nz);
                if (ss != (int)BLOCKSHAPE.NORMAL)
                {
                //    NoticeMessage.Instance.Show("정사각형면에만 부착이 가능합니다.");
             //       return;
                }



                CWResourceManager.Instance.PlaySound("blockadd");
                int nFace = 0;
                if (CWMath.IsEqual(vNormal.x, 1))
                {
                    nFace = 2;
                    nx++;
                }
                if (CWMath.IsEqual(vNormal.x, -1))
                {
                    nFace = 3;
                    nx--;
                }

                if (CWMath.IsEqual(vNormal.y, 1))
                {
                    nFace = 4;
                    ny++;
                }
                if (CWMath.IsEqual(vNormal.y, -1))
                {
                    nFace = 5;
                    ny--;
                }

                if (CWMath.IsEqual(vNormal.z, 1))
                {
                    nFace = 1;
                    nz++;
                }
                if (CWMath.IsEqual(vNormal.z, -1))
                {
                    nFace = 0;
                    nz--;
                }
                if(cData.type=="weapon")
                {
                    if (m_kAirObject.NWeaponCount >= 8)
                    {
                        NoticeMessage.Instance.Show("개수를 초과하였습니다!");
                        return;
                    }


                    // 무기 이면서 face==0이면 금지
                    if (nFace==0)
                    {
                        NoticeMessage.Instance.Show("배치 할 수 없는 위치입니다");
                        return;

                    }
                }

                // 무기 이면서 앞면이면 금지
                if (nData.type == "weapon")
                {

                    if (nFace == 1)
                    {
                        NoticeMessage.Instance.Show("배치 할 수 없는 위치입니다");
                        return;
                    }
                }



                if (cData.type == "Buster")
                {
                    if (m_kAirObject.NSpeedCount >= 8)
                    {
                        NoticeMessage.Instance.Show("개수를 초과하였습니다!");
                        return;
                    }

                    if (nFace == 1)
                    {
                        NoticeMessage.Instance.Show("배치 할 수 없는 위치입니다");
                        return;
                    }

                }

                {

                    

                    AddBlock(nx, ny, nz,nFace);
                   // NoticeMessage.Instance.Show("원하는 곳에 탭하면 블록이 추가 됩니다.");

                }

            }



        }



    }
    BLOCKSHAPE CalShape(int nFace)
    {
        if (SELECT_SHAPE == SHAPE.SLANT)
        {
            if (nFace == 0)
            {
                return BLOCKSHAPE.SLANT_1;
            }
            if (nFace == 1)
            {
                return BLOCKSHAPE.SLANT_3;
            }
            if (nFace == 2)
            {
                return BLOCKSHAPE.SLANT_2;
            }
            if (nFace == 3)
            {
                return BLOCKSHAPE.SLANT_4;
            }
            if (nFace == 4)
            {
                return BLOCKSHAPE.SLANT_3;
            }
            if (nFace == 5)
            {
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.SLANT_1) return BLOCKSHAPE.SLANT_8;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.SLANT_2) return BLOCKSHAPE.SLANT_7;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.SLANT_3) return BLOCKSHAPE.SLANT_6;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.SLANT_4) return BLOCKSHAPE.SLANT_5;



                return BLOCKSHAPE.SLANT_5;
            }

        }
        if (SELECT_SHAPE == SHAPE.HSLANT)
        {
            if (nFace == 0)
            {
                return BLOCKSHAPE.HSLANT_1;
            }
            if (nFace == 1)
            {
                return BLOCKSHAPE.HSLANT_3;
            }
            if (nFace == 2)
            {
                return BLOCKSHAPE.HSLANT_2;
            }
            if (nFace == 3)
            {
                return BLOCKSHAPE.HSLANT_4;
            }
            if (nFace == 4)
            {
                return BLOCKSHAPE.HSLANT_3;
            }
            if (nFace == 5)
            {
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.HSLANT_1) return BLOCKSHAPE.HSLANT_8;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.HSLANT_2) return BLOCKSHAPE.HSLANT_7;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.HSLANT_3) return BLOCKSHAPE.HSLANT_6;
                if (m_kSelectBlockData.nShape == (int)BLOCKSHAPE.HSLANT_4) return BLOCKSHAPE.HSLANT_5;

                return BLOCKSHAPE.HSLANT_5;
            }
        }
        if (SELECT_SHAPE == SHAPE.STICK)
        {
            if (nFace == 0)
            {
                return BLOCKSHAPE.STICK_2;
            }
            if (nFace == 1)
            {
                return BLOCKSHAPE.STICK_2;
            }
            if (nFace == 2)
            {
                return BLOCKSHAPE.STICK_1;
            }
            if (nFace == 3)
            {
                return BLOCKSHAPE.STICK_1;
            }
            if (nFace == 4)
            {
                return BLOCKSHAPE.STICK;
            }

            return BLOCKSHAPE.STICK;

        }

        if (SELECT_SHAPE == SHAPE.HALF)
        {
            //_shape = BLOCKSHAPE.HALF;


            if (nFace == 5)
            {
                return BLOCKSHAPE.HALF_F;
            }

            return BLOCKSHAPE.HALF;

        }
        if (SELECT_SHAPE == SHAPE.QURT)
        {
            if (nFace == 5)
            {
                return BLOCKSHAPE.QURT_F;
            }

            return BLOCKSHAPE.QURT;
        }
        if (SELECT_SHAPE == SHAPE.QURT3)
        {
            if (nFace == 5)
            {
                return BLOCKSHAPE.QURT3_F;
            }

            return BLOCKSHAPE.QURT3;
        }
        if (SELECT_SHAPE == SHAPE.STAIR)
        {
            //_shape = BLOCKSHAPE.STAIRS_1;
            if (nFace == 0)
            {
                return BLOCKSHAPE.STAIRS_1;
            }
            if (nFace == 1)
            {
                return BLOCKSHAPE.STAIRS_3;
            }
            if (nFace == 2)
            {
                return BLOCKSHAPE.STAIRS_2;
            }
            if (nFace == 3)
            {
                return BLOCKSHAPE.STAIRS_4;
            }
            if (nFace == 4)
            {
                return BLOCKSHAPE.STAIRS_3;
            }
            if (nFace == 5)
            {
                return BLOCKSHAPE.STAIRS_3_B;
            }

        }

        return BLOCKSHAPE.NORMAL;
    }
    #endregion


    #region BUTTON


    
    
    
    
    // 상위 블록으로 바꾼다 

    struct TEMPDATA
    {
        public int nKey;
        public int nBlock;
        public int nBlock2;
    }
    // 다른 블록으로 바꿔 준다.
    // 개념, 더 높은 블록으로 바꿈
    // 같은 레벨이라면 다른 블록으로 바꿈
    // 

    public void OnChange()
    {
        ChangeBlockDlg.Instance.Open();

        
        //      EditInven.Instance.ReFresh();

        //Dictionary<int, BlockData> kData = m_kAirObject.GetData();
        //List<TEMPDATA> kTemp = new List<TEMPDATA>();
        //// 무게가 넘지 않게 하여야 한다
        //// 원래의 무게를 잰다.
        //// 오버된 무게를 포함해서 더한다. 
        //int nWeight = m_kAirObject.Weight;

        //foreach (var v in kData)
        //{
        //    int nBlock = EditInven.Instance.FindUpLevelItem(v.Value.nBlock);
        //    if (nBlock > 0)
        //    {
        //        GITEMDATA gData1 = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
        //        GITEMDATA gData2 = CWArrayManager.Instance.GetItemData(nBlock);


        //        int nn = gData2.nWeight - gData1.nWeight;
        //        nWeight += nn;
        //        // 슬롯 무게 


        //        TEMPDATA tt = new TEMPDATA();
        //        tt.nKey = v.Key;
        //        tt.nBlock = v.Value.nBlock;
        //        tt.nBlock2 = nBlock;
        //        kTemp.Add(tt);
        //        EditInven.Instance.DelItem(tt.nBlock2);
        //        EditInven.Instance.AddItem(tt.nBlock, 1);
        //    }
        //}
        //foreach (var v in kTemp)
        //{
        //    m_kAirObject.UpdateBlock(v.nKey, v.nBlock2);

        //}
        //m_kAirObject.CalPower();
        //m_bUpdated = true;
        //EditInven.Instance.ReFresh();

    }
    void _Create()
    {
        AddUndo();
        int nFirstBlock = 0;

        Dictionary<int, BlockData> kData = m_kAirObject.GetData();
        foreach (var v in kData)
        {
            if (nFirstBlock == 0)
            {
                GITEMDATA gData = CWArrayManager.Instance.GetItemData(v.Value.nBlock);
                if (gData.type == "shipblock")
                {
                    nFirstBlock = v.Value.nBlock;
                    continue;
                }
            }

            EditInvenDlg.Instance._AddItem(v.Value.nBlock, 1);
        }


        EditInvenDlg.Instance.UpdateData();


        nFirstBlock = (int)GITEM.CoreEngine;
        m_kAirObject.ClearBlock();
        m_kAirObject.AddBlock(16, 5, 16, nFirstBlock, 0, 0);
        

    }
    public void OnCreate()
    {
        MessageBoxDlg.Instance.Show(_Create, null, "블록조립", "새로 만들겠습니까?");

    }

    public void OnBlock()
    {
    //    EditInven.Instance.Open();
        BDelete = false;

    }
    public void OnDelete()
    {
        BDelete = !BDelete;
    }
    public void OnUndo()
    {
        Undo();
    }
    // 나갈때 자동 저장
    // 부스터와, 무기가 있는지 확인한다. 
    public void OnAirShop()
    {
       

    }
    public void OnOpenChangeBlockDlg()
    {
        ChangeBlockDlg.Instance.Open();
    }
    public void OnShapeShop()
    {
        m_State = STATE.BLOCK;
        ShapeShopDlg.Instance.Open();
    }
    void __CloseEdit()
    {
        Close();
    }
    void SaveExit()
    {
        __CloseEdit();
        CaptureImage();
        CopyAirBlock();
        
    }
    public void OnSave()
    {
        byte[] buffer = m_kAirObject.GetJSonByte();

        m_kAirObject.CalPower();
        CWSocketManager.Instance.UpdateAirObject(m_nAirSlotID,m_kAirObject.GetMaxHP(),m_kAirObject.GetDamage(),m_kAirObject.NBlockCount, buffer, () => {

            NoticeMessage.Instance.Show("비행기가 저장되었습니다.");
        });

    }
    void CopyAirBlock()
    {

        byte[] buffer = m_kAirObject.GetJSonByte();

        m_kAirObject.CalPower();
        CWSocketManager.Instance.UpdateAirObject(m_nAirSlotID, m_kAirObject.GetMaxHP(), m_kAirObject.GetDamage(), m_kAirObject.NBlockCount, buffer, () => {

            //NoticeMessage.Instance.Show("비행기가 저장되었습니다.");
            EditInvenDlg.Instance.MoveToInven(() => {
                //CWADManager.Instance.Show();
            });// 인벤으로 

        });

        //CWSocketManager.Instance.UpdateAirObject(m_nAirSlotID, buffer,()=> {
            
        //    EditInvenDlg.Instance.MoveToInven(()=> {
        //        //CWADManager.Instance.Show();
        //    });// 인벤으로 
        //});
        


    }
    void ReBlock()
    {
        EditInvenDlg.Instance.ReInven();
        __CloseEdit();
    }

    bool CheckExit()
    {
        // 필요 아이템이 없다면 나가면 안됨
        if (!m_kAirObject.CheckWeaponItem())
        {

            NoticeMessage.Instance.Show("무기를 장착해야 합니다.");
            return false;
        }
        if (!m_kAirObject.CheckBusterItem())
        {

            NoticeMessage.Instance.Show("부스터를 장착해야 합니다.");
            return false;
        }

        if (!m_kAirObject.CheckShipblockItem())
        {

            NoticeMessage.Instance.Show("블록을 장착해야 합니다.");
            return false;
        }
        if (!m_kAirObject.CheckCharBlockItem())
        {

            NoticeMessage.Instance.Show("캐릭터를 장착해야 합니다");
            return false;
        }

        if (m_bUpdated)
        {
            m_bUpdated = false;
            MessageBoxDlg.Instance.Show(SaveExit, ReBlock, "저장", "저장하시겠습니까?");
            return false;
        }

        return true;
    }
    public void OnExitEdit()
    {
        if (!CheckExit()) return;
        __CloseEdit();

    }
    public void DeleteButton()
    {
        BDelete =!BDelete;
        
    }




    void CloseAirShop()
    {
        gameObject.SetActive(true);
    }
    // 자동으로 최대 블록을 만든다. 모양을 고르는 개념 추가 
    public void AutoShape()
    {
        gameObject.SetActive(false);

        //ShopDlg.Instance.AirShow(CloseAirShop);


    }

   

    public void OnRotateReset()
    {
        m_kAirObject.transform.localPosition = Vector3.zero;
        m_kAirObject.transform.eulerAngles = Vector3.zero;
        m_kAirObject.transform.localScale = Vector3.one;

    }

    #endregion

    #region 중심 축 
/*
    public Text Lefttext;
    public Image CenterImage;

    public Text Righttext;
    
    // 코어블록을 중심으로 왼쪽개수, 오른쪽 개수 
    void UpdateCenterCount()
    {
        if(m_kUnitClass!=null)
        {
            
            return;
        }

        int Left = 0;
        int Right = 0;
        foreach(var v in m_kAirObject.GetData())
        {
            int x = m_kAirObject.GetSellX(v.Key);
            int y = m_kAirObject.GetSellY(v.Key);
            int z = m_kAirObject.GetSellZ(v.Key);
            if (x < 15) Left++;
            if (x > 15) Right++;
        }
        Lefttext.text = Left.ToString();
        Righttext.text = Right.ToString();
        if(Left!=Right)
        {
            CenterImage.color = Color.red;
        }
        else
        {
            CenterImage.color = Color.green;
        }

    }
    */
    #endregion

    #region  모양버튼 

    //enum SHAPE {NORMAL, SLANT, HSLANT, HALF, QURT3, QURT,  STICK,STAIR };
    enum SHAPE { NORMAL, QURT3, STICK, STAIR, HALF, QURT, SLANT, HSLANT};

    SHAPE __SHAPE;
    SHAPE SELECT_SHAPE
    {
        get { return __SHAPE; }
        set
        {
            __SHAPE = value;

            UpdateSelect();
        }
    }

    public void OnSamgak()
    {
        SELECT_SHAPE = SHAPE.SLANT;

    }
    public void OnHalfSamgak()
    {
        SELECT_SHAPE = SHAPE.HSLANT;
    }

    // 반토막 
    public void OnHalfSagak()
    {
        SELECT_SHAPE = SHAPE.HALF;

    }
    //1/4 크기 
    public void OnHalf4Sagak()
    {
        SELECT_SHAPE = SHAPE.QURT;
    }
    //3/4 크기 블록 
    public void OnQURT3Sagak()
    {
        SELECT_SHAPE = SHAPE.QURT3;
    }
    #region 토글 변환 회전

    bool m_bShapeToggle;
    public void OnNormal()
    {
        m_bShapeToggle = !m_bShapeToggle;
        SELECT_SHAPE = SHAPE.NORMAL;
    }
    #endregion

    // 막대기 
    public void OnSteak()
    {
        SELECT_SHAPE = SHAPE.STICK;
    }
    // 계단
    public void OnStair()
    {
        SELECT_SHAPE = SHAPE.STAIR;
    }
    public void OnSelectShape(int num)
    {
        if (m_State == STATE.COLOR) return;
        // 컬러모드 인가?
        SELECT_SHAPE = (SHAPE)(num+1);
    }

    #endregion

    #region 회전 블록

    public GameObject m_gRotate;
    public CWButton m_kRotateBtn;
    bool _bRotateBlock;
    bool m_bRotateBlock
    {
        get
        {
            return _bRotateBlock;
        }
        set
        {
            _bRotateBlock = value;
            m_State = STATE.ROTATE;
            //if(value)
            //    BDelete = false;
            //NSelectID = 0;
            //m_gRotate.SetActive(value);
            //m_kRotateBtn.SetDisableView(!_bRotateBlock);
            //UpdateSelect();
            //if(value==true)
            //{
            //    //TipMessageDlg.Instance.Show("모양을 블록을 클릭하면 회전을 합니다.");
            //    NoticeMessage.Instance.Show("모양을 블록을 클릭하면 회전을 합니다.");
            //}
        }
    }
    
    // 회전을 하는 상태로 바꾼다
    public void OnRotateBlock()
    {
        m_bRotateBlock = !m_bRotateBlock;

    }
    int NextShape(int nShape)
    {
        BLOCKSHAPE kShape = (BLOCKSHAPE)nShape;
        //if (kShape == BLOCKSHAPE.STICK)
        //{
        //    return (int)BLOCKSHAPE.STICK_1;
        //}
        //if (kShape == BLOCKSHAPE.STICK_1)
        //{
        //    return (int)BLOCKSHAPE.STICK_2;
        //}
        //if (kShape == BLOCKSHAPE.STICK_2)
        //{
        //    return (int)BLOCKSHAPE.STICK_3;
        //}

        //if (kShape == BLOCKSHAPE.STICK_3)
        //{
        //    return (int)BLOCKSHAPE.STICK_4;
        //}

        //if (kShape == BLOCKSHAPE.STICK_4)
        //{
        //    return (int)BLOCKSHAPE.STICK;
        //}


        if (kShape == BLOCKSHAPE.SLANT_1)
        {
            return (int)BLOCKSHAPE.SLANT_2;
        }
        if (kShape == BLOCKSHAPE.SLANT_2)
        {
            return (int)BLOCKSHAPE.SLANT_3;
        }
        if (kShape == BLOCKSHAPE.SLANT_3)
        {
            return (int)BLOCKSHAPE.SLANT_4;
        }
        if (kShape == BLOCKSHAPE.SLANT_4)
        {
            return (int)BLOCKSHAPE.SLANT_5;
        }
        if (kShape == BLOCKSHAPE.SLANT_5)
        {
            return (int)BLOCKSHAPE.SLANT_6;
        }
        if (kShape == BLOCKSHAPE.SLANT_6)
        {
            return (int)BLOCKSHAPE.SLANT_7;
        }

        if (kShape == BLOCKSHAPE.SLANT_7)
        {
            return (int)BLOCKSHAPE.SLANT_8;
        }
        if (kShape == BLOCKSHAPE.SLANT_8)
        {
            return (int)BLOCKSHAPE.SLANT_1;
        }



        if (kShape == BLOCKSHAPE.HSLANT_1)
        {
            return (int)BLOCKSHAPE.HSLANT_2;
        }
        if (kShape == BLOCKSHAPE.HSLANT_2)
        {
            return (int)BLOCKSHAPE.HSLANT_3;
        }
        if (kShape == BLOCKSHAPE.HSLANT_3)
        {
            return (int)BLOCKSHAPE.HSLANT_4;
        }
        if (kShape == BLOCKSHAPE.HSLANT_4)
        {
            return (int)BLOCKSHAPE.HSLANT_5;
        }
        if (kShape == BLOCKSHAPE.HSLANT_5)
        {
            return (int)BLOCKSHAPE.HSLANT_6;
        }
        if (kShape == BLOCKSHAPE.HSLANT_6)
        {
            return (int)BLOCKSHAPE.HSLANT_7;
        }
        if (kShape == BLOCKSHAPE.HSLANT_7)
        {
            return (int)BLOCKSHAPE.HSLANT_8;
        }
        if (kShape == BLOCKSHAPE.HSLANT_8)
        {
            return (int)BLOCKSHAPE.HSLANT_1;
        }


      

        if (kShape == BLOCKSHAPE.STAIRS_1)
        {
            return (int)BLOCKSHAPE.STAIRS_2;
        }
        if (kShape == BLOCKSHAPE.STAIRS_2)
        {
            return (int)BLOCKSHAPE.STAIRS_3;
        }
        if (kShape == BLOCKSHAPE.STAIRS_3)
        {
            return (int)BLOCKSHAPE.STAIRS_4;
        }
        if (kShape == BLOCKSHAPE.STAIRS_4)
        {
            return (int)BLOCKSHAPE.STAIRS_1;
        }


        if (kShape == BLOCKSHAPE.STAIRS_1_B)
        {
            return (int)BLOCKSHAPE.STAIRS_2_B;
        }
        if (kShape == BLOCKSHAPE.STAIRS_2_B)
        {
            return (int)BLOCKSHAPE.STAIRS_3_B;
        }
        if (kShape == BLOCKSHAPE.STAIRS_3_B)
        {
            return (int)BLOCKSHAPE.STAIRS_4_B;
        }
        if (kShape == BLOCKSHAPE.STAIRS_4_B)
        {
            return (int)BLOCKSHAPE.STAIRS_1_B;
        }


        return nShape;

    }
    void RotateBlock(int x,int y,int z)
    {
        int nShape = m_kAirObject.GetShape(x, y, z);
        if(nShape==0)
        {
            NoticeMessage.Instance.Show("모양 블록을 클릭하여야 합니다!");
            return;
        }
        AddUndo();
        int nextShape = (byte)NextShape(nShape);
        m_kAirObject.UpdateShape(x, y, z, nextShape);
        m_bUpdated = true;
        
    }

    #endregion

    #region 상태 관리

    enum STATE {BLOCK,COLOR,DELETE,ROTATE };

    STATE _State;
    STATE m_State
    {
        get
        {
            return _State;
        }
        set
        {
            _State = value;
            UpdateState();
        }
    }

    public GameObject[] m_gStates;

    void UpdateState()
    {

        foreach (var v in m_gStates)
        {
            if(v)
            {
                CWButton cc = v.gameObject.GetComponentInChildren<CWButton>();
                cc.SetDisableView(true);

            }
        }

        if (m_State==STATE.BLOCK)
        {
            
            _bDelete = false;
            _bRotateBlock = false;

            // 회전 블록 디저블
            if(m_gRotate)
                m_gRotate.SetActive(false);
            //m_kRotateBtn.SetDisableView(true);

            // 디릴트 디저블
            if(m_gDelete)
                m_gDelete.SetActive(false);
            //m_kDeleteBtn.SetDisableView(true);

            //컬러 디저블
            if(m_gColorUI)
                m_gColorUI.SetActive(false);

            m_tModeTextUI.text = "설치 모드";


               
            ValueUI.g_kSelectItemData = CWArrayManager.Instance.GetItemData(_nSelectID);

            UpdateSelect();

        }
        if (m_State == STATE.COLOR)
        {

            _bDelete = false;
            _bRotateBlock = false;

            // 블록 disable
            m_kSelectBlock.gameObject.SetActive(false);
            // 회전 블록 디저블
            if(m_gRotate)
                m_gRotate.SetActive(false);
            //m_kRotateBtn.SetDisableView(true);
            // 디릴트 디저블
            if(m_gDelete)
                m_gDelete.SetActive(false);
            //m_kDeleteBtn.SetDisableView(true);
            if (m_gColorUI)
                m_gColorUI.SetActive(true);

            if (_nColor == (int)COLORNUMBER.WHITE)
            {
                if (m_gEraser)
                    m_gEraser.SetActive(true);
                if(m_gColor)
                    m_gColor.SetActive(false);
            }
            else
            {
                Color kcolor = CWGlobal.GetColor((COLORNUMBER)_nColor);
                kcolor.a = 1;
                if(m_kColor)
                    m_kColor.color = kcolor;
                if (m_gEraser)
                    m_gEraser.SetActive(false);
                if (m_kColor)
                    m_gColor.SetActive(true);
            }

            m_tModeTextUI.text = "색칠 모드";

        }
        if (m_State == STATE.DELETE)
        {

            _bRotateBlock = false;
            if (m_gDelete)
                m_gDelete.SetActive(true);
            //m_kDeleteBtn.SetDisableView(false);

            m_kSelectBlock.gameObject.SetActive(false);
            // 회전 블록 디저블
            if (m_gRotate)
                m_gRotate.SetActive(false);
            //m_kRotateBtn.SetDisableView(true);
            // 디릴트 디저블
            if (m_gColorUI)
                m_gColorUI.SetActive(false);

            m_tModeTextUI.text = "지우기 모드";

        }
        if (m_State == STATE.ROTATE)
        {
            _bDelete = false;
            if (m_gRotate)
                m_gRotate.SetActive(true);
            //m_kRotateBtn.SetDisableView(false);

            m_kSelectBlock.gameObject.SetActive(false);
            if (m_gDelete)
                m_gDelete.SetActive(false);
            //m_kDeleteBtn.SetDisableView(true);
            if (m_gColorUI)
                m_gColorUI.SetActive(false);

            m_tModeTextUI.text = "회전 모드";
        }



        CWButton cc2 = m_gStates[(int)m_State].gameObject.GetComponentInChildren<CWButton>();
        cc2.SetDisableView(false);



    }


    #endregion

    #region 비행기 캡처

    public Camera m_gCapture;
    void CaptureImage()
    {
        if (m_gCapture == null) return;


        string szname = string.Format("Ship_{0}", m_nAirSlotID);
        string szPath = string.Format("{0}/{1}", Application.persistentDataPath, szname);

        CWLib.MakeImage(m_gCapture, szPath,new Color(0,0,0,255));

    }


    #endregion



}
