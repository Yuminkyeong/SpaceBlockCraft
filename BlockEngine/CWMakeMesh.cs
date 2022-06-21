using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using CWUnityLib;
using UnityEngine.Rendering;
using CWEnum;
using CWStruct;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Diagnostics;
#if UNITY_EDITOR
using UnityEditor;
#endif
public class CWMakeMesh  
{
    const float FLight = 0.45f;
    const float FRate = 1 / 32f;
    const float GRIDE = 32f;

    BLOCKINFO[] m_kBlock;

    private static int g_MeshCount = 0;

    public delegate int dgGetBlock(int x, int y, int z);
    public delegate Color dgGetColor(int x, int y, int z);

    #region 버텍스 배열
    // 버텍스 방향 순
    Vector2[]g_LightVertex =
    {
       new Vector2(0,1),new Vector2(1,3),new Vector2(2,0),new Vector2(3,2)
    };
    Vector3[,] g_XPlanVarray =
    {
        {
            new Vector3(1,1,0),
            new Vector3(1,0,0),
            new Vector3(0,1,1),
            new Vector3(0,0,1),

        },
        {
            new Vector3(1,1,1),
            new Vector3(1,0,1),
            new Vector3(0,1,0),
            new Vector3(0,0,0),

        },

    };
    Vector3[,,] g_Varray_Wedge = 
 {

//Wedge  
{
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(1,1,1),
new Vector3(1,0,1),
new Vector3(0,1,1),
new Vector3(0,0,1),
},
{
new Vector3(1,1,1),
new Vector3(1,0,1),
new Vector3(1,0,0),
new Vector3(1,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,1,0),
new Vector3(1,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
},
{
new Vector3(1,0,1),
new Vector3(1,0,0),
new Vector3(0,0,1),
new Vector3(0,0,0),
},
},
//Wedge_2  
{
{
new Vector3(1,1,0),
new Vector3(1,0,1),
new Vector3(0,1,0),
new Vector3(0,0,1),
},
{
new Vector3(1,1,0),
new Vector3(1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,1,0),
new Vector3(1,0,1),
new Vector3(1,1,0),
new Vector3(1,0,0),
},
{
new Vector3(1,0,1),
new Vector3(1,0,0),
new Vector3(0,0,1),
new Vector3(0,0,0),
},
{
    new Vector3(1,1,0),
    new Vector3(1,1,0),
    new Vector3(0,1,0),
    new Vector3(0,1,0),
},

},
//Wedge_3  
{
{
new Vector3(0,1,0),
new Vector3(0,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(-1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(-1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
},
{
new Vector3(-1,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
},
//Wedge_4  
{
{
new Vector3(0,1,0),
new Vector3(0,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,-1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,-1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
},
{
new Vector3(0,0,-1),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
},
//Wedge_5  
{
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,1),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(1,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,0,1),
new Vector3(0,0,0),
},
},
//Wedge_6  
{
{
new Vector3(-1,-1,0),
new Vector3(-1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(-1,-1,0),
new Vector3(0,0,1),
new Vector3(0,-1,0),
new Vector3(0,0,1),
},
{
new Vector3(-1,-1,0),
new Vector3(0,0,1),
new Vector3(-1,-1,0),
new Vector3(-1,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,1),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(-1,-1,0),
new Vector3(-1,-1,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
},
{
new Vector3(0,0,1),
new Vector3(-1,0,0),
new Vector3(0,0,1),
new Vector3(0,0,0),
},
},
//Wedge2  
{
{
new Vector3(0,-1,0),
new Vector3(-1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(-1,0,1),
new Vector3(0,-1,0),
new Vector3(0,0,1),
},
{
new Vector3(0,-1,0),
new Vector3(-1,0,1),
new Vector3(0,-1,0),
new Vector3(-1,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,1),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
},
{
new Vector3(-1,0,1),
new Vector3(-1,0,0),
new Vector3(0,0,1),
new Vector3(0,0,0),
},
},
//Wedge2_2  
{
{
new Vector3(0,-1,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,-1),
},
{
new Vector3(0,-1,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(1,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,-1),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,0,-1),
new Vector3(0,0,0),
},
},
//Wedge2_3  
{
{
new Vector3(0,-1,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,-1),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,-1),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,0,-1),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
},
//Wedge2_4  
{
{
new Vector3(0,-1,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(1,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
{
new Vector3(1,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
new Vector3(0,0,0),
},
},
//Wedge2_5  
{
{
new Vector3(-1,1,0),
new Vector3(-1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(-1,1,0),
new Vector3(-1,0,0),
new Vector3(0,1,0),
new Vector3(0,0,-1),
},
{
new Vector3(-1,1,0),
new Vector3(-1,0,0),
new Vector3(-1,1,0),
new Vector3(-1,0,0),
},
{
new Vector3(0,1,0),
new Vector3(0,0,-1),
new Vector3(0,1,0),
new Vector3(0,0,0),
},
{
new Vector3(-1,1,0),
new Vector3(-1,1,0),
new Vector3(0,1,0),
new Vector3(0,1,0),
},
{
new Vector3(-1,0,0),
new Vector3(-1,0,0),
new Vector3(0,0,-1),
new Vector3(0,0,0),
},
},
//Wedge2_6  
{
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,0,0),
new Vector3(0,0,-1),
new Vector3(0,-1,0),
new Vector3(0,0,-1),
},
{
new Vector3(1,0,0),
new Vector3(0,0,-1),
new Vector3(1,0,0),
new Vector3(1,0,0),
},
{
new Vector3(0,-1,0),
new Vector3(0,0,-1),
new Vector3(0,-1,0),
new Vector3(0,0,0),
},
{
new Vector3(1,0,0),
new Vector3(1,0,0),
new Vector3(0,-1,0),
new Vector3(0,-1,0),
},
{
new Vector3(0,0,-1),
new Vector3(1,0,0),
new Vector3(0,0,-1),
new Vector3(0,0,0),
},
},

 };
    Vector3 [,,]g_Varray_Slant=
{

//-0분기
///////////////////////////////////////////////////////////////////////////
	{
		{  // -Z 평면
			new Vector3(1 ,1, 1),//new Vector3(1, 1, 0),
			new Vector3(1, 0, 0),
            new Vector3(0, 1 ,1),//new Vector3(0, 1, 0),
            new Vector3(0, 0, 0),


		},
		{  // +Z 평면
			new Vector3(1, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(0, 1, 1),
			new Vector3(0, 0, 1),

		},
		{	//+X 평면
			new Vector3(1, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(1 ,1, 1),
			new Vector3(1, 0, 0),

		},
		{	//-X 평면 
			new Vector3(0, 1, 1),
			new Vector3(0, 0, 1),
			new Vector3(0, 1 ,1),
			new Vector3(0, 0, 0),

		},
		{  //+ Y 평면
			new Vector3(1, 1, 1),	
			new Vector3(1 ,1, 1),	
			new Vector3(0, 1, 1),	
			new Vector3(0, 1 ,1),	

		},
		{  //- Y 평면
			new Vector3(1, 0, 1),	
			new Vector3(1, 0, 0),	
			new Vector3(0, 0, 1),	
			new Vector3(0, 0, 0),	
		},

	},
//-1분기 new Vector3(1, 1, 0} -> 0,1,0   new Vector3(1 ,1, 1}-> 0,1,1
///////////////////////////////////////////////////////////////////////////
	{
		{  // -Z 평면
			new Vector3(0, 1, 0),//new Vector3(1, 1, 0}
			new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 0, 0),


		},
		{  // +Z 평면
			new Vector3(0, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(0, 1, 1),
			new Vector3(0, 0, 1),

		},
		{	//+X 평면
			new Vector3(0, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(0, 1, 0),
			new Vector3(1, 0, 0),

		},
		{	//-X 평면 
			new Vector3(0, 1, 1),
			new Vector3(0, 0, 1),
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 0),

		},
		{  //+ Y 평면
			new Vector3(0, 1, 1),	
			new Vector3(0, 1, 0),	
			new Vector3(0, 1, 1),	
			new Vector3(0, 1, 0),	

		},
		{  //- Y 평면
			new Vector3(1, 0, 1),	
			new Vector3(1, 0, 0),	
			new Vector3(0, 0, 1),	
			new Vector3(0, 0, 0),	
		},
	},

//-2분기 new Vector3(1, 1, 1} -> 1,1,0  new Vector3(0, 1, 1} -> 0,1,0
///////////////////////////////////////////////////////////////////////////
	{
		{  // -Z 평면
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 0),
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 0),


		},
		{  // +Z 평면
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 1),
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 1),

		},
		{	//+X 평면
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 1),
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 0),

		},
		{	//-X 평면 
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 1),
			new Vector3(0, 1, 0),
			new Vector3(0, 0, 0),

		},
		{  //+ Y 평면
			new Vector3(1, 1, 0),	
			new Vector3(1, 1, 0),	
			new Vector3(0, 1, 0),	
			new Vector3(0, 1, 0),	

		},
		{  //- Y 평면
			new Vector3(1, 0, 1),	
			new Vector3(1, 0, 0),	
			new Vector3(0, 0, 1),	
			new Vector3(0, 0, 0),	
		},

	},

//-3분기 new Vector3(0, 1, 1}->1,1,1  new Vector3(0, 1, 0}-> 1,1,0
///////////////////////////////////////////////////////////////////////////
	{
		{  // -Z 평면
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 0),
			new Vector3(1, 1, 0),
			new Vector3(0, 0, 0),


		},
		{  // +Z 평면
			new Vector3(1, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(1, 1, 1),//
			new Vector3(0, 0, 1),

		},
		{	//+X 평면
			new Vector3(1, 1, 1),
			new Vector3(1, 0, 1),
			new Vector3(1, 1, 0),
			new Vector3(1, 0, 0),

		},
		{	//-X 평면 
			new Vector3(1, 1, 1),
			new Vector3(0, 0, 1),
			new Vector3(1, 1, 0),
			new Vector3(0, 0, 0),

		},
		{  //+ Y 평면
			new Vector3(1, 1, 1),	
			new Vector3(1, 1, 0),	
			new Vector3(1, 1, 1),	
			new Vector3(1, 1, 0),	

		},
		{  //- Y 평면
			new Vector3(1, 0, 1),	
			new Vector3(1, 0, 0),	
			new Vector3(0, 0, 1),	
			new Vector3(0, 0, 0),	
		},

	},
/////////////////////////////////////////////////////

/////////////////////////////////////////////////////
//-0분기{ 0,2,1,3,1,2}
//    { 0,1,2,3,2,1},
///////////////////////////////////////////////////////////////////////////
	{
        {  // -Z 평면
			new Vector3(0 ,0, 0),
            new Vector3(1, 0 ,0),
			new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),


        },
        {  // +Z 평면
			new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),

        },
        {	//+X 평면
			new Vector3(0, 0, 0),
            new Vector3(0 ,0, 0),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),

        },
        {	//-X 평면 
			new Vector3(1, 0, 0),
            new Vector3(1, 0 ,0),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),

        },
        {  //+ Y 평면
			new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0 ,0, 0),
            new Vector3(1, 0 ,0),

        },
        {  //- Y 평면
			new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        },

    },
//-0분기 new Vector3(0, 0, 1} -> 1,0,1   new Vector3(0 ,0, 0}-> 1,0,0
///////////////////////////////////////////////////////////////////////////
	{
        {  // -Z 평면
			new Vector3(1, 0, 1),//new Vector3(0, 0, 1}
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),


        },
        {  // +Z 평면
			new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),

        },
        {	//+X 평면
			new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),

        },
        {	//-X 평면 
			new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),

        },
        {  //+ Y 평면
			new Vector3(1, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0, 1),
            new Vector3(1, 0, 1),

        },
        {  //- Y 평면
			new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        },
    },

//-0분기 new Vector3(0, 0, 0} -> 0,0,1  new Vector3(1, 0, 0} -> 1,0,1
///////////////////////////////////////////////////////////////////////////
	{
        {  // -Z 평면
			new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),


        },
        {  // +Z 평면
			new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),

        },
        {	//+X 평면
			new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),

        },
        {	//-X 평면 
			new Vector3(1, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),

        },
        {  //+ Y 평면
			new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1),

        },
        {  //- Y 평면
			new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        },

    },

//-3분기 new Vector3(1, 0, 0}->0,0,0  new Vector3(1, 0, 1}-> 0,0,1
///////////////////////////////////////////////////////////////////////////
	{
        {  // -Z 평면
			new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),


        },
        {  // +Z 평면
			new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),//
            new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),

        },
        {	//+X 평면
			new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 1, 0),
            new Vector3(0, 1, 1),

        },
        {	//-X 평면 
			new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(1, 1, 0),
            new Vector3(1, 1, 1),

        },
        {  //+ Y 평면
			new Vector3(0, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, 1),

        },
        {  //- Y 평면
			new Vector3(0, 1, 0),
            new Vector3(1, 1, 0),
            new Vector3(0, 1, 1),
            new Vector3(1, 1, 1),
        },

    },
/////////////////////////////////////////////////////


};
    Vector3[,] g_LightArray =
   {
    {  // -Z 평면
		new Vector3(1, 0,  -1),//x+
		new Vector3(0, -1, -1), //z-
		new Vector3(0, 1,  -1), //z+
		new Vector3(-1, 0,  -1), //x-
	},
	{  // +Z 평면
		new Vector3(1, 0,  1), //x+
		new Vector3(0, -1, 1), //z-
		new Vector3(0, 1,  1), //z+
		new Vector3(-1, 0, 1), //x-

	},
	{	//+X 평면
		new Vector3(1, 0, 1), //x+
		new Vector3(1, -1, 0),
        new Vector3(1, 1, 0),
        new Vector3(1, 0, -1),
	},
	{	//-X 평면 
		new Vector3(-1, 0, 1),
        new Vector3(-1, -1, 0),
        new Vector3(-1, 1, 0),
        new Vector3(-1, 0, -1),

	},
	{  //+ Y 평면
		new Vector3(1, 1, 0),	//X+
		new Vector3(0, 1, -1),	//z-
		new Vector3(0, 1, 1),	//z+
		new Vector3(-1, 1, 0),	//x-
	},
    {  //- Y 평면
		new Vector3(1, 0, 1),
        new Vector3(1, 0, 0),
        new Vector3(0, 0, 1),
        new Vector3(0, 0, 0),
    },

};

    Vector3[,] g_Varray=
{

	{  // -Z 평면
		new Vector3(1, 1, 0),
		new Vector3(1, 0, 0),
		new Vector3(0, 1, 0),
		new Vector3(0, 0, 0),


	},
	{  // +Z 평면
		new Vector3(1, 1, 1),
		new Vector3(1, 0, 1),
		new Vector3(0, 1, 1),
		new Vector3(0, 0, 1),

	},
	{	//+X 평면
		new Vector3(1, 1, 1),
		new Vector3(1, 0, 1),
		new Vector3(1, 1, 0),
		new Vector3(1, 0, 0),

	},
	{	//-X 평면 
		new Vector3(0, 1, 1),
		new Vector3(0, 0, 1),
		new Vector3(0, 1, 0),
		new Vector3(0, 0, 0),

	},
	{  //+ Y 평면
		new Vector3(1, 1, 1),	
		new Vector3(1, 1, 0),	
		new Vector3(0, 1, 1),	
		new Vector3(0, 1, 0),	

	},
	{  //- Y 평면
		new Vector3(1, 0, 1),	
		new Vector3(1, 0, 0),	
		new Vector3(0, 0, 1),	
		new Vector3(0, 0, 0),	
	},


};
    int [,] g_nIndexOrder = 
{

    { 0,1,2,3,2,1},
    { 0,2,1,3,1,2}

};
    Vector2 []  g_VUV=
{
    new Vector2(FRate, FRate),
    new Vector2(FRate, 0) ,
    new Vector2(0, FRate),
    new Vector2(0, 0),


};
    #endregion
    int MAXVERTEX = 655560; //10배 
    int MAXINDEX = 655560;

    // 면을 만든다 
    public Vector3 [] m_kVerts;
    public Vector2[] m_kUV;
    public Color[] m_kColor;
    public int[] m_Indexs;

    public int m_dwVertexCount;
    public int m_dwIndexCount;

    Vector3 vPos = new Vector3();
    Vector3 vTemp = new Vector3();

    public static int G_MeshCount
    {
        get
        {
            return g_MeshCount;
        }

        set
        {
            g_MeshCount = value;
          
            
        }
    }
    public void PutMesh(Mesh kMesh)
    {
        m_dwVertexCount = kMesh.vertexCount;
        m_dwIndexCount = kMesh.triangles.Length;
        Array.Copy(kMesh.vertices, m_kVerts, m_dwVertexCount);
        Array.Copy(kMesh.uv, m_kUV,  m_dwVertexCount);
        Array.Copy(kMesh.triangles, m_Indexs,  m_dwIndexCount);
        Array.Copy(kMesh.colors, m_kColor,  m_dwVertexCount);

    }

    public CWMakeMesh(Vector3[] v, Vector2[] v2, Color[] c, int[] n, int vcnt = 0, int ncnt = 0)
    {
        m_kVerts = v;
        m_kUV = v2;
        m_kColor = c;
        m_Indexs = n;
        m_dwVertexCount = vcnt;
        m_dwIndexCount = ncnt;

        if(CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.BAD)
        {
            MAXVERTEX = 0xffff;
            MAXINDEX = 0xffff;
        }
        else
        {
            MAXVERTEX = 650000;
            MAXINDEX = 650000;
        }


        m_kBlock = CWArrayManager.Instance.m_kBlock;

        
    }

    public CWMakeMesh()
    {

        if (CWGlobal.g_SystemState == CWGlobal.SYSTEMSTATE.BAD)
        {
            MAXVERTEX = 0xffff;
            MAXINDEX = 0xffff;
        }
        else
        {
            MAXVERTEX = 650000;
            MAXINDEX = 650000;
        }

    //    UnityEngine.Debug.Log(string.Format("CWMakeMesh size {0}", MAXVERTEX)); 

        m_kVerts = new Vector3[MAXVERTEX];
        m_kUV = new Vector2[MAXVERTEX];
        m_kColor = new Color[MAXVERTEX];
        m_Indexs = new int[MAXINDEX];
        m_dwVertexCount = 0;
        m_dwIndexCount = 0;
        m_kBlock = CWArrayManager.Instance.m_kBlock;

        
    }


    public void Clear()
    {
        Array.Clear(m_kVerts, 0, m_kVerts.Length);
        Array.Clear(m_kUV, 0, m_kUV.Length);
        Array.Clear(m_kColor, 0, m_kColor.Length);
        Array.Clear(m_Indexs, 0, m_Indexs.Length);

        m_dwVertexCount = 0;
        m_dwIndexCount = 0;
    }
    /*
     * 텍스쳐 개념
     * 바닥 텍스쳐는 4X4로 만든다. 
     * 바닥은 최대 4를 넘어가면 안된다
     * 일반 텍스쳐는 2X2로 제작한다. 
     * 
     * */


    void GetUV(int nFace, ref Vector2 vUV,byte nBlock, dgGetColor _Color=null,float sx=0, float sy =0, float sz =0)
    {
        // 컬러는 흰색에서 칠힌다, 
        if(_Color!=null)
        {
            int x;
            int y;
            if (nFace == 4|| nFace == 5)// 윗면
            {
                x = m_kBlock[nBlock].x;
                y = m_kBlock[nBlock].y;

            }
            else
            {
                x = m_kBlock[nBlock].side_x;
                y = m_kBlock[nBlock].side_y;
            }
            if (_Color((int)sx, (int)sy, (int)sz)!=Color.white)
            {
                y += 10;
            }
            vUV.x = x / GRIDE;
            vUV.y = y / GRIDE;
            return;
        }
        if (nFace == 4 || nFace == 5)// 윗면
        {
            int x = m_kBlock[nBlock].x;
            int y = m_kBlock[nBlock].y;
            vUV.x = x / GRIDE;
            vUV.y = y / GRIDE;
            return;
        }
        {
            int x = m_kBlock[nBlock].side_x;
            int y = m_kBlock[nBlock].side_y;
            vUV.x = x / GRIDE;
            vUV.y = y / GRIDE;

        }


    }
    // UV 개념. 
    // 특정 부위를 늘리는 개념이 없으므로
    // 텍스쳐를 카피를 미리 한 다음 UV 길이를 맞춘다. 
    // 텍스쳐의 크기가 dx보다 크거나 같다면 그대로 dx가 되며
    // 크다면 dx의 비율로 계산되서 들어가야 한다 
    // 크기 :  4 dx 8 이라면 
    // 크기는 파일크기로 정해 진다 
    // 나머지는 늘어지는 개념 



    public Vector3 m_vPos=Vector3.zero;

    Vector2 m_vUV = new Vector2();// uv start
    public void MakeFace(int nFace, float x, float  y, float  z, float  dx, float  dy,float dz, int nBlock, dgGetBlock _getblock, dgGetColor _getcolor,bool bAirmode)
    {

        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;
      //  TimeCheck.Instance.StartTime(14);
        // 삼각형 1면
        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;
        int dwVerCnt =m_dwVertexCount;
        
        
        GetUV(nFace, ref m_vUV,(byte)nBlock, _getcolor,x,y,z);
        for (int i=0;i<4;i++)
        {
            vTemp.x =  g_Varray[nFace,i].x * dx;
            vTemp.y =  g_Varray[nFace, i].y*dy;
            vTemp.z =  g_Varray[nFace, i].z * dz;
            m_kVerts[m_dwVertexCount] =  vTemp+vPos;

            //m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x*ux;
            //m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y * uy;

            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;


            if (_getcolor == null)
            {
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
            }
            else
            {
                m_kColor[m_dwVertexCount] = _getcolor((int)x, (int)y, (int)z); ;
            }

            m_dwVertexCount++;
            if (m_dwVertexCount >= MAXVERTEX - 4)
            {
                m_dwVertexCount = MAXVERTEX - 4;
            }
        }

        // 
        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
            if (m_dwIndexCount >= m_Indexs.Length)
            {
                continue;
            }

            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;

        }
   //     TimeCheck.Instance.EndTime(14);
        if (dx > 1) return;
        if (dz > 1) return;
        if (dy > 1) return;

        if (bAirmode) return;// 비행기 모드는 통과
        //if (_getcolor!=null) return;
        
    
        if (nFace == 0)
        {
            float tx, ty, tz;
            tx = vPos.x + 0;
            ty = vPos.y - 1;
            tz = vPos.z - 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

        }
        if (nFace == 1)
        {
            float tx, ty, tz;
            tx = vPos.x - 0;
            ty = vPos.y - 1;
            tz = vPos.z + 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

        }
        if (nFace == 2)
        {
            float tx, ty, tz;
            tx = vPos.x + 1;
            ty = vPos.y - 1;
            tz = vPos.z + 0;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

        }
        if (nFace == 3)
        {
            float tx, ty, tz;
            tx = vPos.x - 1;
            ty = vPos.y - 1;
            tz = vPos.z - 0;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

        }
        if (nFace==4)
        {
            float tx, ty, tz;
            tx = vPos.x + 1;
            ty = vPos.y + 1;
            tz = vPos.z + 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt].a = 1.0f;
                m_kColor[dwVerCnt].r *= FLight;
                m_kColor[dwVerCnt].g *= FLight;
                m_kColor[dwVerCnt].b *= FLight;
            }
            tx = vPos.x + 1;
            ty = vPos.y + 1;
            tz = vPos.z - 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;
            }

            tx = vPos.x - 1;
            ty = vPos.y + 1;
            tz = vPos.z + 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 2].a = 1.0f;
                m_kColor[dwVerCnt + 2].r *= FLight;
                m_kColor[dwVerCnt + 2].g *= FLight;
                m_kColor[dwVerCnt + 2].b *= FLight;
            }

            tx = vPos.x - 1;
            ty = vPos.y + 1;
            tz = vPos.z - 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;
            }

            tx = vPos.x + 0;
            ty = vPos.y + 1;
            tz = vPos.z + 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 0].a = 1.0f;
                m_kColor[dwVerCnt + 0].r *= FLight;
                m_kColor[dwVerCnt + 0].g *= FLight;
                m_kColor[dwVerCnt + 0].b *= FLight;

                m_kColor[dwVerCnt + 2].a = 1.0f;
                m_kColor[dwVerCnt + 2].r *= FLight;
                m_kColor[dwVerCnt + 2].g *= FLight;
                m_kColor[dwVerCnt + 2].b *= FLight;

            }

            tx = vPos.x + 1;
            ty = vPos.y + 1;
            tz = vPos.z + 0;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 0].a = 1.0f;
                m_kColor[dwVerCnt + 0].r *= FLight;
                m_kColor[dwVerCnt + 0].g *= FLight;
                m_kColor[dwVerCnt + 0].b *= FLight;

                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

            }

            tx = vPos.x + 0;
            ty = vPos.y + 1;
            tz = vPos.z - 1;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 1].a = 1.0f;
                m_kColor[dwVerCnt + 1].r *= FLight;
                m_kColor[dwVerCnt + 1].g *= FLight;
                m_kColor[dwVerCnt + 1].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

            tx = vPos.x - 1;
            ty = vPos.y + 1;
            tz = vPos.z + 0;
            if (_getblock((int)tx, (int)ty, (int)tz) > 0)
            {
                m_kColor[dwVerCnt + 2].a = 1.0f;
                m_kColor[dwVerCnt + 2].r *= FLight;
                m_kColor[dwVerCnt + 2].g *= FLight;
                m_kColor[dwVerCnt + 2].b *= FLight;

                m_kColor[dwVerCnt + 3].a = 1.0f;
                m_kColor[dwVerCnt + 3].r *= FLight;
                m_kColor[dwVerCnt + 3].g *= FLight;
                m_kColor[dwVerCnt + 3].b *= FLight;

            }

        }
  //      TimeCheck.Instance.EndTime(15);

    }
    public void BlockMakeFace_Size_F(int nFace, float x, float y, float z, int nBlock, float fSize, dgGetBlock _getblock,dgGetColor _getcolor)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;


        
        
        GetUV(nFace, ref m_vUV, (byte)nBlock, _getcolor, x, y, z);

        int dwVerCnt = m_dwVertexCount;

        for (int i = 0; i < 4; i++)
        {

            vTemp.x = g_Varray[nFace,i].x;
            vTemp.z = g_Varray[nFace,i].z;
            vTemp.y = g_Varray[nFace,i].y * fSize + (1f - fSize);

            m_kVerts[m_dwVertexCount] = vTemp+vPos;

            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;

            if (_getcolor == null)
            {
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
            }
            else
            {
                
                m_kColor[m_dwVertexCount] = _getcolor((int)x, (int)y, (int)z); ;
            }

            m_dwVertexCount++;

        }


        // 
        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }

        
    }
    public void MakeFace_Slant_F(int nFace, float x, float y, float z, int nBlock, int ndir, float fHeight, dgGetBlock _getblock, dgGetColor _getcolor)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;


        
        GetUV(nFace, ref m_vUV, (byte)nBlock, _getcolor, x, y, z);

        int dwVerCnt = m_dwVertexCount;

        for (int i = 0; i < 4; i++)
        {
            if (m_dwVertexCount >= MAXVERTEX - 1) continue;
            vTemp.x = g_Varray_Slant[ndir, nFace, i].x;
            vTemp.z = g_Varray_Slant[ndir, nFace, i].z;
            vTemp.y = g_Varray_Slant[ndir, nFace, i].y * fHeight + (1f - fHeight);

            m_kVerts[m_dwVertexCount] = vTemp + vPos;


            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;


            if (_getcolor == null)
            {
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
            }
            else
            {
                m_kColor[m_dwVertexCount] = _getcolor((int)x, (int)y, (int)z);
            }

            m_dwVertexCount++;


        }



        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }




    }
    public void MakeFace_Slant(int nFace, float x, float y, float z, int nBlock, int ndir, float fHeight, dgGetBlock _getblock, dgGetColor _getcolor)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;


        GetUV(nFace, ref m_vUV, (byte)nBlock, _getcolor, x, y, z);

        int dwVerCnt = m_dwVertexCount;

        for (int i = 0; i < 4; i++)
        {
            if (m_dwVertexCount >= MAXVERTEX - 1)
            {
                continue;
            }
                
            vTemp.x = g_Varray_Slant[ndir, nFace, i].x;
            vTemp.z = g_Varray_Slant[ndir, nFace, i].z;
            vTemp.y = g_Varray_Slant[ndir, nFace, i].y * fHeight;

            m_kVerts[m_dwVertexCount] = vTemp + vPos;


            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;


            if (_getcolor == null)
            {
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
            }
            else
            {
                m_kColor[m_dwVertexCount] = _getcolor((int)x, (int)y, (int)z);
            }

            m_dwVertexCount++;


        }



        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }
        
        


    }
    public void MakeXPlan(float x, float y, float z, int nBlock)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;

        for(int j=0;j<2;j++)
        {
            int nFace = j;
            GetUV(nFace, ref m_vUV, (byte)nBlock, null);
            int dwVerCnt = m_dwVertexCount;
            for (int i = 0; i < 4; i++)
            {
                vTemp.x = g_XPlanVarray[nFace, i].x;
                vTemp.z = g_XPlanVarray[nFace, i].z;
                vTemp.y = g_XPlanVarray[nFace, i].y;
                m_kVerts[m_dwVertexCount] = vTemp + vPos;
                m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
                m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
                m_dwVertexCount++;

            }
            int nd = nFace % 2;
            for (int i = 0; i < 6; i++)
            {
                m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
                m_dwIndexCount++;
            }


        }
        for (int j = 0; j < 2; j++)
        {
            int nFace = j;
            GetUV(nFace, ref m_vUV, (byte)nBlock);
            int dwVerCnt = m_dwVertexCount;
            for (int i = 0; i < 4; i++)
            {
                vTemp.x = g_XPlanVarray[nFace, i].x;
                vTemp.z = g_XPlanVarray[nFace, i].z;
                vTemp.y = g_XPlanVarray[nFace, i].y;
                m_kVerts[m_dwVertexCount] = vTemp + vPos;
                m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
                m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
                m_dwVertexCount++;

            }
            int nd = (nFace+1) % 2;
            for (int i = 0; i < 6; i++)
            {
                m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
                m_dwIndexCount++;
            }


        }



    }
    public void MakePlan(float x, float y, float z, int nBlock)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;
        int nFace = 0;
        GetUV(nFace, ref m_vUV, (byte)nBlock);
        


        int dwVerCnt = m_dwVertexCount;
        for (int i = 0; i < 4; i++)
        {
            vTemp.x = g_XPlanVarray[nFace, i].x;
            vTemp.z = g_XPlanVarray[nFace, i].z;
            vTemp.y = g_XPlanVarray[nFace, i].y;
            m_kVerts[m_dwVertexCount] = vTemp + vPos;
            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y;
            m_kColor[m_dwVertexCount].a = 1.0f;
            m_kColor[m_dwVertexCount].r = 1.0f;
            m_kColor[m_dwVertexCount].g = 1.0f;
            m_kColor[m_dwVertexCount].b = 1.0f;
            m_dwVertexCount++;

        }

        int nd = nFace % 2;
        for (int i = 0; i < 6; i++)
        {
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }




    }
    public void MakeFace_Wedge(int nFace, float x, float y, float z, int nBlock,  int nShape, dgGetBlock _getblock,dgGetColor _getcolor)
    {
        if (m_dwVertexCount > MAXVERTEX - 10) return;
        if (m_dwIndexCount > MAXVERTEX - 10) return;

        vPos.x = x;
        vPos.y = y;
        vPos.z = z;
        vPos += m_vPos;

        int ndir = nShape - (int)BLOCKSHAPE._WEDGE;

        
        GetUV(nFace, ref m_vUV, (byte)nBlock, _getcolor, x, y, z);

        int dwVerCnt = m_dwVertexCount;
        
        for (int i = 0; i < 4; i++)
        {
            if (m_dwVertexCount >= MAXVERTEX - 1) continue;
            vTemp.x = g_Varray_Wedge[ndir,nFace,i].x;
            vTemp.z = g_Varray_Wedge[ndir,nFace,i].z;
            vTemp.y = g_Varray_Wedge[ndir,nFace,i].y ;

            m_kVerts[m_dwVertexCount] = vTemp + vPos;


            m_kUV[m_dwVertexCount].x = m_vUV.x + g_VUV[i].x ;
            m_kUV[m_dwVertexCount].y = m_vUV.y + g_VUV[i].y ;


            if (_getcolor == null)
            {
                m_kColor[m_dwVertexCount].a = 1.0f;
                m_kColor[m_dwVertexCount].r = 1.0f;
                m_kColor[m_dwVertexCount].g = 1.0f;
                m_kColor[m_dwVertexCount].b = 1.0f;
            }
            else
            {
                m_kColor[m_dwVertexCount] = _getcolor((int)x, (int)y, (int)z); 
            }

            m_dwVertexCount++;


        }



        int nd = (nFace+1) % 2;

        for (int i = 0; i < 6; i++)
        {
            m_Indexs[m_dwIndexCount] = dwVerCnt + g_nIndexOrder[nd, i];
            m_dwIndexCount++;
        }

      

    }

    public Mesh GetMesh()
    {

        if (m_dwVertexCount == 0) return null;

        Mesh kMesh;
        kMesh = new Mesh();
        kMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Vector3[] vertices = new Vector3[m_dwVertexCount];
        Vector2[] uv = new Vector2[m_dwVertexCount];
        int[] triangles = new Int32[m_dwIndexCount];
        Color[] colors = new Color[m_dwVertexCount];

        Array.Copy(m_kVerts, vertices, m_dwVertexCount);
        Array.Copy(m_kUV, uv, m_dwVertexCount);
        Array.Copy(m_Indexs, triangles, m_dwIndexCount);
        Array.Copy(m_kColor, colors, m_dwVertexCount);

        kMesh.vertices = vertices;
        kMesh.uv = uv;
        kMesh.triangles = triangles;
        kMesh.colors = colors;
        kMesh.RecalculateNormals();
        CWMakeMesh.G_MeshCount++;

     //   UnityEngine.Debug.Log(string.Format("GetMesh size {0}", m_dwVertexCount));

        return kMesh;


    }
    #region 파일 로드/세이브

    public Mesh Load(string szFile)
    {
        
        CWFile cf = new CWFile();
        if (!cf.Load(szFile)) return null;

        Mesh kMesh = new Mesh();

        m_dwVertexCount = cf.GetInt();
        m_dwIndexCount = cf.GetInt();

        Vector3[] vertices = new Vector3[m_dwVertexCount];
        Vector2[] uv = new Vector2[m_dwVertexCount];
        int[] triangles = new Int32[m_dwIndexCount];

        byte[] bBuffer= cf.GetBuffer();
        int vsize = Marshal.SizeOf(vertices[0]);
        Buffer.BlockCopy(bBuffer, 0, vertices, 0, m_dwVertexCount* vsize);

        bBuffer = cf.GetBuffer();
        vsize = Marshal.SizeOf(uv[0]);
        Buffer.BlockCopy(bBuffer, 0, uv, 0, m_dwVertexCount* vsize);

        bBuffer = cf.GetBuffer();
        vsize = Marshal.SizeOf(m_Indexs[0]);
        Buffer.BlockCopy(bBuffer, 0, m_Indexs, 0, m_dwIndexCount * vsize);

        kMesh.vertices = vertices;
        kMesh.uv = uv;
        kMesh.triangles = triangles;
        kMesh.RecalculateNormals();



        return kMesh;
    }
    public void Save(string szFile)
    {
        CWFile cf = new CWFile();

        cf.PutInt(m_dwVertexCount);
        cf.PutInt(m_dwIndexCount);

        byte[] bBuffer;
        int vsize;

        vsize = Marshal.SizeOf(m_kVerts[0])* m_dwVertexCount;
        bBuffer = new byte[vsize];
        Buffer.BlockCopy(m_kVerts, 0, bBuffer, 0, vsize);
        cf.PutBuffer(bBuffer);

        vsize = Marshal.SizeOf(m_kUV[0]) * m_dwVertexCount;
        bBuffer = new byte[vsize];
        Buffer.BlockCopy(m_kUV, 0, bBuffer, 0, vsize);
        cf.PutBuffer(bBuffer);


        vsize = Marshal.SizeOf(m_Indexs[0]) * m_dwVertexCount;
        bBuffer = new byte[vsize];
        Buffer.BlockCopy(m_Indexs, 0, bBuffer, 0, vsize);
        cf.PutBuffer(bBuffer);

        cf.Save(szFile);

    }

    #endregion

}
