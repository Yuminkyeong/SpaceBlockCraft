using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CWEnum;
using CWStruct;
using CWUnityLib;

// 칼러 선택 
// 기본 블록 선택 
public class MapCtrl : CWSingleton<MapCtrl>
{
    public MakeMapTool m_kMapTool;
    
    public enum BLOCENUM
    {
        None,
        grass = 1,
        gray_sand	=	23	,
gray_stone	=	24	,
ltgreen_grass	=	25	,
ltgreen_dirt	=	26	,
ltgreen_sand	=	27	,
ltgreen_stone	=	28	,
snow_grass	=	29	,
snow_dirt	=	30	,
snow_sand	=	31	,
snow_stone	=	32	,
mk_grass	=	33	,
mk_dirt	=	34	,
mk_sand	=	35	,
mk_stone	=	36	,
dkgreen_grass	=	37	,
dkgreen_dirt	=	38	,
dkgreen_sand	=	39	,
dkgreen_stone	=	40	,
concrete	=	236	,
Red	=	212	,
LightGray	=	213	,
Purple	=	214	,
DarkBlue	=	215	,
Blue	=	216	,
Yellow	=	217	,
WHITE	=	218	,
Black	=	219	,
Green	=	220	,
Orange	=	221	,
SkyBlue	=	222	,
LightGreen	=	223	,
Gray	=	224	,
DarkGray	=	225	,
tree3	=	202	,
tree6	=	203	,
wood1	=	204	,
wood2	=	205	,
wood3	=	206	,
wood4	=	207	,
tree	=	52	,
tree1	=	53	,
tree2	=	54	,
brick	=	75	,
stone1	=	76	,
wood_7	=	85	,
tree7	=	88	,
tree5	=	98	,
wood5	=	99	,
glass	=	100	,
tree8	=	101	,
tree9	=	102	,
wood6	=	126	,
wood_9	=	130	,
wood_8	=	137	,
wood_5	=	138	,
stone7	=	230	,
stone8	=	231	,
wood	=	234	,
tree4	=	235	,
indigo	=	237	,
Pink	=	238	,
bricks	=	239	,
Darkpurple	=	80	,
Lightbluegreen	=	121	,
Darkyellowgreen	=	122	,
Bluepurple	=	123	,
magenta	=	124	,
Cyanblue	=	125	,
Lightgreencyan	=	127	,
scarlet	=	198	,
Blackgreen	=	201	,
DarkBrown	=	228	,
Brown	=	229	,
amber	=	233	,
Deeppink	=	252	,
Darkgreen	=	253	,
DarkRed	=	104	,
LightRed	=	105	,
DarkYellow	=	106	,
LightSkyblue	=	107	,
DarkSkyblue	=	108	,
glassGreen	=	109	,
Lightblue	=	110	,
LightYellowGreen	=	111	,
LightYellow	=	199	,
    };

    public bool CHANGE;
    public bool DEL;
    public bool SPHERE;//구형으로 하는가?
    public bool UpBound;// 위 블록만 칠한다 
    public COLORBLOC COLOR;
    public BLOCENUM BLOCK;

    public MeshFilter m_kMeshFilter;

    int _nFile=1;
    public int m_nFile
    {
        get
        {
            return _nFile;
        }
        set
        {
            _nFile = value;
        }
    }

    int _nRange;
    public int m_nRange
    {
        get
        {
            return _nRange;
        }
        set
        {
            _nRange = value;
        }
    }

    private void Start()
    {
        
    }
    public OLDBLOC GetBlock()
    {

        OLDBLOC Ret = OLDBLOC.None;
        if (DEL)
        {
            return OLDBLOC.None;
        }
        else
        {
            Ret = OLDBLOC.None;
            if (COLOR != COLORBLOC.NONE)
            {
                Ret = (OLDBLOC)COLOR;
            }
            if (BLOCK != BLOCENUM.None)
            {
                Ret = (OLDBLOC)BLOCK;
            }

        }

        return Ret;

    }
 
    public void Test()
    {

        m_kMeshFilter.sharedMesh = CWResourceManager.Instance.GetMeshAsset("1");

        
    }

    public void CreateObject()
    {
        m_kMapTool.CreateObject();
    }
    public void LoadMap()
    {

        m_kMapTool.LoadData();

        MapMake.instance.PatternID = m_nFile;
        MapMake.instance.GetPatternData();


    }

}
