using System;
using UnityEngine; 
using System.Collections; 
using System.Runtime.InteropServices;
using CWEnum;
namespace CWStruct 
{
    
    [StructLayout(LayoutKind.Explicit)]
    public struct TVECTER
    {
        [FieldOffset(0)]
        public int num;

        [FieldOffset(0)]
        public byte x;

        [FieldOffset(1)]
        public byte y;

        [FieldOffset(2)]
        public byte z;

    }
    
    public struct VERTEXBLOCK
    {
        private byte nBlock;
        public Bit m_kFace;

        public byte m_nFace4_0
        {
            get
            {
                return m_kFace.Get(0);
            }
            set
            {
                m_kFace.Set(0, value);
            }
        }
        public byte m_nFace4_1
        {
            get
            {
                return m_kFace.Get(1);
            }
            set
            {
                m_kFace.Set(1, value);
            }
        }
        public byte m_nFace4_2
        {
            get
            {
                return m_kFace.Get(2);
            }
            set
            {
                m_kFace.Set(2, value);
            }
        }
        public byte m_nFace4_3
        {
            get
            {
                return m_kFace.Get(3);
            }
            set
            {
                m_kFace.Set(3, value);
            }
        }
        public byte m_nFace0
        {
            get
            {
                return m_kFace.Get(4);
            }
            set
            {
                m_kFace.Set(4, value);
            }
        }
        public byte m_nFace1
        {
            get
            {
                return m_kFace.Get(5);
            }
            set
            {
                m_kFace.Set(5, value);
            }
        }
        public byte m_nFace2
        {
            get
            {
                return m_kFace.Get(6);
            }
            set
            {
                m_kFace.Set(6, value);
            }
        }
        public byte m_nFace3
        {
            get
            {
                return m_kFace.Get(7);
            }
            set
            {
                m_kFace.Set(7, value);
            }
        }

        public byte NBlock
        {
            get
            {
                return nBlock;
            }
            set
            {
                
                nBlock = value;
                
                m_kFace.m_bValues = 0;
            }
        }

        public bool IsLight()
        {
            if (NBlock == 0) return false;
            if (m_kFace.m_bValues > 0) return true;
            return false;
        }
        public bool IsEqual(VERTEXBLOCK v)
        {
            if (NBlock != v.NBlock) return false;
            if (m_kFace.m_bValues != v.m_kFace.m_bValues) return false;

            return true;
        }

    }
    
    public struct BlockData
    {

        public ushort nBlock;
        public byte ItemID
        {
            get
            {
                return (byte)(nBlock % 256);
            }
            set
            {
                nBlock = (ushort)((Level) * 256 + value);
            }
        }
        public byte Level
        {
            get
            {
                return (byte)(nBlock / 256);
            }
            set
            {
                nBlock = (ushort)(value * 256 + ItemID);
            }

        }

        public byte nShape;
        public byte nColor;

    };
    
    public struct MAPBLOCK
    {
        public int x, y, z;
        public int nblock;
        
        public MAPBLOCK(int tx,int ty,int tz,int n)
        {
            x = tx;y = ty;z = tz;
            nblock = n;
        
        }
    }
    public class SLOTDATA
    {
        public int m_nSlot;
        public int m_HP;
        public int m_nBlockCount;
        
    }
    public class PLANETDATA
    {
        public int m_nID;
        public Vector3 m_vPos;
    }
    public class SOLADATA
    {
        public int m_nSunNumber;
        public Vector3 m_vSun;
        public int m_nLayer;
        public int m_rValues;// 태양 랜덤값
        public int m_nCount;// 행성 수 
        public PLANETDATA[] m_kPlanet;
        public CWSolaSystem m_kSola;
    }


    public struct Bit
    {
        
        public byte m_bValues;
        // 0 ,2 ,4 8 , 16 , 32 , 64 ,128 
        public byte Get(int pos)
        {
            byte bb = (byte)(m_bValues & CWGlobal.BITArray[pos]);
            return (byte)(bb >> pos);
        }
        public void Set(int pos, int values)
        {
            byte bb = (byte)(values << pos);
            m_bValues = (byte)(m_bValues | bb);

        }


    }

    public class UNITDATA
    {
        public int m_nID;
        public int m_nTableNumber;
        public int m_nStageID; // 소속 정거장
        public int m_nUnitType; // 유닛 타입
        public string m_szFilename;// 파일 이름
        public int m_nRoomNumber;
        public int x, y, z;
        public int m_nLevel; // 레벨
        public int m_nState;//0구입안함,1 설치안함 2 작동중 3 파괴상태
        public float m_fHp; // 현재 HP 상태
        

    }

    public class UNITCLASS
    {
        public bool m_bUpdated;
        public UNITTYPE m_UnitType;// 유닛  타입 
        public string m_szName;
        public string m_szFileName;
        public string m_szImage;
        //public int m_nBase;
        public int m_nCointype;
        public int m_nPrice;
        public int m_nHP;
        int _nLevel = 0;
        public int m_nLevel
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
        int _nTotalcount;
        public int m_nTotalcount
        {
            get
            {
                return _nTotalcount;
            }
            set
            {
                _nTotalcount = value;

            }
        }

        int _nBlockcount;
        public int m_nBlockcount
        {
            get
            {
                return _nBlockcount;
            }
            set
            {
                _nBlockcount = value;
            }
        }
    }

    public class QUESTDATA
    {
        public int m_nID;
        public string m_szTitle;
        public string m_szHelp;
        public string m_szIcon;
        public int m_nMaxCount;
        public int m_nCount;
        public int m_nRewardCount;
        public int m_nRewardEnd;//  보상을 받았다
        public string m_szGoogleID;

    }

    public class STATIONINFO
    {
        public Vector3 vPos;
        public int nLevel;
    }
    public class ResultBattleData
    {
        public bool m_bMySlot;
        public int nSlot;
        public int nTeam;
        public string szname;// 이름
        public string szFace;// 등록된 이름
        public int bcnt;// 획득 블록개수
        public int point;// 포인트
        public int killcount;
    }

    public class SLOTITEM
    {
        public int m_nSlot;
        int _nItem ;
        int _nCount;

        int _nCheckItem ;
        int _nCheckCount;

        public bool m_bUpdated=false;
        public int  m_nNewItem = 0;// 새로 얻은 아이템, 클릭해야 풀린다 
        public bool m_bCheck;//선택됨
        public void SetItem(int nSlot, int nItem,int nCount)
        {
            
            m_nSlot = nSlot;
            NItem = nItem;
            NCount = nCount;
            m_bUpdated = false;
        }
        public int NItem
        {
            get
            {
                return (int)_nItem;
            }
            set
            {
                if(_nItem!=value) m_bUpdated = true;
                _nItem = value;
                

            }
        }

        public int NCount
        {
            get
            {
                return (int) _nCount;
            }

            set
            {
                if (_nCount != value) m_bUpdated = true;
                _nCount = value;
                _nCheckCount = value;
                if (_nCount == 0) NItem = 0;
                

            }
        }
        public int NShop = 0;
    };


    [System.Serializable]
    public struct GITEMDATA
    {
        public int nKey;
        public int nID;
        public int level;
        public string szname;
        public string szfilename;
        public int nblock;
        public string type;
        public int InvenType;
        public int price;
        public int pricesell;
        public int cash;
        public int hp;
        public string sziconname;
        public string szGroup;
        public string m_szTitle;
        public string szInfo;
        public int BombCount;
        public int Damage;// 추가 데미지 
        public int subtype;
    };
    public struct BUSTER
    {
        public int nID;
        public int Level;
        public float Speed;
        public float P_Speed;// 행성이동
        public float G_Speed;// 항성이동
        public float S_Speed;// 순간속도
        public float H_Ctrl;// 높이조절
        public float UseEnergy;//연료소모 
        public float Chargetime;// 충전시간
        public float Weight; // 무게 

        public float SpeedRate//0~1으로 리턴
        {
            get {return Speed*0.5f; }
        }
        public float P_SpeedRate//0~1으로 리턴
        {
            get { return P_Speed * 0.5f; }
        }
        public float G_SpeedRate//0~1으로 리턴
        {
            get { return G_Speed * 0.5f; }
        }
        public float S_SpeedRate//0~1으로 리턴
        {
            get { return S_Speed * 0.5f; }
        }
        public float H_CtrlRate//0~1으로 리턴
        {
            get { return H_Ctrl * 0.5f; }
        }
        public float UseEnergyRate//0~1으로 리턴
        {
            get { return UseEnergy * 0.5f; }
        }
        public float ChargetimeRate//0~1으로 리턴
        {
            get { return Chargetime * 0.5f; }
        }
        public float WeightRate//0~1으로 리턴
        {
            get { return Weight * 0.5f; }
        }


    }

    public struct WEAPON
    {
        public int nID;
        public int nType;// 1:기관총,2 : 미사일 ,3: 레이저
        public int Damage;
        public int Level;
        public string szmissile;

    }
    [System.Serializable]
    public struct BLOCKINFO
    {
        public int nID;
        public int x;
        public int y;

        public int white_x; // 흰색
        public int white_y;

        public int side_x;
        public int side_y;

        public string sidename;
        public BLOCKSHAPE nShape;
        public string name;
        public string szItem;
        public int nItemID;
        public int HP;
        public string szType;// grass, dirt,sand,stone
        public string szPatten;// 분류 



    };
    public class TURRETGROUP
    {
        public string szname;
        public int nLevel;
        public int nID;
        public int Hp;
        public int Damage;
        public int Cooltime;
        public int AITime;
        public int nblock;
        public int nResentime;
        public int m_fLimitYaw;
        public string m_szMissile;
    };

    public delegate void dlSetblock(int x, int y, int z, int nBlock);

    public struct WEAPONSLOT
    {
        public int DamageLv;
        public int SpeedLv;
        public int RangeLv;
    }

}
