using UnityEngine; 
using System.Collections; 
using System.Runtime.InteropServices;
namespace CWEnum 
{
    // 운영툴 day log
    public enum DAYLOG {UserCount, UsertotalCount,StoryDie,MultiDie,PVPDie,  FirstStory1, FirstStory2, FirstStory3, FirstStory4, FirstStory5, TutoSkip, Storycount1, Storycount2, Storycount3, Storycount4, Storycount5, Storycount6, PVPCount, MyroomCount, MultiCount };

    public enum DETECTTYPE {ENTER,STAY,EXIT };

    // 테이블 순으로 
    public enum DAYMTYPE { PLANET=1, BLOCK, PVP, MULTI, GOLD,LIKE,TICKET,WEAPONUP };

    public enum Emoticon { EmojiTearyEyes=1 , EmojiSmile , EmojiSilly, EmojiSick , EmojiShocked , EmojiSad, EmojiQueasy , EmojiPuke
            , EmojiPoop, EmojiNervous, EmojiLaughSweatdrop, EmojiLaughCry, EmojiKiss, EmojiHeart, EmojiHappy, EmojiDrool, EmojiDisappointed
            , EmojiDerp, EmojiCute, EmojiCry, EmojiClenchTeeth, EmojiAngry, EmojiCool
    };
    public enum TALKTYPE { NORMAL, HAPPY,HERE,SADNESS, ENEMY };
    public enum UITYPE { NONE, MAIN, EDIT,STORE,MAP,RANKING };
    // 상점 타입
    public enum PLANETTYPE { Empty, Sume, Mine, Ta,Lock }; // 빈행성,내가 일부 점령,내 행성,남의 행성,닫힌행성
    public enum GalxyType { MYSOLA, MULTI, USERSOLA }; // 내별,멀티항성,유저항성
    public enum GameLayer {UI=5, Dummy =8, Shooting =9, Detect , BlockMap , Hero, minimap, Edit, Station };
    public enum COINTYPE {GOLD=1,GEM=2,AD=3,CASH=4 };

    public enum RANKUI {RANK,PRERANK,RANKDEL,NOWPOINT};// 현재 랭킹타입,NOWPOINT 지금 획득한 포인트 

    public enum USERLISDLGTYPE {RANKIG,FRIEND,ASKFRIEND };// 친구 리스트 
         
    public enum ENCHANTTYPE {DAMAGE,ENGINE, BLOCK };
    public enum AITYPE {PASSIVE,ACTIVE };// 수동적, 능동적
                
    public enum MINIMAPTYPE {GOLDMINE,PREV,QUEST,STATION,USER,TURRET, DRONE,ENEMY };
    public enum MAPTYPE {  WORLD,SINGLE,MULTI,CHALLENG };

    public enum UDPPACKET {NOTIFY,SHOOT,DAMAGE, SHOOTPOS};
    

    public enum PATTEN {NONE=0, WIDE=1,HIGH,ISRAND,WIDEROW,WIDEMIDLE,PYRAMID,SEA,GOLD,RUBY
            ,EMERALD,DIAMOND,LEVEL_2,LEVLEL_3,RED1, RED2
            , ORANGE1, ORANGE2, ORANGE3, YELLOW1, YELLOW2, YELLOW3,
            GREEN1, GREEN2, GREEN3, BLUE1, BLUE2, BLUE3,
            INDIGO1, INDIGO2, INDIGO3, PURPLE1, PURPLE2, PURPLE3
    };
    // 추후에 없애야 됨
    public enum USERTYPE { USER, COLLECTUSER, TURRET, DRONE,BATTLEAI,FAKEUSER,FIGHTUSER };
    public enum UNITTYPE {STAGE,TURRET,WORKER,BATTERY };

    public enum CWOBJECTTYPE {TURRET,DRONE,FIGHTUSER,TURRETBUILD,AISTAGE,STAGEBUILD,USER,FAKEUSER,HERO,EVENT,MINERAL, BATTERY };


    public enum AIOBJECTTYPE {TURRET, DRONE,BOSS};
    public enum GAMETYPE { LOBBY, STAGE, WAR, EDIT };
    public enum POWERTONG {HP,ENERGY,TUBO,BOMB };
    public enum POWER {HP,ENERGY,DAMAGE,SPEED,COUNT,ENGINECOUNT,WEPONCOUNT, SPEEDCOUNT };
    public enum COIN { TICKET,GOLD,GEM,ENERGY };
    

    public enum BLOCKSHAPE
    {
        NORMAL = 0, // 일반 블럭
        PLAN,   // 평면
        X_PLAN, // 십자 평면
        HALF,  // 반토막 블럭
        QURT,  // 1/4 블럭
        QURT3,  //3/4 블럭
        WATER,
        STAIRS_1,  // 계단
        STAIRS_2,  // 계단
        STAIRS_3,  // 계단
        STAIRS_4,  // 계단
        FANCE,  // 울타리
        STICK,  // 막대기
        STICK_1,  // 막대기
        STICK_2,  // 막대기
        STICK_3,  // 막대기
        STICK_4,  // 막대기
        PLAN_1,   // 평면
        PLAN_2,   // 평면
        PLAN_3,   // 평면
        PLAN_4,   // 평면
        HALF_F,  // 반대반토막 블럭
        STAIRS_1_B,  //반대 계단
        STAIRS_2_B,  //반대  계단
        STAIRS_3_B,  //반대  계단
        STAIRS_4_B,  //반대  계단
        SLANT_1,
        SLANT_2,
        SLANT_3,
        SLANT_4,
        HSLANT_1,
        HSLANT_2,
        HSLANT_3,
        HSLANT_4,
        CUSTOM,
        _WEDGE,
        _WEDGE_2,
        _WEDGE_3,
        _WEDGE_4,
        _WEDGE_5,
        _WEDGE_6,
        _WEDGE2,
        _WEDGE2_2,
        _WEDGE2_3,
        _WEDGE2_4,
        _WEDGE2_5,
        _WEDGE2_6,
        SLANT_5,
        SLANT_6,
        SLANT_7,
        SLANT_8,

        QURT_F,  // 1/4 블럭
        QURT3_F,  //3/4 블럭

        HSLANT_5,
        HSLANT_6,
        HSLANT_7,
        HSLANT_8,




    };

    public enum COLORNUMBER {
        NONE,
        RED,
        ORANGE,
        YELLOW,
        GREEN,
        BLUE,
        INDIGO,
        PURPLE,
        WHITE,
        Black,
        SkyBlue,
        LightGreen,
        Gray,
        DarkGray,
        LightGray,
        Pink,
        DarkBlue ,
        Darkpurple ,
        Lightbluegreen,
        Darkyellowgreen ,
        Bluepurple,
        magenta ,
        Cyanblue,
        Lightgreencyan ,
        scarlet ,
        Blackgreen ,
        DarkBrown ,
        Brown ,
        amber,
        Deeppink ,
        Darkgreen,

        DarkRed ,
        LightRed ,
        DarkYellow,
        LightSkyblue,
        DarkSkyblue ,
        glassGreen ,
        Lightblue ,
        LightYellowGreen ,
        LightYellow ,

    };
    
    
  
    
   public enum   WEAPONTYPE
    {
       SHOOT =0,
       BOMB = 1
    };
    
   public enum   DRONETYPE
    {
       PASS =0,
       ENEMY = 1
    };
    
   public enum   LANGUAGETYPE
    {
       KOREA =0,
       ENGLISH =1,
       CHINA = 2
    };
    
   
    
   public enum   BLOCKTYPE
    {
       SHIPBLOCK =0,
       WEAPON =1,
       ENGINE =2,
       PICKBLOCK =3,
       BOOSTER =4,
       USE =5,
       BOMBBLOCK =6,
       MATARIAL = 7
    };
    


   public enum   GITEM
    {
        // ////////////////
        NONE = 0,
        tree = 1,
        CoreEngine =   2,
        bricks=       3,
        stone1=       4,
        plastic=      5,
        wood=         6,
        brick=        7,
        concrete=     8,
        asphalt=      9,
        EmptyBlock=   35,
        sapphire=     36,
        GoldBlock=         37,
        Gold = 68,
        Ruby =         38,
        Emerald=      39,
        Diamond=      40,
        Red=          41,
        Blue=         42,
        Orange=       43,
        Yellow=       44,
        Green=        45,
        indigo=       46,
        Purple=       47,
        white=        48,
        Black=        49,
        Skyblue=      50,
        LightGreen=   51,
        Gray=         52,
        DarkGray=     53,
        LightGray=    54,
        Pink=         55,
        scarlet=      56,
        Lightbluegreen=83,
        DarkBlue=     95,
        Darkyellowgreen=84,
        Bluepurple=   85,
        magenta=      86,
        Cyanblue=     87,
        Lightgreencyan=88,
        Blackgreen=   89,
        DarkBrown=    90,
        Brown=        91,
        amber=        92,
        Deeppink=     93,
        Darkgreen=    94,
        Darkpurple=   96,
        Buster=       61,
        grass=        70,
        dirt=         71,
        sand=         72,
        stone=        73,
        Amethyst=     81,
        Topaz=        82,
        DarkRed=      97,
        LightRed=     98,
        DarkYellow=   99,
        LightSkyblue= 100,
        DarkSkyblue=  101,
        glassGreen=   102,
        Lightblue=    103,
        LightYellowGreen=104,
        LightYellow=  105,
        Ticket=       106,
        Repair=       137,
        Gun=          62,
        Missile=      130,
        Laser=        131,
        solidinium=   10,
        willenyum=    16,
        gaongilium=   17,
        luciferium=   74,
        detrium=      75,
        ilbocymium=   11,
        ibranium=     18,
        Hononium=     19,
        protectium=   12,
        lonelynium=   23,
        mosleyum=     20,
        dreaminium=   24,
        zeronium=     76,
        minorium=     77,
        plutium=      25,
        memorium=     22,
        pamelium=     78,
        hubertium=    79,
        mistyum=      80,
        douglasrium=  30,
        glass=        21,
        tempglass=    15,
        Store21=      200,
        Store25=      201,
        Store24=      202,
        Store10=      203,
        Store11=      204,
        Store22=      205,
        Store1=       206,
        Store12=      207,
        Store13=      208,
        Store8=       209,
        Store26=      210,
        Store23=      211,
        Store6=       212,
        Store14=      213,
        Store15=      214,
        Store17=      215,
        charblock=    216,
        ADDel = 253, // 광고 제거
        MAX = 256,
    }

    public enum OLDBLOC
    {
        None,
        grass = 1,
        dirt = 2,
        sand = 3,
        stone = 4,
        concrete = 236,
        Red = 212,
        LightGray = 213,
        Purple = 214,
        DarkBlue = 215,
        Blue = 216,
        Yellow = 217,
        WHITE = 218,
        Black = 219,
        Green = 220,
        Orange = 221,
        SkyBlue = 222,
        LightGreen = 223,
        Gray = 224,
        DarkGray = 225,
        GoldBlock = 55,
        asphalt = 56,
        tree3 = 202,
        tree6 = 203,
        tree = 52,
        tree1 = 53,
        tree2 = 54,
        Copper_1 = 57,
        limestone = 58,
        aluminum = 59,
        Copper_2 = 60,
        Copper_3 = 61,
        Copper_4 = 62,
        Iron_1 = 63,
        crystal = 64,
        Diamond = 65,
        valyrian = 66,
        Vibranium = 67,
        tree_top = 68,
        granite = 69,
        marble = 70,
        tempglass = 71,
        Iron_2 = 72,
        Iron_3 = 73,
        Iron_4 = 74,
        brick = 75,
        stone1 = 76,
        stone2 = 77,
        Emerald = 78,
        Ruby = 79,
        glass = 100,
        stone7 = 230,
        stone8 = 231,
        Gundarium = 232,
        wood = 234,
        tree4 = 235,
        indigo = 237,
        Pink = 238,
        bricks = 239,
        ResBlock = 226,
        GemBlock = 227,
        EmptyBlock = 241,
        nikel_1 = 242,
        nikel_2 = 243,
        nikel_3 = 244,
        nikel_4 = 245,
        titanium_1 = 246,
        titanium_2 = 247,
        titanium_3 = 248,
        titanium_4 = 249,
        Amethyst = 250,
        Topaz = 251,
        Darkpurple = 80,
        Lightbluegreen = 121,
        Darkyellowgreen = 122,
        Bluepurple = 123,
        magenta = 124,
        Cyanblue = 125,
        Lightgreencyan = 127,
        scarlet = 198,
        Blackgreen = 201,
        DarkBrown = 228,
        Brown = 229,
        amber = 233,
        Deeppink = 252,
        Darkgreen = 253,
        DarkRed = 104,
        LightRed = 105,
        DarkYellow = 106,
        LightSkyblue = 107,
        DarkSkyblue = 108,
        glassGreen = 109,
        Lightblue = 110,
        LightYellowGreen = 111,
        LightYellow = 199,
        sapphire = 254,
        MAX =255,

    };

    public enum COLORBLOC
    {
        NONE = 0,

        Red = 212,
        LightGray = 213,
        Purple = 214,
        DarkBlue = 215,
        Blue = 216,
        Yellow = 217,
        WHITE = 218,
        Black = 219,
        Green = 220,
        Orange = 221,
        SkyBlue = 222,
        LightGreen = 223,
        Gray = 224,
        DarkGray = 225,
        indigo = 237,
        Pink = 238,


        Darkpurple = 80,
        Lightbluegreen = 121,
        Darkyellowgreen = 122,
        Bluepurple = 123,
        magenta = 124,
        Cyanblue = 125,
        Lightgreencyan = 127,
        scarlet = 198,
        Blackgreen = 201,
        DarkBrown = 228,
        Brown = 229,
        amber = 233,
        Deeppink = 252,
        Darkgreen = 253,

        DarkRed = 104,
        LightRed = 105,
        DarkYellow = 106,
        LightSkyblue = 107,
        DarkSkyblue = 108,
        glassGreen = 109,
        Lightblue = 110,
        LightYellowGreen = 111,
        LightYellow = 199,





    };
    public enum RESBLOCK
    {
        gold=OLDBLOC.GoldBlock,
        Ruby=OLDBLOC.Ruby,
        Emerald=OLDBLOC.Emerald,
        Diamond=OLDBLOC.Diamond,
    };

    public enum SHIPBLOCK
    {
        NONE=0,
        tree = 1,
        stone1 = 4,
        solidinium = 10,
        willenyum = 11,
        gaongilium = 12,
        luciferium = 13,
        detrium = 14,
        ilbocymium = 30,
        ibranium = 16,
        Hononium = 17,
        protectium = 18,
        lonelynium = 19,
        mosleyum = 20,
        dreaminium = 31,
        zeronium = 22,
        minorium = 23,
        plutium = 24,
        memorium = 25,
        pamelium = 26,
        hubertium = 27,
        mistyum = 28,
        douglasrium = 29,
        glass = 15,
        tempglass = 21,
        Gun = 62,
        Missile = 130,
        Laser = 131,
        Buster = 61,
        charblock = 216,
    }
};
