using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CWEnum;
using CWUnityLib;

using CWStruct;
using System.IO;

public class AirPlaneEdit : MonoBehaviour {


    public Camera m_kCaptureCam;
    public BLOCKSHAPE m_Shape;

    public enum SHAPE { NORMAL, SLANT, HSLANT, HALF, QURT3, QURT, STICK, STAIR };
    public SHAPE SELECT_SHAPE
    {
        get
        {
            if (m_Shape == BLOCKSHAPE.STICK) return SHAPE.STICK;
            if (m_Shape == BLOCKSHAPE.STICK_1) return SHAPE.STICK;
            if (m_Shape == BLOCKSHAPE.STICK_2) return SHAPE.STICK;
            if (m_Shape == BLOCKSHAPE.STICK_3) return SHAPE.STICK;
            if (m_Shape == BLOCKSHAPE.STICK_4) return SHAPE.STICK;

            if (m_Shape == BLOCKSHAPE.SLANT_1) return SHAPE.SLANT;
            if (m_Shape == BLOCKSHAPE.SLANT_2) return SHAPE.SLANT;
            if (m_Shape == BLOCKSHAPE.SLANT_3) return SHAPE.SLANT;
            if (m_Shape == BLOCKSHAPE.SLANT_4) return SHAPE.SLANT;

            if (m_Shape == BLOCKSHAPE.HSLANT_1) return SHAPE.HSLANT;
            if (m_Shape == BLOCKSHAPE.HSLANT_2) return SHAPE.HSLANT;
            if (m_Shape == BLOCKSHAPE.HSLANT_3) return SHAPE.HSLANT;
            if (m_Shape == BLOCKSHAPE.HSLANT_4) return SHAPE.HSLANT;

            if (m_Shape == BLOCKSHAPE.HALF) return SHAPE.HALF;

            if (m_Shape == BLOCKSHAPE.QURT3) return SHAPE.QURT3;
            if (m_Shape == BLOCKSHAPE.QURT) return SHAPE.QURT;

            if (m_Shape == BLOCKSHAPE.STAIRS_1) return SHAPE.STAIR;
            if (m_Shape == BLOCKSHAPE.STAIRS_2) return SHAPE.STAIR;
            if (m_Shape == BLOCKSHAPE.STAIRS_3) return SHAPE.STAIR;
            if (m_Shape == BLOCKSHAPE.STAIRS_4) return SHAPE.STAIR;
            


            return SHAPE.NORMAL;
        }
    }




    public COLORNUMBER m_Color;
    public string m_szName = "noname";
    CWAirObject m_kAirObject;

    public CWAirObject m_kAir;

    public CWAirObject m_kCopyBuild;

    public SHIPBLOCK m_kSelectItem;

    public Text m_kInfo;
    public Text m_kInfo2;
    public InputField m_kInput;
    public GameObject m_gSelect;
    // Use this for initialization
    
    public bool m_bChange = true;
    public bool m_bAutoSave = false;

    public bool m_bColor;
    public bool m_bShape;
    public bool m_bBlock;

    public BlockConverter m_BlockConverter;
    public GameObject m_gMeshDir;
    public List<Dictionary<int, BlockData>> m_kUndo = new List<Dictionary<int, BlockData>>();

    public string m_szBlockCopy;




    BlockData m_kSelectBlock=new BlockData();// 현재 선택된 블록

    Vector3Int m_vMouse = new Vector3Int();
    Vector3 m_vNormal = new Vector3();

    CWAirBlockCopy m_kCopyData = new CWAirBlockCopy();

    void Start()
    {
        m_kAir.Create(0);

        
        m_kCopyBuild.Create(0);
        m_kCopyBuild.gameObject.SetActive(false);
        m_BlockConverter.m_gTarget = m_gMeshDir;
        m_kAirObject = m_kAir;
        m_kAirObject.name = "noname";
        m_kAirObject.EmptyCreate();
        m_kAirObject.m_bMeshCollider = true;

        //CWLib.SetGameObjectLayer(m_kAirObject.gameObject, LayerMask.NameToLayer("Detect"));
    }
    void AddUndo()
    {
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();

        Dictionary<int, BlockData> nData = new Dictionary<int, BlockData>();

        foreach (var v in kData)
        {
            nData.Add(v.Key, v.Value);
        }
        m_kUndo.Add(nData);



    }
    void Undo()
    {
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();

        int tcnt = m_kUndo.Count - 1;
        if (tcnt == 0) return;

        Dictionary<int, BlockData> nData = m_kUndo[tcnt];

        kData.Clear();
        foreach (var v in nData)
        {
            kData.Add(v.Key, v.Value);
        }
        m_kAirObject.m_bUpdated = true;

        m_kUndo.Remove(nData);
        



    }
    public void ClickBlock(int nx, int ny, int nz, Vector3 vNormal)
    {

        if (nx < 0) return;
        if (ny < 0) return;
        if (nz < 0) return;

        if (nx >= 32) return;
        if (ny >= 32) return;
        if (nz >= 32) return;


        {

            BlockData bs = m_kAirObject.GetBlockData(nx, ny, nz);
            m_kSelectBlock = bs;

            if (m_bChange)
            {

                
                int nColor = bs.nColor;
                int nShape = bs.nShape;
                int nBlock = bs.nBlock;
                
                if (m_bColor)
                {
                    nColor = (int)m_Color;
                }
                if (m_bShape)
                {
                    nShape = (int)m_Shape;
                }
                if(m_bBlock)
                {
                    nBlock = (int)m_kSelectItem;
                }
                m_kAirObject.AddBlock(nx, ny, nz, nBlock, nShape, nColor);


            }
            else
            {
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



                AddBlock(nx, ny, nz, nFace);
            }
            

        }



    }
    public void Convert_SetBlock(int x,int y,int z)
    {
        x += 15;
        z += 15;
        if (x <= 0) return;
        if (y <= 0) return;
        if (z <= 0) return;
        if (x > 30) return;
        if (y > 30) return;
        if (z > 30) return;

       // if (x < 8 && z < 8) return;

        m_kAirObject.AddBlock(x, y, z, (int)GITEM.tree, (int)BLOCKSHAPE.NORMAL,(int) COLORNUMBER.NONE);
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
                if(m_kSelectBlock.nShape==(int) BLOCKSHAPE.SLANT_1) return BLOCKSHAPE.SLANT_8;
                if(m_kSelectBlock.nShape==(int) BLOCKSHAPE.SLANT_2) return BLOCKSHAPE.SLANT_7;
                if(m_kSelectBlock.nShape==(int) BLOCKSHAPE.SLANT_3) return BLOCKSHAPE.SLANT_6;
                if(m_kSelectBlock.nShape==(int) BLOCKSHAPE.SLANT_4) return BLOCKSHAPE.SLANT_5;



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
                if (m_kSelectBlock.nShape == (int)BLOCKSHAPE.HSLANT_1) return BLOCKSHAPE.HSLANT_8;
                if (m_kSelectBlock.nShape == (int)BLOCKSHAPE.HSLANT_2) return BLOCKSHAPE.HSLANT_7;
                if (m_kSelectBlock.nShape == (int)BLOCKSHAPE.HSLANT_3) return BLOCKSHAPE.HSLANT_6;
                if (m_kSelectBlock.nShape == (int)BLOCKSHAPE.HSLANT_4) return BLOCKSHAPE.HSLANT_5;

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
    public void AddBlock(int x, int y, int z, int nFace)
    {

        m_Shape = CalShape(nFace);

        m_kAirObject.AddBlock(x, y, z, (int)m_kSelectItem, (int)m_Shape,(int)m_Color);

        if(m_bAutoSave)
        {
            m_kAutoFile.AddData(x, y, z, (int)m_kSelectItem, (int)m_Shape);
        }
    }

    public Vector3 SelectPos(Vector3 vPos, Vector3 vNormal)
    {
        Vector3 v = vPos - vNormal;

        float fx = v.x + (float)m_kAirObject.SELLWIDTH / 2; // 양수로 만듦
        if (vNormal.x < 0)
        {
            fx--;
        }


        int nx = (int)fx - m_kAirObject.SELLWIDTH / 2;
        float fz = v.z + m_kAirObject.SELLWIDTH / 2;
        if (vNormal.z < 0)
        {
            fz--;
        }



        int nz = (int)fz - m_kAirObject.SELLWIDTH / 2;
        int ny = (int)v.y;

        if (vNormal.y < 0)
        {
            ny--;
        }

        return new Vector3(nx+ m_kAirObject.SELLWIDTH / 2, ny, nz+m_kAirObject.SELLWIDTH / 2);

    }
    public void OnPress()
    {
        if (Input.GetMouseButton(0))
        {
            
            int nMask = (1 << LayerMask.NameToLayer("Bound"));
            RaycastHit hit;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
            {
                Quaternion qq = Quaternion.Inverse(m_kAirObject.transform.rotation);
                Vector3 vNormal = qq * hit.normal;
                AddUndo();
                Vector3 vv = SelectPos(hit.point,hit.normal);
                //m_kInfo.text = string.Format("{0} - {1}", hit.point,vv);
                ClickBlock((int)vv.x, (int)vv.y, (int)vv.z, vNormal);
             }


         }

    }

    int NextShape(int nShape)
    {
        BLOCKSHAPE kShape = (BLOCKSHAPE)nShape;
        if(kShape==BLOCKSHAPE.STICK)
        {
            return (int)BLOCKSHAPE.STICK_1;
        }
        if (kShape == BLOCKSHAPE.STICK_1)
        {
            return (int)BLOCKSHAPE.STICK_2;
        }
        if (kShape == BLOCKSHAPE.STICK_2)
        {
            return (int)BLOCKSHAPE.STICK_3;
        }

        if (kShape == BLOCKSHAPE.STICK_3)
        {
            return (int)BLOCKSHAPE.STICK_4;
        }

        if (kShape == BLOCKSHAPE.STICK_4)
        {
            return (int)BLOCKSHAPE.STICK;
        }


        if (kShape==BLOCKSHAPE.SLANT_1)
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
            return (int)BLOCKSHAPE.HSLANT_1;
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
            return (int)BLOCKSHAPE.HSLANT_5;
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
            return (int)BLOCKSHAPE.SLANT_5;
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


        return 0;

    }
    int NextColor(int nColor)
    {
        nColor++;
        if (nColor >= 16) nColor = 0;
        return nColor;
    }

    void Update()
    {
        if(m_kAirObject.m_bUpdated)
        {
            m_kAirObject.CalPower();
            Dictionary<int, BlockData> kdata = m_kAirObject.GetData();
            m_kInfo2.text = string.Format("블록개수 : {0} HP :{1} 가격 : {2}", kdata.Count, m_kAirObject.GetHP(), m_kAirObject.GetPrice());

        }

        m_kAirObject.UpdateBlock();
        if(m_kCopyBuild.gameObject.activeSelf)
            m_kCopyBuild.UpdateBlock();
        int nBlock = 0;
        if(m_kInput.isFocused)
        {
            return;
        }  

        
        int nMask = (1 << LayerMask.NameToLayer("Bound"));
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, nMask))
        {
            Quaternion qq = Quaternion.Inverse(m_kAirObject.transform.rotation);
            Vector3 vNormal = qq * hit.normal;
          //  print(string.Format("{0} {1} {2} {3}", hit.collider.name, hit.point, hit.normal, vNormal));

            int nx, ny, nz;
            Vector3 vv = SelectPos(hit.point, hit.normal);
            nx = (int)vv.x;
            ny = (int)vv.y;
            nz = (int)vv.z;

            Vector3 vPos = m_kAirObject.GetBlockPosition(nx, ny, nz);

            m_vMouse.x = nx;
            m_vMouse.y = ny;
            m_vMouse.z = nz;
            m_vNormal = vNormal;

            m_gSelect.transform.localPosition = vPos;


            nBlock = m_kAirObject.GetBlock(nx,ny,nz);

            BlockData bs = m_kAirObject.GetBlockData(nx, ny, nz);
            m_kSelectBlock = bs;


            if (Input.GetKeyDown(KeyCode.Delete))
            {
                AddUndo();
                

                if (!m_kAirObject.DelBlock_AirEdit(nx, ny, nz))
                {

                    Debug.LogError("끊어진것 있다");
                }



            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                m_kSelectItem = (SHIPBLOCK)nBlock;
            }

            if (Input.GetKeyDown(KeyCode.R))// 회전
            {

                

                m_kSelectBlock.nShape = (byte)NextShape((int)(m_kSelectBlock.nShape));
                m_kAirObject.AddBlock(nx, ny, nz, nBlock, m_kSelectBlock.nShape, m_kSelectBlock.nColor);
            }
            if (Input.GetKeyDown(KeyCode.C))// 컬러
            {
                // 선택된 블록을 회전
                m_kSelectBlock.nColor = (byte)NextColor((int)m_kSelectBlock.nColor);
                m_kAirObject.AddBlock(nx, ny, nz, nBlock, m_kSelectBlock.nShape, m_kSelectBlock.nColor);
            }


            string szname = CWArrayManager.Instance.GetItemName(nBlock);
            int ncolor = m_kAirObject.GetColor(nx, ny, nz);
            int ns = m_kAirObject.GetShape(nx, ny, nz);

            m_kInfo.text = string.Format("{0},{1},{2}={3}({4}) color: {5} shape:{6}", nx, ny, nz, nBlock, szname, ncolor, ns);


                



        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            Undo();
        }
        if (Input.GetKeyDown(KeyCode.W))
        {
            m_kSelectItem = SHIPBLOCK.Gun;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            m_kSelectItem = SHIPBLOCK.Buster;
        }


    }
    void _SelectBrush(int num)
    {

        m_kSelectItem = (SHIPBLOCK)(num + 1);
    }
    public void SelectBrush()
    {
        //ItemListDlg.Instance.Open(_SelectBrush);
    }
    void LoadFile(string szfile)
    {
        
        m_kAirObject.ClearBlock();

        m_kAirObject = m_kAir;
        m_szName = szfile;
        m_kAirObject.Load(m_szName);

        
        Dictionary<int, BlockData> kdata = m_kAirObject.GetData();

        m_kInfo2.text = string.Format("블록개수 : {0} HP :{1} 가격 : {2}", kdata.Count, m_kAirObject.GetHP(), m_kAirObject.GetPrice());
        CWLib.SetGameObjectLayer(m_kAirObject.m_gCenterObject, LayerMask.NameToLayer("Bound"));
    }
    void _Load(int num)
    {
        
        m_kAirObject.ClearBlock();
        
        m_kAirObject = m_kAir;


        //m_szName = AirObjectDlg.Instance.name;




        m_kAirObject.Load(m_szName);
        


        Dictionary<int, BlockData> kdata = m_kAirObject.GetData();
        
        m_kInfo2.text = string.Format("블록개수 : {0} HP :{1} 가격 : {2}", kdata.Count, m_kAirObject.GetHP(), m_kAirObject.GetPrice());

        CWLib.SetGameObjectLayer(m_kAirObject.m_gCenterObject, LayerMask.NameToLayer("Bound"));
    }
    public void NewFile()
    {
        m_kAirObject.name = "noname";
        m_kAirObject.Close();
        m_kAirObject.Create(0);
        
        
        m_kAirObject.EmptyCreate();
        m_kAirObject = m_kAir;
        m_kAirObject.AddBlock(m_kAirObject.SELLWIDTH / 2, 5, m_kAirObject.SELLWIDTH / 2, (int)GITEM.CoreEngine, (int)m_Shape, (int)m_Color);

        Dictionary<int, BlockData> kdata = m_kAirObject.GetData();
        m_kInfo2.text = string.Format("블록개수 : {0} HP :{1}", kdata.Count, m_kAirObject.GetHP());
        CWLib.SetGameObjectLayer(m_kAirObject.m_gCenterObject, LayerMask.NameToLayer("Bound"));

    }
    public void DeleteFile()
    {
        m_kSelectItem = SHIPBLOCK.NONE;
    }
    public void Load()
    {
        LoadDialogBox.Instance.Show("AirCraft", LoadFile);
        
    }
    

    public void Save()
    {
        if (m_szName == "noname") return;
        m_kAirObject.SetBlock(16, 5, 16, (int)GITEM.CoreEngine, 0, 0);
        string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, m_szName);
        m_kAirObject.Save(szpath);
    }
    public void SaveAs()
    {
        string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, m_szName);
        m_kAirObject.Save(szpath);
    }

    void AddMesh(string szName)
    {
        // 
        GameObject gg= CWLib.FindChild(m_kAirObject.m_gMeshDir, szName);
        if (gg != null) return;
        GameObject gNew = CWResourceManager.Instance.GetPrefab(szName);
        m_kAirObject.m_gMeshDir.transform.AddChild(gNew);
        gNew.name = szName;


    }
    
    


    CWAutoAirFile m_kAutoFile = new CWAutoAirFile();

    public Vector3Int m_StartPos=new Vector3Int(15,6,13);

    public void AutoStart()
    {
        m_kAutoFile.Create();
        m_bAutoSave = true;
    }
    public void AutoSave()
    {
        m_kAutoFile.Save();
        CWFileManager.Instance.Save();
        
    }
    public void AutoPlay()
    {
        m_kAutoFile.Load();
        StartCoroutine("AutoRunPlay");
    }
    IEnumerator AutoRunPlay()
    {
        m_kAirObject.ClearBlock();
        yield return null;
        foreach (var v in m_kAutoFile.m_kOrder)
        {
            m_kAirObject.AddBlock(v.x,v.y,v.z,v.nItemNumber,v.nShape,(int)COLORNUMBER.WHITE);
            yield return new WaitForSeconds(0.2f);
        }
    }



    Dictionary<int, int> m_kConvertData = new Dictionary<int, int>();

    int ChangeBlock(int x,int y,int z, int nBlock)
    {
        if (nBlock == 0) return 0;
        if(m_kConvertData.ContainsKey(nBlock))
        {
            return m_kConvertData[nBlock];
        }

        return 1;
    }
    int SystemChangeBlock(int x, int y, int z, int nBlock)
    {
        if (nBlock == (int)GITEM.Gun) return 0;
        if (nBlock == (int)GITEM.Buster) return 0;
        return nBlock;
    }

    void FixBlockSystem()
    {
        TextAsset[] txts = Resources.LoadAll<TextAsset>("AirCraft");
        foreach (var v in txts)
        {
            string szname = v.name;
            m_kAirObject._ChangeBlock = SystemChangeBlock;

            
            m_kAirObject.Load(szname);
          //  m_kAirObject.AddBlock(16, 5, 16, (int)GITEM.CoreEngine, 0, 0);

            string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, szname);
            m_kAirObject.Save(szpath);

        }

    }
    void ConvertFile()
    {

        CWTableManager.CWLoadCSV cs = CWTableManager.Instance.GetTable("변환아이템 - 시트1");
        if (cs != null)
        {
            foreach (var v in cs.m_mkData)
            {
                int nID = cs.GetInt(v.Key, "ID");
                int nNew = cs.GetInt(v.Key, "New");
                if(!m_kConvertData.ContainsKey(nID))
                {
                    m_kConvertData.Add(nID, nNew);
                }
                
            }

        }

        TextAsset[] txts = Resources.LoadAll<TextAsset>("AirCraft");
        foreach (var v in txts)
        {
            string szname = v.name;
            m_kAirObject._ChangeBlock = ChangeBlock;
            m_kAirObject.Load(szname);
            

            string szpath = string.Format("{0}/Resources/AirCraft/{1}.bytes", Application.dataPath, szname);
            m_kAirObject.Save(szpath);



        }

    }
    struct TDATA
    {
        public int x;
        public int y;
        public int z;
        public ushort nBlock;// 비행기는 아이템, 건물은 블록
        public byte nShape;
        public byte nColor;

    }
    void RotateBlock(int angle)
    {
        AddUndo();
        CWAirObject kTemp = m_kAirObject;
        if (m_kCopyBuild.gameObject.activeSelf)
        {
            kTemp = m_kCopyBuild;
        }

        int sx = 0;
        int sy = 0;
        int sz = 0;
        int ex = 0;
        int ey = 0;
        int ez = 0;
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        List<TDATA> kList = new List<TDATA>();
        int SELLWIDTH = m_kAir.SELLWIDTH;
        for (int z = sz; z < ez; z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    

                    BlockData  bs= kTemp.GetBlockData(x, y, z);
                    if (bs.nBlock == 0) continue;

                    Vector3 vv = CWMath.CalYaw(angle, new Vector3(x, y , z));

                    TDATA kdata = new TDATA();


                    kdata.x = ((int)vv.x+ SELLWIDTH)% SELLWIDTH;
                    kdata.y = y;
                    kdata.z = ((int)vv.z + SELLWIDTH) % SELLWIDTH;
                    kdata.nBlock = bs.nBlock;
                    kdata.nColor = bs.nColor;
                    kdata.nShape = bs.nShape;
                    if(kdata.nShape==(int)BLOCKSHAPE.SLANT_1)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_4;
                    }
                    else if(kdata.nShape==(int)BLOCKSHAPE.SLANT_2)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_1;
                    }
                    else if (kdata.nShape==(int)BLOCKSHAPE.SLANT_3)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_2;
                    }
                    else if (kdata.nShape==(int)BLOCKSHAPE.SLANT_4)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_3;
                    }


                    if (kdata.nShape == (int)BLOCKSHAPE.SLANT_5)//1+4
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_8;//4+4
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.SLANT_6)//2+4
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_5;//1+4
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.SLANT_7)//3+4
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_6;//2+4
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.SLANT_8)//4+4
                    {
                        kdata.nShape = (int)BLOCKSHAPE.SLANT_7;//3+4
                    }



                    if (kdata.nShape == (int)BLOCKSHAPE.HSLANT_1)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.HSLANT_4;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.HSLANT_2)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.HSLANT_1;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.HSLANT_3)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.HSLANT_2;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.HSLANT_4)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.HSLANT_3;
                    }

                    if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_1)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_4;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_2)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_1;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_3)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_2;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_4)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_3;
                    }

                    if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_1_B)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_4_B;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_2_B)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_1_B;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_3_B)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_2_B;
                    }
                    else if (kdata.nShape == (int)BLOCKSHAPE.STAIRS_4_B)
                    {
                        kdata.nShape = (int)BLOCKSHAPE.STAIRS_3_B;
                    }




                    kList.Add(kdata);

                }
            }
        }
        kTemp.Clear();

        foreach(var v in kList)
        {
            kTemp.SetBlock(v.x, v.y, v.z, v.nBlock, v.nShape, v.nColor);
        }

        kTemp.m_bUpdated = true;
        
        

    }
    void MoveBlock(int dx, int dy, int dz)
    {

        AddUndo();
        CWAirObject kTemp = m_kAirObject;
        if (m_kCopyBuild.gameObject.activeSelf)
        {
            kTemp = m_kCopyBuild;
        }

        int sx = 0;
        int sy = 0;
        int sz = 0;
        int ex = 0;
        int ey = 0;
        int ez = 0;
        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        Vector3 vvv = m_gSelectBox.transform.position;
        vvv.x += dx;
        vvv.y += dy;
        vvv.z += dz;
        m_gSelectBox.transform.position = vvv;

        List<TDATA> kList = new List<TDATA>();
        int SELLWIDTH = kTemp.SELLWIDTH;
        for (int z =0; z < SELLWIDTH; z++)
        {
            for (int y = 0; y < SELLWIDTH; y++)
            {
                for (int x = 0; x < SELLWIDTH; x++)
                {

                    int tx = 0;
                    int ty = 0;
                    int tz = 0;
                    BlockData bs = kTemp.GetBlockData(x, y, z);
                    if (bs.nBlock == 0) continue;
                    if (bs.nBlock == (int)GITEM.CoreEngine) continue;
                    if(z>=sz&&z<ez)
                    {
                        if (y >= sy && y < ey)
                        {
                            if (x >= sx && x < ex)
                            {
                                tx = dx;
                                ty = dy;
                                tz = dz;

                            }

                        }

                    }
                    TDATA kdata = new TDATA();
                    kdata.x = x+tx;
                    kdata.y = y+ty;
                    kdata.z = z+tz;
                    kdata.nBlock = bs.nBlock;
                    kdata.nColor = bs.nColor;
                    kdata.nShape = bs.nShape;
                    if(!kList.Exists(v=>v.x==x+tx&& v.z == z + tz&& v.y == y + ty))
                    {
                        kList.Add(kdata);
                    }
                    

                }
            }
        }
        kTemp.Clear();
        foreach (var v in kList)
        {
            kTemp.SetBlock(v.x, v.y, v.z, v.nBlock, v.nShape, v.nColor);
        }

        ///kTemp.SetBlock(16,5,16, (int)GITEM.CoreEngine,0,0);
        kTemp.m_bUpdated = true;


        
        


    }

    void MoveUpBlock(bool bflag)
    {
        int SELLWIDTH = m_kAirObject.SELLWIDTH;
        Dictionary<int, BlockData> kData = m_kAirObject.GetData();
        Dictionary<int, BlockData> bTemp = new Dictionary<int, BlockData>();
        foreach (var v in kData)
        {
            bTemp.Add(v.Key, v.Value);
        }
        kData.Clear();

        int dd = 1;
        if (!bflag) dd = -1;

        foreach (var v in bTemp)
        {
            int x = m_kAirObject.GetSellX(v.Key);
            int y = m_kAirObject.GetSellY(v.Key);
            int z = m_kAirObject.GetSellZ(v.Key);
            int num1 = (z) * SELLWIDTH * SELLWIDTH + (y + dd) * SELLWIDTH + x;
            if (y + dd < 0) continue;
            kData.Add(num1, v.Value);
            if (bflag && y == 0)
            {
                int num2 = (z) * SELLWIDTH * SELLWIDTH + (y) * SELLWIDTH + x;
                kData.Add(num2, v.Value);
            }
        }
        m_kAirObject.m_bUpdated = true;
    }


    public void Submit()
    {
        
        string str = m_kInput.text;
        string[] sarry = str.Split(' ');
        if (sarry[0] == "select" || sarry[0] == "sl")
        {
            m_kSelectItem = (SHIPBLOCK)CWLib.ConvertInt(sarry[1]);

            m_kInfo.text =string.Format("선택 {0}", m_kSelectItem);
        }

        if (sarry[0] == "selectcolor" || sarry[0] == "sc")
        {
            //m_kSelectItem = (GITEM)CWLib.ConvertInt(sarry[1]);
            m_Color= (COLORNUMBER)CWLib.ConvertInt(sarry[1]);
            m_kInfo.text = string.Format("선택 칼러 {0}", m_Color);
        }

        if (sarry[0] == "selectshape" || sarry[0] == "ss")
        {
            m_Shape = (BLOCKSHAPE)CWLib.ConvertInt(sarry[1]);
            m_kInfo.text = string.Format("선택 모양 {0}", m_Shape);
        }

        if (sarry[0] == "load" || sarry[0] == "ld")
        {

            m_szName = sarry[1];
            
            LoadFile(m_szName);
            m_kInfo.text = string.Format("불러오기 {0}", m_szName);
        }

        if (sarry[0] == "Copy" || sarry[0] == "cp")
        {
            if (sarry.Length == 2)
            {
                m_szBlockCopy = sarry[1];
                Debug.Log("Export " + sarry[1]);
            }
            else { m_szBlockCopy = ""; Debug.Log("Copy"); }

            ExportBlock();

            m_kInfo.text = string.Format("Copy ");

        }
        if (sarry[0] == "cx" )// 자르기
        {
            ExportBlock();
            DeleteSelectBlock();
        }
        if (sarry[0] == "Import")
        {
            if (sarry.Length > 1)
            {
                m_szBlockCopy = sarry[1];
                Debug.Log("Import " + sarry[1]);
            }
            else m_szBlockCopy = "";
            Vector3 vPos = Vector3.zero;
            if (sarry.Length > 4)
            {
                vPos.x = CWLib.ConvertInt(sarry[2]);
                vPos.y = CWLib.ConvertInt(sarry[3]);
                vPos.z = CWLib.ConvertInt(sarry[4]);
            }
            else
            {
                vPos = m_gSelect.transform.localPosition;

            }
            ImportBlock((int)vPos.x, (int)vPos.y, (int)vPos.z);


            
        }
        // 좌우 반전
        if (sarry[0] == "Paste" || sarry[0] == "pt")
        {

            AddUndo();
            if (m_kCopyBuild.gameObject.activeSelf)
            {
                m_kCopyBuild.AddBlock(m_kAirObject);
            }
            
            

            m_kCopyBuild.gameObject.SetActive(false);

        }
        if (sarry[0] == "Flip" || sarry[0] == "ff")
        {

            AddUndo();
            if(m_kCopyBuild.gameObject.activeSelf)
            {
                m_kCopyBuild.Flip();
            }
            else
            {
                m_kAirObject.Flip();
            }
            
            

        }
        if (sarry[0] == "all")
        {
            AddUndo();
            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {
                        int nItem=m_kAirObject.GetBlock(x, y, z);
                        
                        if (nItem>0)
                        {
                            GITEMDATA gdata = CWArrayManager.Instance.GetItemData(nItem);
                            if (gdata.type == "weapon") continue;
                            if (gdata.type == "Buster") continue;
                            
                                int n = CWLib.ConvertInt(sarry[1]);
                                int s = m_kAirObject.GetShape(x, y, z);
                                int c = m_kAirObject.GetColor(x, y, z);
                                m_kAirObject.SetBlock(x, y, z, n, s, c);

                            

                        }

                    }
                }
            }

            m_kAirObject.m_bUpdated = true;

            m_kInfo.text = string.Format("전부 바꿈 {0}", CWLib.ConvertInt(sarry[1]));

        }
        if (sarry[0] == "change" || sarry[0] == "cg")
        {
            AddUndo();
            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);



            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {
                        if(m_kAirObject.GetBlock(x,y,z)== CWLib.ConvertInt(sarry[1]))
                        {

                            int n = CWLib.ConvertInt(sarry[2]);
                            int s = m_kAirObject.GetShape(x, y, z);
                            int c = m_kAirObject.GetColor(x, y, z);
                            m_kAirObject.SetBlock(x, y, z, n, s, c);

                        }

                    }
                }
            }

            m_kInfo.text = string.Format("바꿈 {0}", CWLib.ConvertInt(sarry[1]));

            m_kAirObject.m_bUpdated = true;

        }
        if (sarry[0] == "color" || sarry[0] == "cc")
        {
            AddUndo();
            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);



            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {

                        
                        if (m_kAirObject.GetColor(x, y, z) == CWLib.ConvertInt(sarry[1]))
                        {
                            int n=m_kAirObject.GetBlock(x, y, z);
                            int s=m_kAirObject.GetShape(x, y, z);
                            m_kAirObject.SetBlock(x, y, z,n,s, CWLib.ConvertInt(sarry[2]));
                        }

                    }
                }
            }

            m_kInfo.text = string.Format("칼러 바꿈  {0} {1}", CWLib.ConvertInt(sarry[1]), CWLib.ConvertInt(sarry[2]));

            m_kAirObject.m_bUpdated = true;

        }
        if (sarry[0] == "allc" || sarry[0] == "ac")
        {
            AddUndo();
            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);



            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {
                        int n = m_kAirObject.GetBlock(x, y, z);
                        if (n>0)
                        {
                            int s = m_kAirObject.GetShape(x, y, z);
                            m_kAirObject.SetBlock(x, y, z, n, s, CWLib.ConvertInt(sarry[1]));
                        }

                    }
                }
            }

            m_kInfo.text = string.Format("칼러 바꿈  {0} ", CWLib.ConvertInt(sarry[1]));

            m_kAirObject.m_bUpdated = true;

        }

        //3개 파라메터와 같은 블록을 모두 바꿈 
        //파라메터 2개가 정확해야 바꿈
        // 
        if (sarry[0] == "cg2" )
        {
            
            int n1 = CWLib.ConvertInt(sarry[1]);
            int n2 = CWLib.ConvertInt(sarry[2]);

            int n3 = CWLib.ConvertInt(sarry[3]);
            int n4 = CWLib.ConvertInt(sarry[4]);
            AddUndo();


            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {

                        BlockData bb=m_kAirObject.GetBlockData(x, y, z);
                        if(bb.nBlock==n1&&bb.nColor==n2)
                        {
                            int s = m_kAirObject.GetShape(x, y, z);
                            m_kAirObject.SetBlock(x, y, z, n3, s, n4);
                        }
                    }
                }
            }
            m_kAirObject.m_bUpdated = true;

        }
        if (sarry[0] == "cg3")
        {

            int n1 = CWLib.ConvertInt(sarry[1]); // 블록,
            int n2 = CWLib.ConvertInt(sarry[2]); // 칼러
            int n3 = CWLib.ConvertInt(sarry[3]); // 모양

            int n4 = CWLib.ConvertInt(sarry[4]);//블록,
            int n5 = CWLib.ConvertInt(sarry[5]); // 칼러
            int n6 = CWLib.ConvertInt(sarry[6]); // 모양


            AddUndo();
            int sx = 0;
            int sy = 0;
            int sz = 0;
            int ex = 0;
            int ey = 0;
            int ez = 0;
            GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

            for (int z = sz; z < ez; z++)
            {
                for (int y = sy; y < ey; y++)
                {
                    for (int x = sx; x < ex; x++)
                    {

                        BlockData bb = m_kAirObject.GetBlockData(x, y, z);

                        if (bb.nBlock == n1 && bb.nColor == n2 && bb.nShape == n3)
                        {
                            
                            m_kAirObject.SetBlock(x, y, z, n4,  n6,n5);
                        }
                    }
                }
            }
            m_kAirObject.m_bUpdated = true;

        }
        // 블록들을 새롭게 맞춘다
        if (sarry[0] == "FixBlockSystem")
        {
            FixBlockSystem();

        }

        if (sarry[0] == "Convert" )
        {
            ConvertFile();
        }
        if (sarry[0] == "Rotate"|| sarry[0] == "rr")
        {
            RotateBlock(90);
        }
        if (sarry[0] == "x+")
        {
            AddUndo();
            MoveBlock(1, 0, 0);
        }
        if (sarry[0] == "x-")
        {
            AddUndo();
            MoveBlock(-1, 0, 0);
        }
        if (sarry[0] == "y-")
        {
            AddUndo();
            MoveBlock(0, -1, 0);
        }
        if (sarry[0] == "y+")
        {
            AddUndo();
            MoveBlock(0, +1, 0);
        }
        if (sarry[0] == "z-")
        {
            AddUndo();
            MoveBlock(0, 0, -1);
        }
        if (sarry[0] == "z+")
        {
            AddUndo();
            MoveBlock(0, 0, 1);
        }
        if (sarry[0] == "x++")
        {
            AddUndo();
            MoveBlock(10, 0, 0);
        }
        if (sarry[0] == "x--")
        {
            AddUndo();
            MoveBlock(-10, 0, 0);
        }
        if (sarry[0] == "y--")
        {
            AddUndo();
            MoveBlock(0, -10, 0);
        }
        if (sarry[0] == "y++")
        {
            AddUndo();
            MoveBlock(0, +10, 0);
        }
        if (sarry[0] == "z--")
        {
            AddUndo();
            MoveBlock(0, 0, -10);
        }
        if (sarry[0] == "z++")
        {
            AddUndo();
            MoveBlock(0, 0, 10);
        }
        if (sarry[0] == "yy+")
        {
            AddUndo();
            MoveUpBlock(true);
        }
        if (sarry[0] == "yy-")
        {
            AddUndo();
            MoveUpBlock(false);
        }



    }


    public void Capture()
    {

        CWLib.SetGameObjectLayer(m_kAirObject.gameObject,12);
        string szpath = string.Format("{0}/Resources/Texture/{1}.png", Application.dataPath, m_szName);

        CWCapture.MakeFile(m_kCaptureCam, szpath);
        // CWLib.MakeImage( m_kCaptureCam, szpath);

    }
    IEnumerator RunFuction()
    {
        TextAsset[] txts = Resources.LoadAll<TextAsset>("AirCraft");
        foreach (var v in txts)
        {
            
            m_szName = v.name;
            m_kAirObject.Load(m_szName);
            yield return new WaitForEndOfFrame();
            CWLib.SetGameObjectLayer(m_kAirObject.gameObject, 12);
            string szpath = string.Format("{0}/Resources/Texture/{1}.png", Application.dataPath, m_szName);
            CWLib.MakeImage( m_kCaptureCam, szpath, new Color(0, 0, 0, 0));


        }
        yield return null;

    }
    public void MakeAllIcon()
    {
        StartCoroutine("RunFuction");

    }

    #region 셀렉트박스 크기 
    public GameObject m_gSelectBox;

    void GetSelectBox(ref int sx, ref int sy, ref int sz, ref int ex, ref int ey, ref int ez)
    {
        int SELLWIDTH = m_kAirObject.SELLWIDTH;
        

        if (m_gSelectBox.activeSelf == false)
        {
            sx = 0;
            sy = 0;
            sz = 0;
            ex = SELLWIDTH;
            ey = SELLWIDTH;
            ez = SELLWIDTH;
            return;
        }

        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        sx = (int)(vPos.x - vScale.x / 2) + SELLWIDTH/2;
        sy = (int)(vPos.y - vScale.y / 2);
        sz = (int)(vPos.z - vScale.z / 2) + SELLWIDTH / 2;

        ex = (int)(vPos.x + vScale.x / 2) + SELLWIDTH / 2;
        ey = (int)(vPos.y + vScale.y / 2);
        ez = (int)(vPos.z + vScale.z / 2) + SELLWIDTH / 2;



    }
    void DeleteSelectBlock()
    {
        AddUndo();
        int sx = 0;
        int sy = 0;
        int sz = 0;

        int ex = 0;
        int ey = 0;
        int ez = 0;

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);

        for (int z = sz; z < ez; z++)
        {
            for (int y = sy; y < ey; y++)
            {
                for (int x = sx; x < ex; x++)
                {
                    m_kAirObject.UpdateBlock(x, y, z, 0);
                }
            }
        }

    }

    public bool ExportBlock()
    {
        if (m_gSelectBox.activeSelf == false) return false;
        Vector3 vPos = m_gSelectBox.transform.position;
        Vector3 vScale = m_gSelectBox.transform.localScale;

        int sx = (int)(vPos.x - vScale.x / 2);
        int sy = (int)(vPos.y - vScale.y / 2);
        int sz = (int)(vPos.z - vScale.z / 2);

        int ex = (int)(vPos.x + vScale.x / 2);
        int ey = (int)(vPos.y + vScale.y / 2);
        int ez = (int)(vPos.z + vScale.z / 2);

        GetSelectBox(ref sx, ref sy, ref sz, ref ex, ref ey, ref ez);
        
        m_kCopyData.TakeMap(sx, sy, sz, ex, ey, ez, m_kAirObject);
        if(CWLib.IsString(m_szBlockCopy))
        {
            m_kCopyData.Save(m_szBlockCopy);
        }
        m_kCopyBuild.gameObject.SetActive(true);

        m_kCopyData.ApplyMap(0, 0, 0, SetCopyBlock);
        


        return true;
    }
    void SetCopyBlock(int x,int y,int z, int nBlock,int nShape,int nColor)
    {
        m_kCopyBuild.AddBlock(x, y, z, nBlock, nShape, nColor);
    }
    
    public void ImportBlock(int x, int y, int z)
    {
        
        m_kCopyBuild.gameObject.SetActive(true);
        if(CWLib.IsString(m_szBlockCopy))
        {
            m_kCopyBuild.Create(0);
            m_kCopyData.Load(m_szBlockCopy);
            m_kCopyData.ApplyMap(x, y, z, SetCopyBlock);

        }


    }

    #endregion

    #region 메쉬 변환
    public GameObject m_gVoxelTarget;
    public Texture2D m_kVoxelImage;

    void _SetBlock(int x, int y, int z, int nColorItem)
    {
        m_kAirObject.SetBlock(x, y, z, (int)GITEM.stone,0, nColorItem);

    }
    
    public void VoxelMaker()
    {



        m_kAirObject.Close();
        m_kAirObject.Create(0);
        m_kAirObject.EmptyCreate();
        m_kAirObject.AddBlock(m_kAirObject.SELLWIDTH / 2, 5, m_kAirObject.SELLWIDTH / 2, (int)GITEM.CoreEngine, (int)m_Shape, (int)m_Color);

        Dictionary<int, BlockData> kdata = m_kAirObject.GetData();
        m_kInfo2.text = string.Format("블록개수 : {0} HP :{1}", kdata.Count, m_kAirObject.GetHP());



        MakeVoxel kMake = new MakeVoxel();
        kMake.Make(true, 32, m_gVoxelTarget, m_kVoxelImage, _SetBlock);
        CWLib.SetGameObjectLayer(m_kAirObject.m_gCenterObject, LayerMask.NameToLayer("Bound"));

    }
    #endregion
}
