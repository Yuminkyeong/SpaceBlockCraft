//#define DONTUPDATE


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
using CWStruct;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib;
using ICSharpCode.SharpZipLib.BZip2;
using ICSharpCode.SharpZipLib.Zip.Compression.Streams;
using ICSharpCode.SharpZipLib.Zip.Compression;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Bson;
using System.Diagnostics;

namespace CWUnityLib
{
    public enum ImageFilterMode : int
    {
        Nearest = 0,
        Biliner = 1,
        Average = 2
    }

    public class DebugX
    {

        public static void Log(string msg)
        {
#if UNITY_EDITOR       
         //   UnityEngine.Debug.Log(msg);
#endif
        }
    }
    public delegate void RECEIVEFUCION(JObject _Data);

    public delegate void FileFuction(string szPath, string szfile);
    public delegate void CallBackFunction();
    public delegate void CallBackCloseEvent(GameObject gg);
    public delegate bool CallBack_EventFuc(string szparam = "", string szprarm2 = "");

    public delegate void DgOKFuction();
    public delegate void DgCancelFuction();
    public class BUILDATA
    {
      public  string name;
      public  int GNumber;
    }
    public class CWCloseEvent : MonoBehaviour
    {

        public CallBackFunction CloseEvent;

        void OnDestroy()
        {
            if (CloseEvent != null)
                CloseEvent();
        }

    }
    //// 반지름 데이타 
    //public class CWSortWonData
    //{
    //    struct POS
    //    {
    //        public int x;
    //        public int y;
    //    }
    //    public CWSortWonData()
    //    {

    //    }
    //    void Create()
    //    {


    //    }

    //}

#if DONTUPDATE
    public class CWBehaviour : MonoBehaviour
    {




        protected bool g_bOnce = false;
        public virtual void Close()
        {

        }
        protected virtual bool OnceRun()
        {
            return true;
        }
        private void OnEnable()
        {
            if (!CWGlobal.g_bGameStart) return;

            g_bOnce = false;
            StartCoroutine("RunCor");
        }
        private void OnDisable()
        {
            g_bOnce = false;
            
        }

        IEnumerator RunCor()
        {
            bool bRet = true;
            while(bRet)
            {
                if(CWGlobal.g_bGameStart)
                {
                    if (!g_bOnce)
                    {
                        g_bOnce = OnceRun();
                    }
                    bRet= Run();
                }
                yield return new WaitForSeconds(0.2f);
            }
        }

        protected virtual bool Run() { return false; }
    }

#else

    public class CWBehaviour : MonoBehaviour
    {

        
        protected bool g_bOnce = false;
        
        public virtual void Close()
        {

        }
        protected virtual bool OnceRun()
        {
            return true;
        }
        private void OnEnable()
        {
            
            g_bOnce = false;
            StartCoroutine("IRun");
            
        }
        private void OnDisable()
        {
            g_bOnce = false;
            

        }
        IEnumerator IRun()
        {
            while(!g_bOnce)
            {
                if (CWGlobal.G_bGameStart)
                {
                    g_bOnce = OnceRun();
                }
                yield return null;
            }

        }
        //void Update()
        //{
            
        //    if (!CWGlobal.g_bGameStart) return;
        //    if (!g_bOnce)
        //    {
        //        g_bOnce = OnceRun();
        //    }
        
        //}

    }
#endif        
    
    class SNailData
    {
        Vector2[] g_Dir =
        {
           new Vector2(1,0),new Vector2(0,-1),new Vector2(-1,0),new Vector2(0,1)
        };
        public List<Vector2> m_kList = new List<Vector2>();
        void AddData(Vector2 v)
        {
            m_kList.Add(v);
        }

        public void MakeSnailData(int nCount)
        {
            int j = 0;


            Vector2 vStart = new Vector2(0, 0);
            Vector2 val = vStart + g_Dir[j];


            AddData(val);

            j++; j %= 4;
            val += g_Dir[j];
            AddData(val);

            j++; j %= 4;

            float tcnt = 2;
            for (int i = 0; ; i++)
            {

                for (int k = 0; k < (int)tcnt; k++)
                {
                    val += g_Dir[j];
                    AddData(val);
                }
                tcnt += 0.5f;

                j++; j %= 4;

                if (m_kList.Count >= nCount)
                {
                    break;
                }
            }



        }



    }

    public class CWMath
    {

        #region 좌표 변환

        //float 를 byte로 변환
        public static byte ConvertByteYaw(float fYaw)
        {
            int val = (360 + (int)(fYaw)) % 360;
            val = (int)((float)val * 255 / 360.0f);//0~255 
            return (byte)val;
        }
        public static float ConvertFloatYaw(byte bVal)
        {
            return ((float)bVal) * (360.0f / 255.0f);
        }

        //float 를 byte로 변환
        public static byte ConvertByteHeight(float fHeight)
        {
            int val = (int)(fHeight*(255/100f));//100 -> 255
            return (byte)val;
        }
        public static float ConvertFloatHeight(byte bVal)
        {
            return ((float)bVal) * (100f / 255.0f);//255->100
        }


        // float -> 2바이트 변환
        public static byte[] ConvertByte_Float(float fValue)
        {
            int n1 = (int)fValue;
            int n2 =(int) ((fValue - (float)n1) * 255.0f);

            byte[] bb = new byte[2];
            bb[0] = (byte)n1;
            bb[1] = (byte)n2;
            return bb;
        }
        // 16비트 - > float
        public static float ConvertFloat_Byte(byte b1,byte b2)
        {
            return (float)b1 + (float)b2 / 255f;
        }


        #endregion


        public static float ContAngle(Vector3 fwd, Vector3 targetDir)
        {
            float angle = Vector3.Angle(fwd, targetDir);

            if (AngleDir(fwd, targetDir, Vector3.up) == -1)
            {
                angle = 360.0f - angle;
                if (angle > 359.9999f)
                    angle -= 360.0f;
                return angle;
            }
            else
                return angle;
        }
        public static int AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
        {
            Vector3 perp = Vector3.Cross(fwd, targetDir);
            float dir = Vector3.Dot(perp, up);

            if (dir > 0.0)
                return 1;
            else if (dir < 0.0)
                return -1;
            else
                return 0;
        }

        /// 개념 
        /// 회전각의 방향을 구한다 
        /// // 어떤 방향으로 증가를 해야 되는가
        /// 
        public static float GetAngleDir(float a, float b)
        {
            a = (360 + a) % 360;
            b = (360 + b) % 360;
            float fdir = 1;
            if (a - b < 0)// 뒤에 것이 크고 오른쪽 회전 
            {
                fdir = 1;
            }
            else
            {
                fdir = -1;
            }
            if (Mathf.Abs(a - b) > 180)
            {
                fdir = fdir * -1;
            }

            return fdir;
        }
        public static float GetAngleDist(float a, float b)
        {

            // 절대 마이너스 값이 나오면 안됨
            a = (360 + a) % 360;
            b = (360 + b) % 360;

            float v1 = Mathf.Abs(a - b);
            float v2 = Mathf.Abs(360 - v1);

            if (v1 > v2) return v2;
            return v1;


        }
        // num은 1부터 시작하는 숫자임
        // num은 순서를 뜻하는 숫자가 아니라, 실제 수를 뜻함
        // 4의 주기의 함수  0,1,2,3,2,1,0 을 리턴함 
        // LoofWidth 주기 
        // 
        /// <summary>
        /// 예]
        /// LoofWidth = 5 
        /// 결과   0,1,2,3,4,3,2,1,0 
        /// </summary>
        /// <param name="num">1 부터 시작하는 숫자</param>
        /// <param name="LoofWidth">주기 길기 </param>
        /// <returns></returns>
        public static int GetLoof(int num, int LoofWidth)
        {
            if (num > LoofWidth * 2) return -1;
            int n = 0;
            if (num <= LoofWidth)
            {
                n = num % LoofWidth;
                if (n == 0)
                {
                    return LoofWidth - 1;
                }
                return n - 1;

            }
            else
            {
                n = (LoofWidth * 2 - 1) - num;
            }
            return n;
        }

        /// <summary>
        /// 핵사 도형 좌표를 리턴한다
        /// </summary>
        /// <param name="sx">시작 </param>
        /// <param name="sy">시작 </param>
        /// <param name="width">핵사 도형 dx</param>
        /// <param name="height">핵사 도형 dy</param>
        /// <param name="HexaWidth">핵사 개수 </param>
        /// <returns></returns>
        public static List<Vector2> MakeHedron(int sx, int sy, int width, int height,int HexaWidth)
        {
            
            List<Vector2> kList = new List<Vector2>();
            int dx = HexaWidth * 2 - 1;
            int startx = sx;
            int starty = sy;

            for (int x = 0; x < dx; x++)
            {
                int rr = GetLoof(x + 1, HexaWidth);
                int dy = HexaWidth + rr;
                starty = sy - (rr) * (height / 2);

                for (int y = 0; y < dy; y++)
                {
                    Vector2 v = new Vector2(startx + x * width, starty + y * (height));
                    kList.Add(v);
                }

            }
            return kList;
        }






        /// <summary>
        /// 주기함수
        /// 어떤 값을 주면 주기적으로 값이 0~1~0으로 이동한다.
        /// fRate : 0~1의 값을 넣는다
        /// 결과값 : 0~1~0의 값을 리턴한다
        /// </summary>
        /// <param name="fRate"></param>
        /// <returns></returns>
        public static float GetCycle(float fRate)
        {
            return Mathf.Sin(fRate * Mathf.PI);
        }
        // 0  1    2    3
        // 0,-1,1,-2,2,-3,3 이렇게 넘겨준다
        public static int GetCircle(int num)
        {
            if(num==0)
            {
                return 0;
            }
            int val = num / 2;
            if(num%2==0)
            {
                return val;
            }
            return -val;

        }


        // 게임 오브젝트 좌표를 마우스 좌표로 바꾼다 
        public static Vector3 ConvertMousePos(GameObject gg)
        {
            return ConvertMousePos(gg.transform.localPosition);

        }
        public static Vector3 ConvertMousePos(Vector3 vPos)
        {
            Vector3 vMovePos = new Vector3();
            vMovePos.x = vPos.x + (Screen.width / 2f);
            vMovePos.y = vPos.y + (Screen.height / 2f);

            return vMovePos;

        }

        // 마우스 좌표를 게임오브젝트 좌표로 바꾼다 
        public static Vector3 ConvertByMousePos(Vector3 vMousePos)
        {
            Vector2 vRet = new Vector2();
            Canvas cv=MainUI.Instance.GetComponentInParent<Canvas>();
            RectTransform  rr= cv.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rr, vMousePos, cv.worldCamera, out vRet);
            return vRet;
        }
        //-  두 벡터사이의 평면거리 제곱근 없음
        public static float VectorDistSQ(Vector3 v1, Vector3 v2)
        {
            float fdist = (v1.x - v2.x) * (v1.x - v2.x) + (v1.z - v2.z) * (v1.z - v2.z);
            return fdist;
        }
        // 상대적인 방향을 구한다

        // 현재의 벡터에서 fYaw 각도 만큼 이동을 한 벡터을 출력
        public static Vector3 CalYaw(float fYaw, Vector3 vForward)
        {
            Quaternion qq = Quaternion.Euler(new Vector3(0, fYaw, 0));
            return qq * vForward;
        }
        public static Vector3 CalPitch(float fPitch)
        {
            Quaternion qq = Quaternion.Euler(new Vector3(fPitch, 0, 0));
            return qq * Vector3.forward;
        }
        public static Vector3 CalAngle(float fYaw,float fPitch, Vector3 vForward)
        {
            Quaternion qq = Quaternion.Euler(new Vector3(fPitch, fYaw, 0));
            return qq * vForward;
        }
        public static float GetLookYaw(Vector3 vSource, Vector3 vTarget)
        {
            Vector3 vDir = vTarget - vSource;
            vDir.Normalize();
            return CalRadian_Yaw(vDir);
        }
        public static float CalRadian_Yaw(Vector3 vDir)//XZ 평면상의 각도 구하기
        {

            float fdir;
            if (vDir.x > 0.0f)
            {
                fdir = -Mathf.Atan(vDir.z / vDir.x) + Mathf.PI / 2f;
            }
            else
            {
                if (vDir.x == 0)
                {
                    if (vDir.z > 0)
                    {
                        fdir = 0f;
                    }
                    else
                    {
                        fdir = Mathf.PI;
                    }
                }
                else
                {
                    fdir = -Mathf.Atan(vDir.z / vDir.x) - Mathf.PI / 2f;
                }
            }
            return (360f + (fdir * Mathf.Rad2Deg)) % 360;

        }

        public static bool IsEqual(float a, float b,float fEps=0.001f)
        {
            
            if (a >= b - fEps && a <= b + fEps)
                return true;
            else
                return false;
        }
        public static bool IsEqualAnlge(float a,float b,float fEps = 0.001f)
        {
            int x1 = (360 + (int)(a)) % 360;
            int x2 = (360 + (int)(b)) % 360;
            if (Mathf.Abs(x1 - x2) > fEps) return false;
            return true;

        }
        public static bool IsEqualEuler(Vector3 a, Vector3 b)
        {
            float fEps = 0.001f;

            if (!IsEqualAnlge(a.x, b.x, fEps)) return false;
            if (!IsEqualAnlge(a.y, b.y, fEps)) return false;
            if (!IsEqualAnlge(a.z, b.z, fEps)) return false;

            return true;
            

        }
        public static bool IsEqual(Vector3 a, Vector3 b, float fEps = 0.001f)
        {
            if (!IsEqual(a.x, b.x, fEps)) return false;
            if (!IsEqual(a.y, b.y, fEps)) return false;
            if (!IsEqual(a.z, b.z, fEps)) return false;

            return true;
        }
        public static bool IsEqual(Color a, Color b)
        {
            if (!IsEqual(a.r, b.r)) return false;
            if (!IsEqual(a.g, b.g)) return false;
            if (!IsEqual(a.b, b.b)) return false;
            return true;
        }

        public static bool IsEqualDist(Vector3 a, Vector3 b,float fdist=0.01f)
        {
            if (!IsEqual(a.x, b.x,fdist)) return false;
            if (!IsEqual(a.y, b.y, fdist)) return false;
            if (!IsEqual(a.z, b.z, fdist)) return false;

            return true;


        }
        /// <summary>
        /// 양의 정수 구간 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="range"></param>
        /// <returns></returns>

        public static int IntGrid(int val, int range)
        {
            return (range + (val % range)) % range;
        }

        public static float GetDistColor(Color a1,Color a2)
        {
            Vector3 v1 = new Vector3(a1.r, a1.g, a1.b);
            Vector3 v2 = new Vector3(a2.r, a2.g, a2.b);
            v1.Normalize();
            v2.Normalize();
            return Vector3.Distance(v1, v2);
        }



    }
    public class CWLib 
    {
        


        #region 파일 로드 ==> 버그 났음, 추후에 고치던지 혹은 지우던지, 사용금지 참고용으로만!!

        public static object RawDeserializeEx(byte[] rawdatas, Type anytype)
        {
            int rawsize = Marshal.SizeOf(anytype);
            if (rawsize > rawdatas.Length)
                return null;
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            object retobj = Marshal.PtrToStructure(buffer, anytype);
            handle.Free();
            return retobj;
        }

        public static byte[] RawSerializeEx(object anything)
        {
            int rawsize = Marshal.SizeOf(anything);
            byte[] rawdatas = new byte[rawsize];
            GCHandle handle = GCHandle.Alloc(rawdatas, GCHandleType.Pinned);
            IntPtr buffer = handle.AddrOfPinnedObject();
            Marshal.StructureToPtr(anything, buffer, false);
            handle.Free();
            return rawdatas;
        }
        //StreamAssets에 존재하는 파일 로드
        public static object GetStreamAssetsFile(string szFile, Type anytype)
        {
            string szpath = Application.streamingAssetsPath + "/" + szFile;

            if (File.Exists(szpath))
            {
                using (FileStream fs = File.OpenRead(szpath))
                {
                    if (fs.Length == 0)
                    {
                        fs.Close();
                        return null;
                    }

                    byte[] bBuffer = new byte[fs.Length];
                    fs.Read(bBuffer, 0, (int)fs.Length);
                    fs.Close();
                    return RawDeserializeEx(bBuffer, anytype);
                }
            }

            return null;
        }

        #endregion


        // 달팽이처럼 빙빙 돌아가는 알고리즘 
        public static List<Vector2> MakeSnailData(int nCount)
        {
            SNailData kData = new SNailData();
            kData.MakeSnailData(nCount);
            return kData.m_kList;
        }
        // 3d 오브젝트를 UI 좌표로 변환

       public static Vector2 ConvertUIPosition(GameObject gTarget, float Xanchor = 0.0f,float Yanchor = 0.0f, float Xoff = 0.0f,float Yoff = 0.0f, float Xcor = 0.0f, float Ycor = 0.0f, float Zcor = 0.0f)
       {
            if (Camera.main == null) return Vector2.zero;
            Canvas kCanvas = Game_App.Instance.Canvas_Window;

            RectTransform CanvasRect = kCanvas.GetComponent<RectTransform>();
            Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(new Vector3(gTarget.transform.position.x + Xcor,
                                                                                    gTarget.transform.position.y + Ycor,
                                                                                    gTarget.transform.position.z + Zcor));

            Vector2 WorldObject_ScreenPosition = new Vector2(
            ((ViewportPosition.x * CanvasRect.sizeDelta.x) - (CanvasRect.sizeDelta.x * 0.5f)) + Xanchor + Xoff,
            ((ViewportPosition.y * CanvasRect.sizeDelta.y) - (CanvasRect.sizeDelta.y * 0.5f)) + Yanchor + Yoff);


            //GetComponent<RectTransform>().anchoredPosition =
            return new Vector2(WorldObject_ScreenPosition.x,
                                                   WorldObject_ScreenPosition.y);
            
            
       }
            



        // 화면에 벗어났는지 검사 
        public static bool IsOutScreen(int dx,int dy, GameObject gTarget)
        {

            Vector2 vv= ConvertUIPosition(gTarget);
            float fx = dx / 2;
            float fy = dy / 2;

            if (vv.x > fx) return true;
            if (vv.x < -fx) return true;

            if (vv.y > fy) return true;
            if (vv.y < -fy) return true;

            return false;

        }


        // 대소문자 구분안하고 검색
        public static bool IsEqual(string sz1,string sz2)
        {
            return sz1.Equals(sz2, StringComparison.CurrentCultureIgnoreCase);
        }

        static System.Random gRandom=new System.Random();
        static AudioListener mListener;
        static float mLastTimestamp = 0f;
        static AudioClip mLastClip;

        static bool mLoaded = false;
        static float mGlobalVolume = 1f;

        public static void RandomInit(int nSeed)
        {
            //UnityEngine.Random.
            gRandom = new System.Random(nSeed);
        }
        public static int Random(int min,int max)
        {
            
            return gRandom.Next(min, max);
        }

        //3D 좌표를 2D 좌표로 바꾸어 준다 
        static public Vector3 Convert2DUIPos(Vector3 vTargetPos)
        {
            Camera kCamera = Camera.main;

            Vector3 vPos = kCamera.WorldToScreenPoint(vTargetPos);
            vPos.z = 0;
            vPos.x -= Screen.width / 2;
            vPos.x = vPos.x / 2;
            vPos.y -= Screen.height / 2;
            vPos.y = vPos.y / 2;

            return vPos;
        }


        static public float soundVolume
        {
            get
            {
                if (!mLoaded)
                {
                    mLoaded = true;
                    mGlobalVolume = PlayerPrefs.GetFloat("Sound", 1f);
                }
                return mGlobalVolume;
            }
            set
            {
                if (mGlobalVolume != value)
                {
                    mLoaded = true;
                    mGlobalVolume = value;
                    PlayerPrefs.SetFloat("Sound", value);
                }
            }
        }



        public static AudioSource PlaySound(AudioClip clip, float volume, float pitch)
        {
            

            float time = Time.time;
            if (clip == null) return null;
            if (mLastClip == clip && mLastTimestamp + 0.1f > time) return null;

            

            mLastClip = clip;
            mLastTimestamp = time;
            volume *= soundVolume;

            if (clip != null && volume > 0.01f)
            {
                if (mListener == null || !NGUITools.GetActive(mListener))
                {
                    AudioListener[] listeners = GameObject.FindObjectsOfType(typeof(AudioListener)) as AudioListener[];

                    if (listeners != null)
                    {
                        for (int i = 0; i < listeners.Length; ++i)
                        {
                            if (NGUITools.GetActive(listeners[i]))
                            {
                                mListener = listeners[i];
                                break;
                            }
                        }
                    }

                    if (mListener == null)
                    {
                        Camera cam = Camera.main;
                        if (cam == null) cam = GameObject.FindObjectOfType(typeof(Camera)) as Camera;
                        if (cam != null) mListener = cam.gameObject.AddComponent<AudioListener>();
                    }
                }

                if (mListener != null && mListener.enabled && NGUITools.GetActive(mListener.gameObject))
                {


#if UNITY_4_3 || UNITY_4_5 || UNITY_4_6
				AudioSource source = mListener.audio;
#else
                    AudioSource source= mListener.GetComponent<AudioSource>();
                    
#endif
                    if (source == null) source = mListener.gameObject.AddComponent<AudioSource>();
#if !UNITY_FLASH
                    source.priority = 50;
                    source.pitch = pitch;
#endif

                    source.PlayOneShot(clip, volume);
                    

                    return source;
                }
            }
            return null;

        }

        static public AudioSource PlaySound(AudioClip clip, GameObject gTarget=null,float fVolumnRate = 1f) 
        {

               if (clip == null) return null;
               if (!CWGlobal.g_bSoundOn) return null;

            Camera kCamera = Camera.main;

            float fVolumn =1f;
            if (gTarget != null)
            {
                float fDist = Vector3.Distance(gTarget.transform.position, kCamera.transform.position);
                if (fDist > 150f) return null;
                if (fDist > 100f) fVolumn=0.2f;
                if (fDist > 50f) fVolumn = 0.5f;
                else fVolumn = 1- fDist / 100f;

                
              //   UnityEngine.Debug.Log(string.Format("sound dist {0} {1} ",fDist,fVolumn* fVolumnRate));

            }


            
            return PlaySound(clip, fVolumn, 1f); 
        }
        // 맵셀의 좌표를 구한다 
        static public int GetSellNumber(int x,int y,int z)
        {
            return (x * CWGlobal.SELLCOUNT + z) * CWGlobal.WD_WORLD_HEIGHT + y;
        }
        static public int GetSellX(int num)
        {
            return num / (CWGlobal.WD_WORLD_HEIGHT * CWGlobal.SELLCOUNT);
        }
        static public int GetSellY(int num)
        {
            return num % CWGlobal.WD_WORLD_HEIGHT;
        }
        static public int GetSellZ(int num)
        {
            return (num / CWGlobal.WD_WORLD_HEIGHT) % ( CWGlobal.SELLCOUNT);

        }
        // 충돌박스를 구한다 

        static public void GetDetectBox(Vector3 vPos,Vector3 vNormal,ref int nx,ref int ny,ref int nz)
        {
            Vector3 v = vPos - vNormal;

            float fx = v.x + (float)CWGlobal.SELLCOUNT / 2;
            if (vNormal.x < 0)
            {
                fx--;
            }

            nx = (int)fx - CWGlobal.SELLCOUNT / 2;
            float fz = v.z + (float)CWGlobal.SELLCOUNT / 2;
            if (vNormal.z < 0)
            {
                fz--;
            }

            nz = (int)fz - CWGlobal.SELLCOUNT / 2;
            ny = (int)v.y;


        }
        // 회전을 하며 끝난후 펑션 실행
        public static void TurntoEnmey(GameObject gg, Vector3 vTarget, EventDelegate.Callback fuc)
        {
            Vector3 _Way = vTarget - gg.transform.position;
            float _Angle = Mathf.Atan2(_Way.x, _Way.z) * Mathf.Rad2Deg;

            TweenRotation TweenRotation = gg.GetComponent<TweenRotation>();
            if (TweenRotation == null)
            {
                TweenRotation = gg.AddComponent<TweenRotation>();
            }
            TweenRotation.style = UITweener.Style.Once;
            TweenRotation.duration = 0.5f;
            TweenRotation.SetOnFinished(fuc);
            
            TweenRotation.from = gg.transform.eulerAngles;
            TweenRotation.to = new Vector3(TweenRotation.from.x, _Angle, TweenRotation.from.z);
            if (TweenRotation.from.y - TweenRotation.to.y > 180)
            {
                TweenRotation.from.y -= 360;
            }
            else if (TweenRotation.from.y - TweenRotation.to.y < -180)
            {
                TweenRotation.from.y += 360;
            }
            
            TweenRotation.ResetToBeginning();
            TweenRotation.PlayForward();



        }


        static public void AddBombRigiBody(GameObject gg)
        {
            if (gg.GetComponent<MeshRenderer>() != null)
            {
                Rigidbody rr = gg.GetComponent<Rigidbody>();
                if (rr == null)
                {
                    rr = gg.AddComponent<Rigidbody>();
                }
                if (rr != null)
                {
                   
                    //rr.AddExplosionForce(2000f, gg.transform.position, 20f,30f);
                    rr.AddForce(Vector3.up*200f);
                }

            }
            Transform trans = gg.transform;
            for (int i = 0; i < trans.childCount; i++)
            {
                Transform tChild= trans.GetChild(i);
                AddBombRigiBody(tChild.gameObject);
            }
        }
        
        
        public static string GetGameObjectPath(Transform transform)
        {
            string path = transform.name;
            while (transform.parent != null)
            {
                transform = transform.parent;
                path = transform.name + "/" + path;
            }
            return path;
        }
 

        public static bool IsCameraView(Vector3 vStPos, Vector3 vEnPos, float fDist)
        {
            Camera kCamera = Camera.main;

            float ff = CWMath.VectorDistSQ(kCamera.transform.position, vStPos);
            if (ff > fDist * fDist)
            {
                return false;
            }
            if (IsCameraView(vStPos))
            {
                return true;
            }
            if (IsCameraView(vEnPos))
            {
                return true;
            }
            return false;
        }

        ////public static bool IsFront(Vector3 vPos, Vector3 vforward, Vector3 vtarget)
        //{
        //    vPos.y = 0;
        //    vforward.y = 0;
        //    vtarget.y = 0;

        //    Vector3 vdir = vtarget - vPos;
        //    float ff = Vector3.Angle(vdir, vforward);
        //    if (ff < 90) return true;

        //    return false;

        //}

        // 내 앞에있는 벡터인가? [6/17/2015 CWWork]
        public static bool IsFront(Vector3 vPos, Vector3 vforward, Vector3 vtarget,float fAngle)
        {

            vPos.y = 0;
            vforward.y = 0;
            vtarget.y = 0;

            Vector3 vdir = vtarget - vPos;
            float ff = Vector3.Angle(vdir, vforward);
            if (ff < fAngle) return true;
            return false;// 안보임

        }

        public static bool IsCameraView(Vector3 vTarget)
        {
            Camera kCamera = Camera.main;

            bool bviewflag = IsFront(kCamera.transform.position, kCamera.transform.forward, vTarget, 90);
            Vector3 pos = kCamera.WorldToViewportPoint(vTarget);
            if (!bviewflag || (pos.x < 0f || pos.x > 1 || pos.y < 0f || pos.y > 1))
            {
                return false;
            }
            return true;

        }

        public static void ShowGameObject(GameObject gg, bool bshow)
        {
            if (gg == null) return;
            Transform tt = gg.transform;
            Renderer rr = tt.GetComponent<Renderer>();
            if (rr != null)
            {
                rr.enabled = bshow;
            }
            for (int i = 0; i < tt.childCount; i++)
            {
                ShowGameObject(tt.GetChild(i).gameObject, bshow);
            }

        }

        public static GameObject CheckMeshbyPos(Vector3 vPos,float fMaxDistance=10f,int layermask=0)
        {
            Camera kCamera = Camera.main;
            RaycastHit hit;
            if (!Physics.Raycast(kCamera.ScreenPointToRay(vPos), out hit,fMaxDistance,layermask))
                return null;
            return hit.collider.gameObject;

        }


        public static GameObject CheckMesh()
        {
            Camera kCamera = Camera.main;


            RaycastHit hit;
            if (!Physics.Raycast(kCamera.ScreenPointToRay(Input.mousePosition), out hit))
                return null;
           // Debug.Log(hit.collider.name);
            return hit.collider.gameObject;

        }

        static public int [] ConvertIntArry(string szValues)
        {
            string [] vv = szValues.Split(',');
            if (vv.Length == 0) return null;

            int[] aa = new int[vv.Length];

            for(int i=0;i<aa.Length;i++)
            {
                aa[i] = ConvertInt(vv[i]);
            }
            return aa;
        }
   
       static public float ConvertFloat(string szValues)
       {
           if (szValues.Length == 0) return 0;
            if (szValues == "NaN") return 0;
           float result = 0;
        
           if (!float.TryParse(szValues, out result)) return 0;
           return result;

       }
       static public long ConvertLong(string szValues)
       {
            if (szValues == null) return 0;
            if (szValues.Length == 0) return 0;
           long result = 0;
            if (!long.TryParse(szValues, out result)) return 0;

           return result;
       }

       static public int ConvertInt(string szValues)
       {
            if (szValues == null) return 0;
            if (szValues.Length == 0) return 0;
            if (szValues == "NaN") return 0;
            int result = 0;
            if (!int.TryParse(szValues, out result)) return 0;
            return result;


        }
        static public bool IsJSonData(JToken kt)
        {
            if (kt == null) return false;
            if (kt.Type == JTokenType.Null) return false;
            if (kt.ToString() == "") return false;
            return true;
        }
        static public int ConvertIntbyJson(JToken kt)
        {
            if (kt == null) return 0;

            return ConvertInt(kt.ToString());
        }
        //sz1을 sz2로 바꾼다
        static  string _ChangeString(string szStr,string sz1,string sz2)
        {
            int tcnt = szStr.LastIndexOf(sz1);
            if (tcnt == -1) return null;
            int l = szStr.Length;
            string szRet = szStr.Substring(0, tcnt);
            tcnt += sz1.Length;
            string szRet2 = szStr.Substring(tcnt,l-tcnt);
            return szRet + sz2 + szRet2;
        }
        static public string ChangeString(string szStr, string sz1, string sz2)
        {
            int tcnt = szStr.LastIndexOf("@");
            if (tcnt == -1) return szStr;

            string szRet = szStr;
            while(true)
            {
                string sz = _ChangeString(szRet, sz1, sz2);
                if(sz==null)
                {
                    break;
                }
                szRet = sz;
            }

            return szRet;
        }

        static public string DelExtString(string szPath)
       {
           int tcnt = szPath.LastIndexOf('.');
           if (tcnt <= 0) return szPath;
           szPath = szPath.Substring(0,szPath.LastIndexOf('.') );
           return szPath;

       }
       static public string DelPathString(string szPath)
       {
           szPath = szPath.Substring(szPath.LastIndexOf('/') + 1);
           return szPath;
       }
        static public string pathForDocumentsPath()
        {
            string path = Application.dataPath;
            path = path.Substring(0, path.LastIndexOf('/'));
            return path;
        
        }
        public static Vector3 ExtractTranslationFromMatrix(ref Matrix4x4 matrix)
        {
            Vector3 translate;
            translate.x = matrix.m03;
            translate.y = matrix.m13;
            translate.z = matrix.m23;
            return translate;
        }

        public static Quaternion ExtractRotationFromMatrix(ref Matrix4x4 matrix)
        {
            Vector3 forward;
            forward.x = matrix.m02;
            forward.y = matrix.m12;
            forward.z = matrix.m22;

            Vector3 upwards;
            upwards.x = matrix.m01;
            upwards.y = matrix.m11;
            upwards.z = matrix.m21;

            return Quaternion.LookRotation(forward, upwards);
        }
        public static Vector3 ExtractScaleFromMatrix(ref Matrix4x4 matrix)
        {
            Vector3 scale;
            scale.x = new Vector4(matrix.m00, matrix.m10, matrix.m20, matrix.m30).magnitude;
            scale.y = new Vector4(matrix.m01, matrix.m11, matrix.m21, matrix.m31).magnitude;
            scale.z = new Vector4(matrix.m02, matrix.m12, matrix.m22, matrix.m32).magnitude;
            return scale;
        }
        //2번째 텍스쳐를 바꾼다
        public static void SetMaterialSecondTexture(Transform tt,Texture kTexture)
        {
            Renderer rr = tt.GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.SetTexture("_DecalTex", kTexture);

            }
            for (int i = 0; i < tt.childCount; i++)
            {
                SetMaterialSecondTexture(tt.GetChild(i),  kTexture);
            }

        }
        public static void SetMaterialChild(Transform tt, Color nColor,Texture kTexture,Texture kTexture2=null)
        {
            Renderer rr = tt.GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.color = nColor;
                if (kTexture!=null)
                {
                    rr.material.mainTexture = kTexture;
                    if (kTexture2!=null)
                    {
                        rr.material.SetTexture("_DecalTex", kTexture2);
                    }
                    
                }
            }
            for (int i = 0; i < tt.childCount; i++)
            {
                SetMaterialChild(tt.GetChild(i), nColor,kTexture);
            }
        }

        public static void SetColorChild(Transform tt, Color nColor)
        {
            Renderer rr = tt.GetComponent<Renderer>();
            if (rr != null)
            {
                rr.material.color = nColor;
                rr.material.SetColor("_TintColor", nColor);
                
            }
            for (int i = 0; i < tt.childCount; i++)
            {
                SetColorChild(tt.GetChild(i), nColor);
            }
        }

        public static void GetUV(ref Vector2 vUV, byte nBlock)
        {
            int x = CWArrayManager.Instance.m_kBlock[nBlock].x;
            int y = CWArrayManager.Instance.m_kBlock[nBlock].y;

            float sx = (float)x;
            float sy = (float)y;
            vUV.x = sx / 64f;
            vUV.y = sy / 64f;

        }

        public static void UpdateUV(GameObject gg, Texture2D tex, int nBlock)
        {


            Renderer rr = gg.GetComponent<Renderer>();
            Vector2 vv = new Vector2();
            GetUV(ref vv, (byte)nBlock);
            rr.material = new Material(Shader.Find("CWShader"));
            rr.material.mainTextureOffset = vv;
            rr.material.mainTextureScale = new Vector2(1f / 64f, 1f / 64f);
            rr.material.mainTexture = tex;
        }


        public static Transform SearchObject(Transform target, Transform tObject)
        {
            if (target == tObject) return target;

            for (int i = 0; i < target.childCount; ++i)
            {
                var result = SearchObject(target.GetChild(i), tObject);

                if (result != null) return result;
            }

            return null;
        }


        public static Transform Search(Transform target, string name)
        {
                
            if (string.Compare(target.name,  name,  true)==0) return target;
            
            

            for (int i = 0; i < target.childCount; ++i)
            {
                var result = Search(target.GetChild(i), name);

                if (result != null) return result;
            }

            return null;
        }

        public static GameObject SearchParent(GameObject gg, string name)
        {

            Transform pkNode = gg.transform;
            do
            {
                if (pkNode.gameObject.name == name)
                {
                    return pkNode.gameObject;
                }

                pkNode = pkNode.parent;
            } while (pkNode != null);


            return null;
        }
        static public GameObject FindChildbyObject(GameObject parent, GameObject gObject)
        {

            Transform tt = SearchObject(parent.transform, gObject.transform);
            if (tt != null)
            {
                return tt.gameObject;
            }
            return null;
        }

        static public GameObject FindChild(GameObject parent, string szname)
        {
            if (szname == null) return null;
            if (szname.Length == 0) return null;
            if (parent == null)
            {
                
                //Debug.LogError("Parent error");
                return null;
            }
            Transform tt= Search(parent.transform, szname);
            if (tt!=null)
            {
                return tt.gameObject;
            }
            return null;
        }
        static public Vector3 GetPostion(GameObject parent, string szname)
        {
            GameObject gg = FindChild(parent, szname);
            if (gg == null) return Vector3.zero;
            return gg.transform.position;
        }
        static public void SetGameObjectLayer(GameObject gg,int nLayer)
        {
            if (gg == null)
            {
                return;
            }
            gg.layer = nLayer;
            for (int i = 0; i < gg.transform.childCount; i++)
            {
                Transform tChild = gg.transform.GetChild(i);
                SetGameObjectLayer(tChild.gameObject, nLayer);
            }
        }
        static public void SetGameObjectTag(GameObject gg, string sztag)
        {
            if (gg == null)
            {
                return;
            }
            gg.tag = sztag;
            for (int i = 0; i < gg.transform.childCount; i++)
            {
                Transform tChild = gg.transform.GetChild(i);
                SetGameObjectTag(tChild.gameObject, sztag);
            }
        }
        // 메쉬저장

        static public void SaveMesh(Mesh kMesh, string szname)
        {
#if UNITY_EDITOR
            if (kMesh==null)
            {
                DebugX.Log("null!!");
                return;
            }


            //string szPath;
            //szPath = string.Format("{0}/{1}", Application.dataPath, szname);
            string szPath = string.Format("Assets/Resources/Mapsell/{0}.asset", szname);

            AssetDatabase.CreateAsset(kMesh, szPath);
            AssetDatabase.SaveAssets();
            DebugX.Log("save ok assets !! ");
#endif
        }

        static public void SetParent(GameObject gChild, GameObject gParent)
        {
            gChild.transform.parent = gParent.transform;
            gChild.transform.localPosition = new Vector3(0, 0, 0);
            gChild.transform.localRotation = new UnityEngine.Quaternion();
            

            SetGameObjectLayer(gChild,gParent.layer);

        }
        static public void ClearAllChild(GameObject parent)
        {
            if (parent == null) return;
            for (int i = 0; i < parent.transform.childCount; i++)
            {
                GameObject.Destroy(parent.transform.GetChild(i).gameObject);
            }
        }
        static public GameObject AddChild(GameObject parent, GameObject prefab)
        {
            // 에디터 용!!
            GameObject go = GameObject.Instantiate(prefab) ;
            if (go != null && parent != null)
            {
                Transform t = go.transform;
                t.parent = parent.transform;
                t.localPosition = Vector3.zero;
                t.localRotation = Quaternion.identity;
                t.localScale = Vector3.one;
                go.layer = parent.layer;
            }
            return go;
        }
        static  void SubAddChildList(ref List<Transform> gList, Transform TT)
        {
//             if (TT.name == "barpos")
//             {
//                 TT.gameObject.SetActive(false);
//                 return;
//             }

            gList.Add(TT);

            
            for (int i = 0; i <TT.childCount; i++)
            {
                SubAddChildList(ref gList, TT.GetChild(i));
            }
        }
        

        static public GameObject FindObject(string sztag, Vector3 vPos)
        {
            if (sztag == null) return null;
            if (sztag.Length < 1) return null;
            float m_fMinDist = 0f;
            Transform tRet = null;
            GameObject[] gArray = GameObject.FindGameObjectsWithTag(sztag);
            foreach (var l in gArray)
            {
                if (l == null) continue;
                Transform tChild = l.transform;
                float fdist = Vector3.Distance(tChild.position, vPos);
                if (m_fMinDist==0 || m_fMinDist >= fdist)
                {
                    m_fMinDist = fdist;
                    tRet = tChild;
                }
            }
            if (tRet == null) return null;
            return tRet.gameObject;
        }


        static public void TimerFuc(float fTime, CallBackFunction fuc)
        {
            GameObject gg = new GameObject();
            CWCloseEvent cs =gg.AddComponent<CWCloseEvent>();
            if (cs != null)
            {
                cs.CloseEvent = fuc;

            }
            GameObject.Destroy(gg, fTime);
        }

        static public void MakeDir(string szdir)
        {
            string filePath = Application.persistentDataPath;
            string m_szdir = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(filePath);
            try
            {

                if (!Directory.Exists(szdir))
                {
                    Directory.CreateDirectory(szdir);
                }

            }
            catch (IOException ex)
            {
                Console.WriteLine(ex.Message);
            }


            Directory.SetCurrentDirectory(m_szdir);



        }

        static public void DirectoryFuction(string szPath, FileFuction cbfunction)
        {
            DirectoryInfo dirs = new DirectoryInfo(szPath);
            FileInfo[] files = dirs.GetFiles();
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] != null)
                    {
                        cbfunction(files[i].FullName, files[i].Name);
                    }
                }
            }
        }

        static public void DeleteFile(string szPath,string szName)
        {
            DirectoryInfo dirs = new DirectoryInfo(szPath);
            FileInfo[] files = dirs.GetFiles();
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] != null)
                    {
                        if (files[i].Name.Contains(szName))
                        {
                            files[i].Delete();
                            files[i] = null;
                            return;
                        }
                    }
                }
                files = null;
            }

        }
        static public void DelDir(string szdir)
        {

            string filePath = Application.persistentDataPath;
            string szpath = filePath + "/" + szdir;
            DirectoryInfo file1 = new DirectoryInfo(szpath);
            deleteDirs(file1);
        }

        static public void deleteDirs(DirectoryInfo dirs,bool bdirdelete=false)
        {
            if (dirs == null || (!dirs.Exists))
            {
                return;
            }

            DirectoryInfo[] subDir = dirs.GetDirectories();
            if (subDir != null)
            {
                for (int i = 0; i < subDir.Length; i++)
                {
                    if (subDir[i] != null)
                    {
                        deleteDirs(subDir[i]);
                    }
                }
                subDir = null;
            }

            FileInfo[] files = dirs.GetFiles();
            if (files != null)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    if (files[i] != null)
                    {
            //            Debug.Log("파일 삭제:" + files[i].FullName + "__over");
                        files[i].Delete();
                        files[i] = null;
                    }
                }
                files = null;
            }

            // Debug.Log("폴더 삭제:" + dirs.FullName + "__over");
            if(bdirdelete)
                dirs.Delete();
        }
        // 알수 없는 버그 코드 삭제 요망
        //static public Texture2D TranTexture(Texture2D source, int targetWidth, int targetHeight)
        //{
        //    Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, true);
        //   // CWDebugManager.Instance.Log(string.Format("ScaleTexture {0} {1} {2}", targetWidth, targetHeight, source.format));

        //    float incX = (float)source.width / targetWidth;
        //    float incY = (float)source.height / targetHeight;

        //    float fdist = targetWidth / 2;
        //    for (int y = 0; y < targetWidth; y++)
        //    {
        //        for (int x = 0; x < targetHeight; x++)
        //        {
        //            float rx = (fdist - x);
        //            float ry = (fdist - y);
        //            float ff = (rx * rx) + (ry * ry);
        //            if (ff <= fdist * fdist)
        //            {
        //                int tx = (int)(x * incX);
        //                int ty = (int)(y * incY);
        //                Color color = source.GetPixel(tx, ty);//source.GetPixelBilinear(incX * x, incY * y);
        //                result.SetPixel(x, y, color);
        //            }
        //            else
        //            {
        //                result.SetPixel(x, y, new Color(0, 0, 0, 0));
        //            }
        //        }
        //    }

        //    result.Apply();
        //    return result;
        //}
        static public Texture2D ScaleTexture(Texture2D source, int targetWidth, int targetHeight)
        {
            Texture2D result = new Texture2D(targetWidth, targetHeight, TextureFormat.ARGB32, true);
            //CWDebugManager.Instance.Log(string.Format("ScaleTexture {0} {1} {2}", targetWidth, targetHeight, source.format));

            float incX =  (float)source.width/ targetWidth;
            float incY = (float)source.height / targetHeight; 

            float fdist = targetWidth / 2;
            for (int y=0;y< targetWidth; y++)
            {
                for (int x = 0; x < targetHeight; x++)
                {
                    float rx = (fdist - x);
                    float ry = (fdist - y);
                    float ff = (rx * rx) + (ry * ry);
                    if (ff <= fdist*fdist)
                    {
                        int tx = (int)(x * incX);
                        int ty = (int)(y * incY);
                        Color color = source.GetPixel(tx, ty);//source.GetPixelBilinear(incX * x, incY * y);
                        result.SetPixel(x, y, color);
                    }
                    else
                    {
                        result.SetPixel(x, y, new Color(255, 255, 255, 255));
                    }
                }
            }
            
            result.Apply();
            return result;
        }
        static public void SaveImage(Texture2D tImage,string szname)
        {

            string szPath;
            szPath = string.Format("{0}/{1}", Application.persistentDataPath, szname);

            Texture2D kNew = new Texture2D(tImage.width, tImage.height);
            kNew.SetPixels32(tImage.GetPixels32());
            kNew.Apply(false);


            //File.WriteAllBytes(szPath, kNew.EncodeToJPG());
            File.WriteAllBytes(szPath, kNew.EncodeToPNG());

        }
        static public Texture2D LoadImage(string szname,int dx,int dy)
        {

            string szPath ;
            szPath = string.Format("{0}/{1}", Application.persistentDataPath,szname);

            if (File.Exists(szPath))
            {

                try
                {
                    byte[] fileData = File.ReadAllBytes(szPath);
                    Texture2D Tx = new Texture2D(dx, dy);
                    Tx.LoadImage(fileData);
                    return Tx;
                }
                catch (System.Exception )
                {
                    DebugX.Log("exception " + szPath);	
                }

                
            }
            return null;
        }
        static public void CaptureImage(int nsize, Camera rdCam, string szPath, bool bJpg = false)
        {
            var oldRT = RenderTexture.active;
            Texture2D m_screenShot;
            m_screenShot = new Texture2D(nsize, nsize, TextureFormat.ARGB32, false);

            RenderTexture.active = rdCam.targetTexture;
            m_screenShot.ReadPixels(new Rect(0, 0, nsize, nsize), 0, 0);
            m_screenShot.Apply();
            


            
            try
            {
                if(bJpg)
                          File.WriteAllBytes(szPath, m_screenShot.EncodeToJPG());
                else
                    File.WriteAllBytes(szPath, m_screenShot.EncodeToPNG());
            }
            catch (System.Exception ex)
            {
                DebugX.Log("Exception " + szPath + ex.ToString());
            }
            

            RenderTexture.active = oldRT;
        }

        static public void CaptureImage(int nsize, RenderTexture kTexture, string szPath, bool bJpg = false)
        {
            var oldRT = RenderTexture.active;
            Texture2D m_screenShot;
            m_screenShot = new Texture2D(nsize, nsize, TextureFormat.ARGB32, false);

            RenderTexture.active = kTexture;
            m_screenShot.ReadPixels(new Rect(0, 0, nsize, nsize), 0, 0);
            m_screenShot.Apply();

            try
            {
                if (bJpg)
                    File.WriteAllBytes(szPath, m_screenShot.EncodeToJPG());
                else
                    File.WriteAllBytes(szPath, m_screenShot.EncodeToPNG());
            }
            catch (System.Exception ex)
            {
                DebugX.Log("Exception " + szPath + ex.ToString());
            }


            RenderTexture.active = oldRT;
        }


        static public Texture2D GetRTPixels(RenderTexture rt)
        {
            // Remember currently active render texture
            RenderTexture currentActiveRT = RenderTexture.active;

            // Set the supplied RenderTexture as the active one
            RenderTexture.active = rt;

            // Create a new Texture2D and read the RenderTexture image into it
            Texture2D tex = new Texture2D(rt.width, rt.height);
            tex.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
            tex.Apply();
            // Restorie previously active render texture
            RenderTexture.active = currentActiveRT;
            return tex;
        }
        
        static public void MakeImage( Camera rdCam, string szPath,Color kAlpha)
        {
                     byte [] pBuffer= MakeImageBuffer( rdCam, kAlpha);
                    if (pBuffer == null) return;
                    File.WriteAllBytes(szPath, pBuffer);
            

        }

        static public byte[] MakeImageBuffer( Camera rdCam, Color kAlpha)
        {

            Texture2D kTexture= MakeTexture( rdCam, kAlpha);
            if (kTexture == null) return null;
            return kTexture.EncodeToPNG();
        }
        static public Texture2D MakeTexture(Camera rdCam, Color kAlpha)
        {
            
            var oldRT = RenderTexture.active;
            RenderTexture.active = rdCam.targetTexture;
            int nsize = RenderTexture.active.width;

            Texture2D m_screenShot;
            m_screenShot = new Texture2D(nsize, nsize, TextureFormat.ARGB32, false);
            

            

            m_screenShot.ReadPixels(new Rect(0, 0, nsize, nsize), 0, 0);
            m_screenShot.Apply();
            RenderTexture.active = oldRT;


            int sx = 10000, sy = 10000, ex = 0, ey = 0;
            for (int y = 0; y < nsize; y++)
            {
                for (int x = 0; x < nsize; x++)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                        DebugX.Log("");
                    }
                    else
                    {
                        if (sx > x)
                        {
                            sx = x;
                        }
                        break;
                    }
                }
            }

            for (int x = 0; x < nsize; x++)
            {
                for (int y = 0; y < nsize; y++)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                        DebugX.Log("");
                    }
                    else
                    {
                        if (sy > y)
                        {
                            sy = y;
                        }
                        break;
                    }
                }
            }

            for (int y = 0; y < nsize; y++)
            {
                for (int x = nsize - 1; x >= 0; x--)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                        DebugX.Log("");
                    }
                    else
                    {
                        if (ex < x)
                        {
                            ex = x;
                        }
                        break;
                    }
                }
            }

            for (int x = 0; x < nsize; x++)
            {
                for (int y = nsize - 1; y >= 0; y--)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                        DebugX.Log("");
                    }
                    else
                    {
                        if (ey < y)
                        {
                            ey = y;
                        }
                        break;
                    }
                }
            }



            int nx, ny;
            nx = 1 + ex - sx;
            ny = 1 + ey - sy;

            if (nx < 0) return null;
            Texture2D kIcon = new Texture2D(nx, ny, TextureFormat.ARGB32, false);
            for (int y = sy; y <= ey; y++)
            {
                for (int x = sx; x <= ex; x++)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (cc == kAlpha)
                    {
                        cc = new Color(0, 0, 0, 0);
                    }
                    else
                    {
                        cc.a = 1;
                    }
                    kIcon.SetPixel(x - sx, y - sy, cc);
                }
            }

            kIcon.Apply(false);

     
            return kIcon;

        }
        public static MemoryStream compress(byte[] input,int nOffset=0)// 압축을 한다 
        {
            
            Deflater decompressor = new Deflater();
            decompressor.SetInput(input, nOffset, input.Length- nOffset);
            decompressor.Finish();

            MemoryStream bos = new MemoryStream();
            byte[] buf = new byte[1024];
            while (!decompressor.IsFinished)
            {
                int count = decompressor.Deflate(buf);
                bos.Write(buf, 0, count);
            }
            bos.Seek(0, SeekOrigin.Begin);
            return bos;
        }
        public static byte[] compressByte(byte[] input)// 압축을 한다 
        {

            Deflater decompressor = new Deflater();
            decompressor.SetInput(input, 0, input.Length);
            decompressor.Finish();

            MemoryStream bos = new MemoryStream();
            byte[] buf = new byte[1024];
            while (!decompressor.IsFinished)
            {
                int count = decompressor.Deflate(buf);
                bos.Write(buf, 0, count);
            }
            bos.Seek(0, SeekOrigin.Begin);
            return bos.ToArray();
        }

        public static MemoryStream Uncompress(byte[] input,int nOffset=0)// 압축을 푼다
        {
            if (input == null) return null;
            if (input.Length<10) return null;

            Inflater decompressor = new Inflater();
            decompressor.SetInput(input,nOffset,input.Length-nOffset);

            MemoryStream bos = new MemoryStream();
            // Decompress the data  
            byte[] buf = new byte[1024];
            while (!decompressor.IsFinished)
            {
                int count = decompressor.Inflate(buf);
                bos.Write(buf, 0, count);
            }
            bos.Seek(0, SeekOrigin.Begin);
            return bos;
        }

        public static byte[] UncompressByte(byte[] input, int nStart,int nLength)// 압축을 푼다
        {
            if (nLength == 0) return null;
            if(input==null)
            {
                return null;
            }

            try {
                Inflater decompressor = new Inflater();
                decompressor.SetInput(input, nStart, nLength);
                MemoryStream bos = new MemoryStream();
                // Decompress the data  
                byte[] buf = new byte[1024];
                while (!decompressor.IsFinished)
                {
                    int count = decompressor.Inflate(buf);
                    bos.Write(buf, 0, count);
                }
                bos.Seek(0, SeekOrigin.Begin);
                return bos.ToArray();

            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log(ex.ToString());
            }

            return null;

        }



        static public byte[] ConvertBin(JToken JData)
        {
            if (JData == null) return null;
            MemoryStream ms = new MemoryStream();
            using (BsonWriter writer = new BsonWriter(ms))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(writer, JData);
                MemoryStream cdata = CWLib.compress(ms.ToArray());
                return cdata.ToArray();
            }
        }
        // bin -> Json
        static public JToken ConvertJSon(byte[] bdata)
        {
            if (bdata == null) return null;
            MemoryStream ms = CWLib.Uncompress(bdata);
            JToken JData;
            using (BsonReader reader = new BsonReader(ms))
            {
                JData = JToken.ReadFrom(reader);
            }
            return JData;
        }

        static string GetValueMoney(int num)
        {
            if (num == 0) return "";
            if (num == 1) return "K";
            if (num == 2) return "M";
            if (num == 3) return "B";
            if (num == 4) return "T";
            if (num == 5) return "Q";
            return "MAX";

        }
        static public string ConvertMoney(string szMoney)
        {



            int n1 = (szMoney.Length - 1) / 3;
            int n2 = (szMoney.Length - 1) % 3 + 1;
            if (n1 == 0) return szMoney;

            long ll = ConvertLong(szMoney);
            if (ll >= 99999999999999)
            {
                return "MAX";
            }

            string sz1 = GetValueMoney(n1);
            string sz = szMoney.Substring(0, n2);
            string sz2 = szMoney.Substring(n2, 2);

            sz += ".";
            sz += sz2;

            if (sz1 == "MAX") return "MAX";
            sz += sz1;
            return sz;
        }

        static public Vector3 GetCenterPos()
        {
            return CWHeroManager.Instance.GetPosition();
        }
        // 숫자인지 판별
        static public bool IsDigit(string szValues)
        {
            int result = 0;
            if (!int.TryParse(szValues, out result)) return false;
            return true;

        }
        //1글자 이상
        static public bool IsString(string szvalue)
        {
            if (szvalue == null) return false;
            if (szvalue == "") return false;
            if (szvalue.Length<1) return false;
            return true;
        }
        static public string GetTodayStringbyDay()
        {
            DateTime tt = DateTime.Now;
            return string.Format("{0}_{1}_{2}", tt.Year, tt.Month, tt.Day);
        }

        static public string GetTodayString()
        {
            DateTime tt = DateTime.Now;
            return string.Format("{0}_{1}_{2}_{3}_{4}_{5}", tt.Year,tt.Month,tt.Day,tt.Hour,tt.Minute,tt.Second);
        }
        // 현재로 부터 몇일 전인가?
        static public string GetStringDateBefore(DateTime tt)
        {
            TimeSpan ts =  tt- DateTime.Now;
            string szVal = CWLocalization.Instance.GetLanguage("{0}");
            return string.Format(szVal, GetTimeString((float)ts.TotalSeconds));
        }
        static public string GetStringDate(DateTime tt)
        {
            return tt.ToString("yyyy/MM/dd");
        }
        static public string GetDateStringbyTime(int Sec)
        {
            int hour = ((int)Sec / 3600);
            int min = ((int)Sec / (60)) % 60;
            int sec = Sec % 60;
            string szVal = CWLocalization.Instance.GetLanguage("{0:00}:{1:00}:{2:00}");
            return string.Format(szVal,  hour, min, sec);
        }

        static public string GetDateString(int Sec)
        {
            int day = (int)Sec / (3600 * 24);
            int hour = ((int)Sec / 3600) % 24;
            int min = ((int)Sec / (60)) % 60;
            int sec = Sec % 60;
            string szVal = CWLocalization.Instance.GetLanguage("{0}일 {1:00}:{2:00}:{3:00}");
            return string.Format(szVal, day,hour,min,sec);
        }
        static public string GetTimeString(float fSec)
        {
            if(fSec<=0)
            {
                return "";
            }
            if (fSec > 3600 * 24)// 하루 보다 크다면 1일단위
            {
                int nVal1 = (int)fSec / (3600 * 24);
                int nVal2 = ((int)fSec / 3600) % 24;
                string szVal = CWLocalization.Instance.GetLanguage("{0}일{1}시");
                return string.Format(szVal, nVal1, nVal2);
            }
            else if (fSec > 3600)//1시간보다 크다면, 시간단위
            {
                int nVal1 = (int)fSec / (3600);
                int nVal2 = ((int)fSec / (60)) % 60;
                int nVal3 = (int)fSec % 60;
                string szVal = CWLocalization.Instance.GetLanguage("{0}:{1:00}:{2:00}");
                return string.Format(szVal, nVal1, nVal2, nVal3);
            }
            else if(fSec>60)
            {
                int nVal1 = (int)fSec / 60;
                int nVal2 = (int)fSec % 60;
                string szVal =CWLocalization.Instance.GetLanguage("{0:00}:{1:00}");
                return string.Format(szVal, nVal1, nVal2);
            }
            else
            {
                int nVal1 = (int)fSec;
                string szVal = CWLocalization.Instance.GetLanguage("00:{0:00}");
                return string.Format(szVal, nVal1);

            }


        }

        public static Texture2D ResizeTexture(Texture2D pSource, Vector2 size, ImageFilterMode pFilterMode= ImageFilterMode.Biliner)
        {

            //*** Variables
            int i;

            //*** Get All the source pixels
            Color[] aSourceColor = pSource.GetPixels(0);
            Vector2 vSourceSize = new Vector2(pSource.width, pSource.height);

            //*** Calculate New Size
            float xWidth = size.x;
            float xHeight = size.y;

            //*** Make New
            Texture2D oNewTex = new Texture2D((int)xWidth, (int)xHeight, TextureFormat.RGBA32, false);

            //*** Make destination array
            int xLength = (int)xWidth * (int)xHeight;
            Color[] aColor = new Color[xLength];

            Vector2 vPixelSize = new Vector2(vSourceSize.x / xWidth, vSourceSize.y / xHeight);

            //*** Loop through destination pixels and process
            Vector2 vCenter = new Vector2();
            for (i = 0; i < xLength; i++)
            {

                //*** Figure out x&y
                float xX = (float)i % xWidth;
                float xY = Mathf.Floor((float)i / xWidth);

                //*** Calculate Center
                vCenter.x = (xX / xWidth) * vSourceSize.x;
                vCenter.y = (xY / xHeight) * vSourceSize.y;

                //*** Do Based on mode
                //*** Nearest neighbour (testing)
                if (pFilterMode == ImageFilterMode.Nearest)
                {

                    //*** Nearest neighbour (testing)
                    vCenter.x = Mathf.Round(vCenter.x);
                    vCenter.y = Mathf.Round(vCenter.y);

                    //*** Calculate source index
                    int xSourceIndex = (int)((vCenter.y * vSourceSize.x) + vCenter.x);

                    //*** Copy Pixel
                    aColor[i] = aSourceColor[xSourceIndex];
                }

                //*** Bilinear
                else if (pFilterMode == ImageFilterMode.Biliner)
                {

                    //*** Get Ratios
                    float xRatioX = vCenter.x - Mathf.Floor(vCenter.x);
                    float xRatioY = vCenter.y - Mathf.Floor(vCenter.y);

                    //*** Get Pixel index's
                    int xIndexTL = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexTR = (int)((Mathf.Floor(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));
                    int xIndexBL = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Floor(vCenter.x));
                    int xIndexBR = (int)((Mathf.Ceil(vCenter.y) * vSourceSize.x) + Mathf.Ceil(vCenter.x));

                    //*** Calculate Color
                    aColor[i] = Color.Lerp(
                        Color.Lerp(aSourceColor[xIndexTL], aSourceColor[xIndexTR], xRatioX),
                        Color.Lerp(aSourceColor[xIndexBL], aSourceColor[xIndexBR], xRatioX),
                        xRatioY
                    );
                }

                //*** Average
                else if (pFilterMode == ImageFilterMode.Average)
                {

                    //*** Calculate grid around point
                    int xXFrom = (int)Mathf.Max(Mathf.Floor(vCenter.x - (vPixelSize.x * 0.5f)), 0);
                    int xXTo = (int)Mathf.Min(Mathf.Ceil(vCenter.x + (vPixelSize.x * 0.5f)), vSourceSize.x);
                    int xYFrom = (int)Mathf.Max(Mathf.Floor(vCenter.y - (vPixelSize.y * 0.5f)), 0);
                    int xYTo = (int)Mathf.Min(Mathf.Ceil(vCenter.y + (vPixelSize.y * 0.5f)), vSourceSize.y);

                    //*** Loop and accumulate
                    Vector4 oColorTotal = new Vector4();
                    Color oColorTemp = new Color();
                    float xGridCount = 0;
                    for (int iy = xYFrom; iy < xYTo; iy++)
                    {
                        for (int ix = xXFrom; ix < xXTo; ix++)
                        {

                            //*** Get Color
                            oColorTemp += aSourceColor[(int)(((float)iy * vSourceSize.x) + ix)];

                            //*** Sum
                            xGridCount++;
                        }
                    }

                    //*** Average Color
                    aColor[i] = oColorTemp / (float)xGridCount;
                }
            }

            //*** Set Pixels
            oNewTex.SetPixels(aColor);
            oNewTex.Apply();

            //*** Return
            return oNewTex;
        }

        // 같은 색을 고름
        public static bool CompareColor(Color c1, Color c2, float fdelta = 0.01f)
        {
            Vector3 v1 = new Vector3(c1.r, c1.g, c1.b);
            Vector3 v2 = new Vector3(c2.r, c2.g, c2.b);
            float fdist = Vector3.Distance(v1, v2);
            if (fdist <= fdelta) return true;

            return false;

        }

        // 바운드에 존재하는지 검사
        public static bool IsInsideBounds(Vector3 worldPos, BoxCollider bc)
        {
            Vector3 localPos = bc.transform.InverseTransformPoint(worldPos);
            Vector3 delta = localPos - bc.center + bc.size * 0.5f;
            return Vector3.Max(Vector3.zero, delta) == Vector3.Min(delta, bc.size);
        }


    }
    #region 화면캡쳐
    public delegate void ResultFuction(Texture2D kTex);
    class CaptrueObject : MonoBehaviour
    {
        

        ResultFuction CBFunction;
        Camera m_kCam;
        public void MakeTexture(Camera rdCam, ResultFuction func)
        {
            CBFunction = func;
            m_kCam = rdCam;
            StartCoroutine("IRun");
        }


        IEnumerator IRun()
        {
            yield return new WaitForEndOfFrame();
            Color kAlpha = m_kCam.backgroundColor;

            var oldRT = RenderTexture.active;
            RenderTexture.active = m_kCam.targetTexture;
            int nsize = RenderTexture.active.width;

            Texture2D m_screenShot;
            m_screenShot = new Texture2D(nsize, nsize, TextureFormat.ARGB32, false);

            m_screenShot.ReadPixels(new Rect(0, 0, nsize, nsize), 0, 0);
            m_screenShot.Apply();
            RenderTexture.active = oldRT;


            int sx = 10000, sy = 10000, ex = 0, ey = 0;
            for (int y = 0; y < nsize; y++)
            {
                for (int x = 0; x < nsize; x++)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                   //     DebugX.Log("");
                    }
                    else
                    {
                        if (sx > x)
                        {
                            sx = x;
                        }
                        break;
                    }
                }
            }

            for (int x = 0; x < nsize; x++)
            {
                for (int y = 0; y < nsize; y++)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                //        DebugX.Log("");
                    }
                    else
                    {
                        if (sy > y)
                        {
                            sy = y;
                        }
                        break;
                    }
                }
            }

            for (int y = 0; y < nsize; y++)
            {
                for (int x = nsize - 1; x >= 0; x--)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                    //    DebugX.Log("");
                    }
                    else
                    {
                        if (ex < x)
                        {
                            ex = x;
                        }
                        break;
                    }
                }
            }

            for (int x = 0; x < nsize; x++)
            {
                for (int y = nsize - 1; y >= 0; y--)
                {
                    Color cc = m_screenShot.GetPixel(x, y);
                    if (CWMath.IsEqual(cc, kAlpha))
                    {
                      //  DebugX.Log("");
                    }
                    else
                    {
                        if (ey < y)
                        {
                            ey = y;
                        }
                        break;
                    }
                }
            }



            int nx, ny;
            nx = 1 + ex - sx;
            ny = 1 + ey - sy;

            if (nx > 1)
            {
                Texture2D kIcon = new Texture2D(nx, ny, TextureFormat.ARGB32, false);
                for (int y = sy; y <= ey; y++)
                {
                    for (int x = sx; x <= ex; x++)
                    {
                        Color cc = m_screenShot.GetPixel(x, y);
                        if (cc == kAlpha)
                        {
                            cc = new Color(0, 0, 0, 0);
                        }
                        else
                        {
                            cc.a = 1;
                        }
                        kIcon.SetPixel(x - sx, y - sy, cc);
                    }
                }
                kIcon.Apply(false);
                CBFunction(kIcon);
            }

            GameObject.Destroy(gameObject, 0.01f);
        }
    }
    static public class CWCapture
    {
        static string m_szPath;
        static public void MakeTexture(Camera cam, ResultFuction cbfuc)
        {
            GameObject gg = new GameObject();
            CaptrueObject tt = gg.AddComponent<CaptrueObject>();
            tt.MakeTexture(cam,cbfuc);
        }
        static void SaveFuction(Texture2D kTexture)
        {
            File.WriteAllBytes(m_szPath, kTexture.EncodeToPNG());
        }
        static public void MakeFile(Camera cam, string szPath)
        {
            m_szPath = szPath;
            GameObject gg = new GameObject();
            CaptrueObject tt = gg.AddComponent<CaptrueObject>();
            tt.MakeTexture(cam, SaveFuction);
        }

    }

    #endregion
    /*
     *  이 객체 개념 정리
     *  
     *  움직임에 있어서, 시간으로 조정하는 개념이 아니라, 속도로 조정하는 개념 
     *  이유는 움직이는 액션이 중요하다. 액션 이펙트에 사용 
     *  점점 빨라지는 개념의 움직임으로 사용 
     *  시간으로 하는 개념은 DO Tween으로 사용한다 
     *  DO Tween의 보완 개념으로 개발 
     * */

    #region TWEEN 작업  움직임 관련 

    class TWEENRUN : MonoBehaviour
    {
        Transform m_tTrans;
        Vector3 m_vEndPos;
        float m_fSpeed = 1f;

        IEnumerator IRun()
        {
            float fPrefdist = 0;
            float fTime = Time.time;
            float fValues = 0;
            while (true)
            {
                if(m_tTrans==null)
                {
                    break;
                }
                // 무조건 3초 이상은 생존하면 안된다
                float ff = Time.time - fTime;
                if(ff>3f)
                {
                    break;
                }
                m_tTrans.localPosition = Vector3.Lerp(m_tTrans.localPosition, m_vEndPos, fValues * Time.deltaTime);

                // 버그가 나올 가능성 : 가까워 졌다가 갑자기 멀어지면 버그가 나온다 
                if (CWMath.IsEqual(m_tTrans.localPosition, m_vEndPos,1f))
                {
                    break;
                }
                float fdist = Vector3.Distance(m_tTrans.localPosition, m_vEndPos);
                if(fPrefdist!=0&& fdist > fPrefdist)// 점점 가까워 지지 안았다
                {
                    break;// 가까워 진걸로 본다 
                }
                fPrefdist = fdist;
                fValues += Time.deltaTime* m_fSpeed;
                yield return null;
            }

            GameObject.Destroy(gameObject,0.01f);
        }
        public void Run(Transform trans,Vector3 vEnd,float fSpeed)
        {
            m_vEndPos = vEnd;
            m_tTrans = trans;
            m_fSpeed = fSpeed;

            StartCoroutine("IRun");
        }
    }

    // 기본적으로 Lerp 움직인다, 점점 빨라지는 효과를 준다 
    static public class CWTWEEN
    {
        static public void CWMove(this Transform trans,Vector3 vWhere,float fSpeed)
        {
            GameObject gg = new GameObject();
            TWEENRUN tt = gg.AddComponent<TWEENRUN>();
            tt.Run(trans, vWhere, fSpeed);

        }
        static public void CWHide(this Transform trans, CallBackFunction cbFunc)
        {
            CWHideAction cs= trans.gameObject.AddComponent<CWHideAction>();
            cs.m_CBFunc = cbFunc;
        }

        // 현재에서 어느 방향으로 

    }

    // 기본적으로 Lerp 움직인다, 점점 빨라지는 효과를 준다 

    #endregion

    #region CWTOOL

    static public class CWTOOL
    {
        static public GameObject AddChild(this Transform parent, GameObject gObject)
        {
            
            var t = gObject.transform;
            t.parent = parent;
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            gObject.SetActive(true);
            return gObject;
        }

        



    }

    #endregion

    #region 랜덤 중복되지 않는 랜덤값을 준다

    public class SRandom
    {
        static List<int> m_kList = new List<int>();
        static public void Create(int cnt)
        {
            m_kList.Clear();
            List<int> ktemp = new List<int>();
            for (int i = 0; i < cnt; i++) ktemp.Add(i);
            for(int i=0;i<cnt;i++)
            {
                int vv = CWLib.Random(0, ktemp.Count);
                m_kList.Add(ktemp[vv]);
                ktemp.RemoveAt(vv);
            }
            

        }
        static public int GetNextValue()
        {
            if (m_kList.Count == 0)
            {
                return 0;
            }
                
            int vv= m_kList[0];
            m_kList.RemoveAt(0);
            return vv;
        }
    }

    #endregion

}

