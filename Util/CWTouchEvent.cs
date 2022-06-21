using UnityEngine;
using System.Collections;
/*
 * 터치 이벤트
 * 
 * 빠르게 누르면서 띈다면 : 슈팅
 * 누르면서 이동 : 카메라 회전 및 본체 회전
 * 누르고 있을 때 : 앞으로 전진
 * 길게 누르면 조이스틱이 나온다. 
 * 조이스틱이 나오면 , 조이스틱에 조정하는 것으로 이동한다.
 * 
 * 
 * 
*/


public class CWTouchEvent : MonoBehaviour
{

    bool m_bDrag = false;
    bool m_bDown=false;// 눌러짐
    float m_fTouchDist=0;

    float m_fStartTimer = 0;
    public float m_fLongDownTime = 0.5f;//0.5 이상 누르면 길게 누른 신호로 간주 
    public float m_fDistSencer=1f; // 움직임 감지 
    Vector3 m_vPreVector=new Vector3();

    BoxCollider m_bCollider;

    public delegate void DgTouchUpFuc(Vector3 vPos);
    public delegate void DgTouchLongDowning(Vector3 vPos);
    public delegate void DgTouchDraging(Vector3 vDrag);
    public delegate void DgTouchScale(bool bflag);
    

    public DgTouchUpFuc TouchUpFuc = null;
    public DgTouchLongDowning TouchLongDowning = null;
    public DgTouchDraging TouchDraging = null;
    public DgTouchScale TouchScaleFuc = null;

    public void Clear()
    {
        m_bDrag = false;
        m_bDown = false;
        m_fStartTimer = 0;
    }

    bool CheckBox(Vector3 vPos)
    {
        float cx = Screen.width / 2 +  m_bCollider.center.x;
        float cy = Screen.height / 2 + m_bCollider.center.y;

        float sx = cx - m_bCollider.size.x / 2;
        float sy = cy - m_bCollider.size.y / 2;
        float ex = cx + m_bCollider.size.x / 2;
        float ey = cy + m_bCollider.size.y / 2;

//        string str = vPos.ToString() + "(" + sx.ToString() + "," + sy.ToString() + "," + ex.ToString() + "," + ey.ToString();

        
        if (vPos.x < sx) return false;
        if (vPos.y < sy) return false;
        if (vPos.x > ex) return false;
        if (vPos.y > ex) return false;



        return true;
    }

    public void Run()
    {

//#if UNITY_EDITOR
//        //if(m_kBound!=null)
//        //{
//        //    if (!m_kBound.Contains(new Vector2(Input.mousePosition.x, Input.mousePosition.y))) return;
//        //}

//        if (!CheckBox(Input.mousePosition))
//        {
//            return;
//        }

        
//        //if (!m_bCollider.bounds.Contains(Input.mousePosition))
//        //{
//        //    return;
//        //}
        
//        if (Input.GetMouseButtonDown(0))
//        {
//            m_bDrag = false;
//            m_bDown = true;// 눌러짐
//            m_fStartTimer = Time.time;
//            m_vPreVector = Input.mousePosition;
            
            
//        }
//        if (TouchScaleFuc != null)
//        {
//            if (Input.GetAxis("Mouse ScrollWheel") > 0)
//            {
//                TouchScaleFuc(true);
//            }
//            if (Input.GetAxis("Mouse ScrollWheel") < 0)
//            {
//                TouchScaleFuc(false);
//            }
//        }

//        if (m_bDown)// 눌러짐
//        {
//            if (Vector3.Distance(m_vPreVector, Input.mousePosition) > m_fDistSencer)
//            {
//                m_bDrag = true;
//            }
//            Vector3 vDelta = Input.mousePosition - m_vPreVector;
//            m_vPreVector = Input.mousePosition;
//            bool bUp = false;
//            if (Input.GetMouseButtonUp(0))
//            {
//                Clear();
//                bUp = true;
//            }
//            // 드래깅 체크
//            if (!m_bDrag)
//            {
//                m_vPreVector = Input.mousePosition;
//                if (m_fStartTimer>0 &&Time.time - m_fStartTimer > m_fLongDownTime)
//                {
//                    if (TouchLongDowning!=null)
//                    {
//                        TouchLongDowning(m_vPreVector);
//                        Clear();
//                        return;

//                    }
//                }
//                if (bUp)
//                {
//                    if (TouchUpFuc!=null)
//                    {
//                        TouchUpFuc(m_vPreVector);
//                    }
                    
//                }
//            }
//            else
//            {
//                if (TouchDraging!=null)
//                {
//                    TouchDraging(vDelta);// 드래깅 중
//                }
                
//            }

//        }

//#else
        int nCount = 0;
        Touch [] tc = new Touch[2];

        for (int i = 0; i < Input.touchCount; i++)
        {
            if (CheckBox(Input.GetTouch(i).position))
            {
                tc[nCount] = new Touch();
                nCount++;
                if (nCount >= 2) break;
            }
        }

        if (nCount == 2)
        {


            if (tc[0].phase == TouchPhase.Began || tc[1].phase == TouchPhase.Began)
            {
                Clear();
                // 스크롤 시작
                Vector3 vTouchPos1 = tc[0].position;
                Vector3 vTouchPos2 = tc[1].position;
                float fDist = Vector3.Distance(vTouchPos1,vTouchPos2);
                m_fTouchDist =fDist;
                
            }
            else if (tc[0].phase == TouchPhase.Moved || tc[1].phase == TouchPhase.Moved)
            {
                Vector3 vTouchPos1 = tc[0].position;
                Vector3 vTouchPos2 = tc[1].position;
                float fDist = Vector3.Distance(vTouchPos1,vTouchPos2);
                if (m_fTouchDist<fDist)
                {
                    TouchScaleFuc(true);
                    
                }
                else
                {
                    TouchScaleFuc(false );
                    
                }
                
                m_fTouchDist =fDist;

            }

        }
        if (nCount == 1)
        {

            if (tc[0].phase == TouchPhase.Began)
            {
                Clear();
                m_bDrag = false;
                m_bDown = true;// 눌러짐
                m_fStartTimer = Time.time;
                m_vPreVector = tc[0].position;
            }
            Vector3 vTouchPos = tc[0].position;
            if (m_bDown)// 눌러짐
            {
                if (Vector3.Distance(m_vPreVector, vTouchPos) > m_fDistSencer)
                {
                    m_bDrag = true;
                }
                Vector3 vDelta = vTouchPos - m_vPreVector;
                m_vPreVector = Input.mousePosition;
                bool bUp = false;
                if (Input.GetMouseButtonUp(0))
                {
                    Clear();
                    bUp = true;
                }
                // 드래깅 체크
                if (!m_bDrag)
                {
                    m_vPreVector = vTouchPos;
                    if (m_fStartTimer > 0 && Time.time - m_fStartTimer > m_fLongDownTime)
                    {
                        TouchLongDowning(m_vPreVector);
                        Clear();
                        return;
                    }
                    if (bUp)
                    {
                        TouchUpFuc(m_vPreVector);
                    }
                }
                else
                {
                    TouchDraging(vDelta);// 드래깅 중
                }

            }

        }

//#endif
    }

    private void Start()
    {
        m_bCollider = GetComponent<BoxCollider>();
    }

    float ZoomMinBound = 0.1f;
    float ZoomMaxBound = 179.9f;
    float MouseZoomSpeed = 15.0f;
    float TouchZoomSpeed = 0.1f;
    void Zoom(float deltaMagnitudeDiff, float speed)
    {
        Camera cam = Camera.main;
        cam.fieldOfView += deltaMagnitudeDiff * speed;
        // set min and max value of Clamp function upon your requirement
        cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, ZoomMinBound, ZoomMaxBound);
    }

    private void Update()
    {

        if (Input.touchSupported)
        {
            // Pinch to zoom
            if (Input.touchCount == 2)
            {
                // get current touch positions
                Touch tZero = Input.GetTouch(0);
                Touch tOne = Input.GetTouch(1);
                // get touch position from the previous frame
                Vector2 tZeroPrevious = tZero.position - tZero.deltaPosition;
                Vector2 tOnePrevious = tOne.position - tOne.deltaPosition;

                float oldTouchDistance = Vector2.Distance(tZeroPrevious, tOnePrevious);
                float currentTouchDistance = Vector2.Distance(tZero.position, tOne.position);

                // get offset value
                float deltaDistance = oldTouchDistance - currentTouchDistance;
                Zoom(deltaDistance, TouchZoomSpeed);
            }
        }
        else
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            Zoom(scroll, MouseZoomSpeed);
        }


    }



}
